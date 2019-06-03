using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    class MainGameScreen : GameScreen
    {
        Zombicide game;
        public Random RNG;
        enum MoveState { PlayerTurn, ZombieTurn }
        MoveState whosTurn;
        private Texture2D resetMapButton;
        private Texture2D Highlight;
        private Texture2D closedDoor;
        private Texture2D openDoor;
        private Texture2D zombieIcon;

        Texture2D player;

        TiledSharp.TmxMap map;
        Texture2D mapPic;
        int mapWidth = 800;
        int mapHeight = 800;
        int mapX;
        int mapY;
        int tileWidth;
        int tileHeight;
        int tilesHigh;
        int tilesWide;

        List<Tile> tileData;
        List<Tile> litTiles;
        List<Tile> doorTiles;
        List<Zombie> zombieList;
        List<Dice> DiceList;

        int resetMapX;
        int resetMapY;

        public Paragraph life { get; set; }
        public Paragraph moves { get; set; }
        public Paragraph level { get; set; }
        public Paragraph experience { get; set; }

        public MainGameScreen(Zombicide game)
        {
            LoadMainGame(game);
            this.game = game;
            RNG = new Random();
        }

        public override void Update(Zombicide game)
        {
            int mapSpeed = 10;

            //Update UI components
            life.Text = ("Life: " + (game.ActiveCharacter.DeathThreshold - game.ActiveCharacter.GetDamageTaken()).ToString());
            moves.Text = ("Moves Left: " + (game.ActiveCharacter.movesLeft).ToString());
            level.Text = ("Level: " + (game.ActiveCharacter.Level).ToString());
            experience.Text = ("Experience: " + (game.ActiveCharacter.Experience).ToString());


            if (game.PreviousMouseState.LeftButton == ButtonState.Pressed &&
                game.MouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(game, 1);
            }
            if (game.PreviousMouseState.RightButton == ButtonState.Pressed &&
                game.MouseState.RightButton == ButtonState.Released)
            {
                MouseClicked(game, 2);
            }

            int mapCenterX = mapX + (mapWidth / 2);
            int mapCenterY = mapY + (mapHeight / 2);

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && mapCenterY > 0) { mapY -= mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && mapCenterX > 400) { mapX -= mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && mapCenterX < game.GraphicsDevice.Viewport.Width - 400) { mapX += mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && mapCenterY < game.GraphicsDevice.Viewport.Height) { mapY += mapSpeed; }
            tileHeight = mapHeight / tilesHigh;
            tileWidth = mapWidth / tilesWide;
            game.ActiveCharacter.PlayerX = mapX + (Convert.ToInt32(game.ActiveCharacter.PlayerTile.column) * tileWidth) + (tileWidth / 2);
            game.ActiveCharacter.PlayerY = mapY + (Convert.ToInt32(game.ActiveCharacter.PlayerTile.row) * tileHeight) + (tileHeight / 2);

            if (whosTurn == MoveState.PlayerTurn)
            {

                applyMoveTiles(game);
                if (game.ActiveCharacter.movesLeft <= 0) { whosTurn = MoveState.ZombieTurn; }
            }

            if (whosTurn == MoveState.ZombieTurn)
            {
                foreach (Zombie Z in zombieList)
                {
                    if (game.ActiveCharacter.PlayerTile.row == Z.ZombieTile[0] && game.ActiveCharacter.PlayerTile.column == Z.ZombieTile[1])
                    { Z.attackPlayer(); }
                    else { Z.move(tileData, game.ActiveCharacter); }

                }


                whosTurn = MoveState.PlayerTurn;
                game.ActiveCharacter.resetMoves(moves);
            }
        }

        public override void Draw(Zombicide game)
        {
            game.SpriteBatch.Draw(mapPic, new Rectangle(mapX, mapY, mapWidth, mapHeight), Color.White);
            game.SpriteBatch.Draw(player, new Rectangle(game.ActiveCharacter.PlayerX - 15, game.ActiveCharacter.PlayerY - 15, 40, 40), Color.White);
            game.SpriteBatch.Draw(resetMapButton, new Rectangle(resetMapX, resetMapY, 54, 54), Color.White);

            if (whosTurn == MoveState.PlayerTurn)
            {
                foreach (Tile T in litTiles)
                {
                    game.SpriteBatch.Draw(Highlight, new Rectangle(mapX + T.column * tileWidth, mapY + T.row * tileHeight, tileWidth, tileHeight), Color.White);
                }
            }
            foreach (Tile T in tileData)
            {
                if (T.LeftSide == RoomSide.closeddoor) { game.SpriteBatch.Draw(closedDoor, new Rectangle(mapX + (T.column * tileWidth) - 25, mapY + (T.row * tileHeight) + (tileHeight / 2) - 25, 50, 50), Color.White); }
                if (T.LeftSide == RoomSide.opendoor) { game.SpriteBatch.Draw(openDoor, new Rectangle(mapX + (T.column * tileWidth) - 25, mapY + (T.row * tileHeight) + (tileHeight / 2) - 25, 50, 50), Color.White); }
                if (T.TopSide == RoomSide.closeddoor) { game.SpriteBatch.Draw(closedDoor, new Rectangle(mapX + (T.column * tileWidth) + (tileWidth / 2) - 25, mapY + (T.row * tileHeight) - 25, 50, 50), Color.White); }
                if (T.TopSide == RoomSide.opendoor) { game.SpriteBatch.Draw(openDoor, new Rectangle(mapX + (T.column * tileWidth) + (tileWidth / 2) - 25, mapY + (T.row * tileHeight) - 25, 50, 50), Color.White); }

            }
            foreach (Zombie Z in zombieList)
            {
                game.SpriteBatch.Draw(zombieIcon, new Rectangle(mapX + Z.ZombieTile[1] * tileWidth, mapY + Z.ZombieTile[0] * tileHeight, 40, 40), Color.White);
            }


        }

        void LoadMainGame(Zombicide game)
        {
            //Initialize map
            map = new TiledSharp.TmxMap(@"Content/Tiled/Big Game Hunting.tmx");
            mapPic = game.Content.Load<Texture2D>(@"Tiled/Big Game Hunting");
            Highlight = game.Content.Load<Texture2D>(@"Highlight");
            closedDoor = game.Content.Load<Texture2D>(@"ClosedDoor");
            openDoor = game.Content.Load<Texture2D>(@"OpenDoor");
            zombieIcon = game.Content.Load<Texture2D>(@"ZombieIcon");
            mapX = (game.GraphicsDevice.Viewport.Width / 2) - (mapWidth / 2);
            mapY = (game.GraphicsDevice.Viewport.Height / 2) - (mapHeight / 2);
            tilesHigh = Convert.ToInt32(map.Layers[0].Properties.Where(x => x.Key == "TilesHigh").FirstOrDefault().Value);
            tilesWide = Convert.ToInt32(map.Layers[0].Properties.Where(x => x.Key == "TilesWide").FirstOrDefault().Value);
            tileWidth = mapWidth / tilesWide;
            tileHeight = mapHeight / tilesHigh;
            litTiles = new List<Tile>();
            doorTiles = new List<Tile>();
            tileData = LoadTileData("BigGameHunting");
            foreach (Tile T in tileData)
            {
                if (T.LeftSide == RoomSide.closeddoor || T.LeftSide == RoomSide.opendoor || T.TopSide == RoomSide.closeddoor || T.TopSide == RoomSide.opendoor)
                { doorTiles.Add(T); }
            }

            //Place reset map Button
            resetMapButton = game.Content.Load<Texture2D>(@"CenterMapButton");
            resetMapX = 1200 - 54;
            resetMapY = game.GraphicsDevice.Viewport.Height - 54;

            //read in Start tile from map data and place player there
            player = game.Content.Load<Texture2D>(@"playerIcons/" + game.ActiveCharacter.PlayerIcon);
            var startTile = map.Layers[0].Properties.Where(x => x.Key == "StartTile").FirstOrDefault().Value.Split(',');
            game.ActiveCharacter.PlayerTile.row = Convert.ToInt32(startTile[0]);
            game.ActiveCharacter.PlayerTile.column = Convert.ToInt32(startTile[1]);
            game.ActiveCharacter.PlayerX = mapX + (Convert.ToInt32(startTile[1]) * tileWidth) + (tileWidth / 2);
            game.ActiveCharacter.PlayerY = mapY + (Convert.ToInt32(startTile[0]) * tileHeight) + (tileHeight / 2);

            //Initialize UI for Main game screen
            PanelTabs tabs = new PanelTabs();
            Panel movePanel = new Panel(new Vector2(400, 800), PanelSkin.Simple, Anchor.CenterRight);
            Panel playerPanel = new Panel(new Vector2(400, 750), PanelSkin.Default, Anchor.TopLeft, new Vector2(0, 50));
            playerPanel.AddChild(tabs);

            TabData tab1 = tabs.AddTab("Player");
            life = new Paragraph("Life: " + (game.ActiveCharacter.DeathThreshold - game.ActiveCharacter.GetDamageTaken()).ToString());
            moves = new Paragraph("Moves Left: " + (game.ActiveCharacter.movesLeft).ToString());
            level = new Paragraph("Level: " + (game.ActiveCharacter.Level).ToString());
            experience = new Paragraph("Experience: " + (game.ActiveCharacter.Experience).ToString());

            var test = new Button("test Zombie");
            test.OnClick = (Entity btn) =>
            {
                
                zombieList.Add(new Zombie(5, 1, tileData.Find(x => x.row == 5 && x.column == 1), game));
            };


            tab1.button.Padding = new Vector2(0, 0);
            tab1.button.Size = new Vector2(100, 50);
            tab1.button.Offset = new Vector2(0, -50);
            tab1.panel.Offset = new Vector2(0, -50);
            tab1.panel.AddChild(new Header(game.ActiveCharacter.CharacterName));
            tab1.panel.AddChild(new HorizontalLine());
            tab1.panel.AddChild(life);
            tab1.panel.AddChild(moves);
            tab1.panel.AddChild(level);
            tab1.panel.AddChild(experience);
            tab1.panel.AddChild(test);
            tab1.button.ButtonParagraph.Scale = .9f;

            TabData tab2 = tabs.AddTab("Backpack");
            tab2.button.Padding = new Vector2(0, 0);
            tab2.button.Size = new Vector2(100, 50);
            tab2.button.ButtonParagraph.Scale = .9f;

            TabData tab3 = tabs.AddTab("Team");
            tab3.button.Padding = new Vector2(0, 0);
            tab3.button.Size = new Vector2(100, 50);
            tab3.button.ButtonParagraph.Scale = .9f;

            UserInterface.Active.AddEntity(playerPanel);
            UserInterface.Active.AddEntity(movePanel);
            whosTurn = MoveState.PlayerTurn;
            applyMoveTiles(game);

            //Add Zombies
            zombieList = new List<Zombie>();
            int testRow = 5;
            int testColumn = 1;
            Tile Testloc = tileData.Find(x => x.row == testRow && x.column == testColumn);
            zombieList.Add(new Zombie(testRow, testColumn, Testloc, game));

            DiceList = new List<Dice>();
        }


        List<Tile> LoadTileData(string nameOfGame)
        {
            List<Tile> tileData = new List<Tile>();
            bool firstLine = true;
            using (var reader = new StreamReader(@"Content/TileData_" + nameOfGame + ".csv"))
            {
                while (!reader.EndOfStream)
                {
                    if (!firstLine)
                    {
                        string line = reader.ReadLine();
                        var vals = line.Split(',');
                        Tile newTile = new Tile(vals);
                        tileData.Add(newTile);

                    }
                    else
                    {
                        firstLine = false;
                        reader.ReadLine();
                    }

                }
            }

            return tileData;
        }

        void applyMoveTiles(Zombicide game)
        {
            Tile curLocation = tileData.Find(x => x.row == game.ActiveCharacter.PlayerTile.row && x.column == game.ActiveCharacter.PlayerTile.column);
            litTiles.Clear();
            if (curLocation.LeftSide == RoomSide.street || curLocation.LeftSide == RoomSide.room || curLocation.LeftSide == RoomSide.opendoor)
            { litTiles.Add(tileData.Find(x => x.row == curLocation.row && x.column == curLocation.column - 1)); }
            if (curLocation.RighSide == RoomSide.street || curLocation.RighSide == RoomSide.room || curLocation.RighSide == RoomSide.opendoor)
            { litTiles.Add(tileData.Find(x => x.row == curLocation.row && x.column == curLocation.column + 1)); }
            if (curLocation.TopSide == RoomSide.street || curLocation.TopSide == RoomSide.room || curLocation.TopSide == RoomSide.opendoor)
            { litTiles.Add(tileData.Find(x => x.row == curLocation.row - 1 && x.column == curLocation.column)); }
            if (curLocation.BottomSide == RoomSide.street || curLocation.BottomSide == RoomSide.room || curLocation.BottomSide == RoomSide.opendoor)
            { litTiles.Add(tileData.Find(x => x.row == curLocation.row + 1 && x.column == curLocation.column)); }
        }

        void breakDoor(Zombicide game, Tile T, string side)
        {
            if (side == "left")
            {
                T.LeftSide = RoomSide.opendoor;
                var neighborTile = tileData.Find(x => x.row == T.row && x.column == T.column - 1);
                neighborTile.RighSide = RoomSide.opendoor;

            }
            else if (side == "top")
            {
                T.TopSide = RoomSide.opendoor;
                var neighborTile = tileData.Find(x => x.row == T.row - 1 && x.column == T.column);
                neighborTile.BottomSide = RoomSide.opendoor;
            }
            applyMoveTiles(game);
            game.ActiveCharacter.movesLeft--;
            moves.Text = "Moves Left: " + (game.ActiveCharacter.movesLeft).ToString();
            if (game.ActiveCharacter.movesLeft == 0) { whosTurn = MoveState.ZombieTurn; }
        }

        public void addDice(Dice D)
        {
            DiceList.Add(D);
        }

        public void DicePopup()
        {
            Panel dicePanel = new Panel(new Vector2(400, 400), PanelSkin.Simple, Anchor.Center);
            var hitText = new Header("", Anchor.TopCenter);
            dicePanel.AddChild(hitText);
            int hits = 0;
            foreach (Dice D in DiceList)
            {
                dicePanel.AddChild(new Image(D.Texture, new Vector2(50, 50), ImageDrawMode.Stretch, Anchor.AutoInline));
                if (D.value >= game.ActiveCharacter.ActiveWeapon.DiceThreshold)
                { hits++; }
            }
            hitText.Text = hits.ToString() + " Hits";
            var okButton = new Button("OK", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            okButton.OnClick = (Entity btn) =>
            {
                UserInterface.Active.RemoveEntity(dicePanel);
                dicePanel.Dispose();
                DiceList.Clear();
            };
            dicePanel.AddChild(okButton);
            UserInterface.Active.AddEntity(dicePanel);
        }

        void MouseClicked(Zombicide game, int LR)
        {
            int x = game.MouseState.X;
            int y = game.MouseState.Y;

            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            if (LR == 1) //left Click
            {
                Rectangle resetMapRect = new Rectangle(resetMapX, resetMapY, 54, 54);
                if (mouseClickRect.Intersects(resetMapRect))
                {
                    mapWidth = 800;
                    mapHeight = 800;
                    mapX = (game.GraphicsDevice.Viewport.Width / 2) - (mapWidth / 2);
                    mapY = (game.GraphicsDevice.Viewport.Height / 2) - (mapHeight / 2);
                }

                if (whosTurn == MoveState.PlayerTurn)
                {



                    foreach (Tile T in doorTiles)
                    {
                        Rectangle leftdoorRect = new Rectangle(mapX + (T.column * tileWidth) - 25, mapY + (T.row * tileHeight) + (tileHeight / 2) - 25, 50, 50);
                        Rectangle topdoorRect = new Rectangle(mapX + (T.column * tileWidth) + (tileWidth / 2) - 25, mapY + (T.row * tileHeight) - 25, 50, 50);
                        if (mouseClickRect.Intersects(leftdoorRect) && T.LeftSide == RoomSide.closeddoor && Math.Abs(T.row - game.ActiveCharacter.PlayerTile.row) <= 1 && Math.Abs(T.column - game.ActiveCharacter.PlayerTile.column) <= 1)
                        {
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Locked Door", "Break Down Door?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {breakDoor(game, T,"left");
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    return true; }) });
                        }
                        else if (mouseClickRect.Intersects(topdoorRect) && T.TopSide == RoomSide.closeddoor && Math.Abs(T.row - game.ActiveCharacter.PlayerTile.row) <= 1 && Math.Abs(T.column - game.ActiveCharacter.PlayerTile.column) <= 1)
                        {
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Locked Door", "Break Down Door?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {breakDoor(game, T,"top");
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    return true; }) });
                        }
                    }
                    foreach (Tile T in litTiles)
                    {
                        Rectangle tileRect = new Rectangle(mapX + T.column * tileWidth, mapY + T.row * tileHeight, tileWidth, tileHeight);
                        if (mouseClickRect.Intersects(tileRect))
                        {
                            game.ActiveCharacter.PlayerTile.row = T.row;
                            game.ActiveCharacter.PlayerTile.column = T.column;
                            applyMoveTiles(game);
                            game.ActiveCharacter.movesLeft--;
                            moves.Text = "Moves Left: " + (game.ActiveCharacter.movesLeft).ToString();
                            if (game.ActiveCharacter.movesLeft == 0) { whosTurn = MoveState.ZombieTurn; }
                            break;
                        }
                    }

                }



            }

            if (LR == 2) //Right Click
            {
                foreach (Tile T in tileData)
                {
                    Rectangle tileRect = new Rectangle(mapX + T.column * tileWidth, mapY + T.row * tileHeight, tileWidth, tileHeight);
                    if (mouseClickRect.Intersects(tileRect))
                    {
                        List<Zombie> tileZombies = new List<Zombie>();
                        tileZombies = zombieList.FindAll(z => z.ZombieTile[0] == T.row && z.ZombieTile[1] == T.column);
                        if (tileZombies.Count > 0)
                        {
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Zombie", "Attack Zombie In This Tile?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {
                                    var hits=game.ActiveCharacter.Attack(game,tileZombies.First());
                                    for (int i =0;i< hits;i++)
                                    {
                                        if(tileZombies.Count > 0)
                                        {
                                            zombieList.RemoveAt(zombieList.FindIndex(z => z.ZombieTile[0] == T.row && z.ZombieTile[1] == T.column));
                                            tileZombies.RemoveAt(0);
                                            game.ActiveCharacter.ApplyExperience(1);
                                        }

                                    }
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    return true; }) });
                        }

                    }
                }
            }
        }

    }
}
