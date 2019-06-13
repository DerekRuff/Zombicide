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
using TiledSharp;

namespace ZomCide
{
    class MainGameScreen : GameScreen
    {
        Zombicide game;
        public static Random RNG;
        MoveState whosTurn;
        private Texture2D resetMapButton;
        private Texture2D Highlight;
        private Texture2D closedDoor;
        private Texture2D openDoor;
        public static SpriteFont Impact;

        TmxMap map;
        Texture2D mapPic;
        int mapWidth = 800;
        int mapHeight = 800;
        public static int mapX;
        public static int mapY;
        public static int tileWidth;
        public static int tileHeight;
        public static int tilesHigh;
        public static int tilesWide;

        public static List<Tile> tileData;
        public static List<Tile> litTiles;
        public static List<Tile> doorTiles;

        int resetMapX;
        int resetMapY;

        public Paragraph life { get; set; }
        public Paragraph moves { get; set; }
        public Paragraph level { get; set; }
        public Paragraph experience { get; set; }
        public Button forfeitMove { get; set; }
        public Image MainHand { get; set; }
        public Image OffHand { get; set; }
        public Image ArmorSlot { get; set; }
        public Paragraph BlueSkill { get; set; }
        public Paragraph YellowSkill { get; set; }
        public Paragraph OrangeSkill1 { get; set; }
        public Paragraph OrangeSkill2 { get; set; }
        public Paragraph RedSkill1 { get; set; }
        public Paragraph RedSkill2 { get; set; }
        public Paragraph RedSkill3 { get; set; }

        public bool EndGameFlag { get; set; }
        public bool PopupFlag { get; set; }
        public bool LastPopupFlag { get; set; }


        public MainGameScreen(Zombicide game)
        {
            LoadMainGame(game);
            PopupFlag = false;
            PopupFlag = false;
            this.game = game;
            Impact = game.Content.Load<SpriteFont>("Impact");
        }

        public override void Update(Zombicide game)
        {
            int mapSpeed = 10;
            game.ActiveCharacter.Update(game);
            bool moving = Zombie.CheckMoving() || game.ActiveCharacter.moving;

            //Update UI components
            life.Text = ("Life: " + (game.ActiveCharacter.DeathThreshold - game.ActiveCharacter.GetDamageTaken()).ToString());
            moves.Text = ("Moves Left: " + (game.ActiveCharacter.movesLeft).ToString());
            level.Text = ("Level: " + (game.ActiveCharacter.Level).ToString());
            experience.Text = ("Experience: " + (game.ActiveCharacter.Experience).ToString());
            var mainWeap = (Weapon)game.ActiveCharacter.MainHandSlot;
            MainHand.ToolTipText = "Damage: " + mainWeap.Damage + "\nDice: " + mainWeap.Dice + "\nHit Value: " + mainWeap.DiceThreshold + "\nRange: " + mainWeap.MinRange + "-" + mainWeap.MaxRange;
            MainHand.OutlineColor = (mainWeap.Active) ? Color.Red : Color.White;
            var OffHandName = (game.ActiveCharacter.OffHandSlot != null) ? game.ActiveCharacter.OffHandSlot.Name : "None";
            if (game.ActiveCharacter.OffHandSlot != null)
            {
                OffHand.OutlineColor = (((Weapon)game.ActiveCharacter.OffHandSlot).Active) ? Color.Red : Color.White;
            }
            else { OffHand.FillColor = Color.White; }

            //Check endgame conditions
            if (EndGameFlag == false)
            {
                if (game.ActiveCharacter.IsAlive == false)
                {
                    EndGamePopup(false);
                }
                else if (Objective.ObjectiveList.Exists(x => x.flipped == true && x.UndersideColor == "blue"))
                {
                    EndGamePopup(true);
                }
            }


            if (PopupFlag == false && LastPopupFlag == false && moving == false)
            {
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
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && whosTurn == MoveState.PlayerTurn) { SearchPopup(); }

            }
            LastPopupFlag = PopupFlag;

            int mapCenterX = mapX + (mapWidth / 2);
            int mapCenterY = mapY + (mapHeight / 2);

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && mapCenterY > 0) { mapY -= mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && mapCenterX > 400) { mapX -= mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && mapCenterX < game.GraphicsDevice.Viewport.Width - 400) { mapX += mapSpeed; }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && mapCenterY < game.GraphicsDevice.Viewport.Height) { mapY += mapSpeed; }
            tileHeight = mapHeight / tilesHigh;
            tileWidth = mapWidth / tilesWide;

            foreach (Zombie Z in Zombie.zombieList)
            {
                Z.Update(game);
            }


            if (whosTurn == MoveState.PlayerTurn && moving == false)
            {
                applyMoveTiles(game);
                if (game.ActiveCharacter.movesLeft <= 0) { whosTurn = MoveState.ZombieTurn; }
            }

            if (whosTurn == MoveState.ZombieTurn)
            {
                //Give zombies their move
                foreach (Zombie Z in Zombie.zombieList)
                {
                    if (game.ActiveCharacter.PlayerTile.row == Z.ZombieTile[0] && game.ActiveCharacter.PlayerTile.column == Z.ZombieTile[1])
                    { Z.attackPlayer(); }
                    else { Z.move(tileData, game.ActiveCharacter); }

                }
                //Spawn new Zombies
                foreach (var ST in Zombie.SpawnTiles)
                {
                    if (RNG.Next(0, 2) == 1)
                    {
                        Zombie.AddZombie(ST[0], ST[1]);
                    }
                }

                whosTurn = MoveState.PlayerTurn;
                game.ActiveCharacter.resetMoves(moves);
            }
        }

        private void SearchPopup()
        {
            PopupFlag = true;
            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Search?", "Do you want to search this tile for one action?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {
                                    List<Item> items = game.ActiveCharacter.Search();
                                    if (items!= null)
                                    {
                                        FoundItemsPopup(true, items);
                                    }
                                    else { FoundItemsPopup(false, items); }
                                    PopupFlag = false;
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    PopupFlag = false;
                                    return true;
                                }) });
        }

        private void FoundItemsPopup(bool found, List<Item> items)
        {
            PopupFlag = true;
            Panel itemsPanel = new Panel(new Vector2(400, 450), PanelSkin.Simple, Anchor.Center);
            var hitText = new Header("", Anchor.TopCenter);
            var okButton = new Button("OK", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));

            if (found)
            {
                hitText.Text = "Found " + items.Count + " Items";
                
                foreach (Item I in items)
                {
                    itemsPanel.AddChild(new Image(I.Texture, new Vector2(I.Size.X, I.Size.Y), ImageDrawMode.Stretch, Anchor.AutoInline,new Vector2(0,50)));
                }
                
                okButton.OnClick = (Entity btn) =>
                {
                    PopupFlag = false;
                    LastPopupFlag = true;
                    UserInterface.Active.RemoveEntity(itemsPanel);
                    itemsPanel.Dispose();
                };
            }
            else
            {
                hitText.Text = "Cant Search Here";
                okButton.OnClick = (Entity btn) =>
                {
                    PopupFlag = false;
                    LastPopupFlag = true;
                    UserInterface.Active.RemoveEntity(itemsPanel);
                    itemsPanel.Dispose();
                };
            }
            itemsPanel.AddChild(hitText);
            itemsPanel.AddChild(okButton);
            UserInterface.Active.AddEntity(itemsPanel);

        }

        public override void Draw(Zombicide game)
        {
            game.SpriteBatch.Draw(mapPic, new Rectangle(mapX, mapY, mapWidth, mapHeight), Color.White);
            foreach (Objective O in Objective.ObjectiveList)
            {
                O.Draw(game);
            }
            game.ActiveCharacter.Draw(game);
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
            foreach (Zombie Z in Zombie.zombieList)
            {
                Z.Draw(game);
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

            //Initialize random number generator
            RNG = new Random();

            //Place reset map Button
            resetMapButton = game.Content.Load<Texture2D>(@"CenterMapButton");
            resetMapX = 1200 - 54;
            resetMapY = game.GraphicsDevice.Viewport.Height - 54;

            //read in Start tile from map data and place player there
            var startTile = map.Layers[0].Properties.Where(x => x.Key == "StartTile").FirstOrDefault().Value.Split(',');
            game.ActiveCharacter.Move(startTile, true);

            //Load Zombie Spawns
            Zombie.SpawnTiles = map.Layers[0].Properties.Where(x => x.Key.StartsWith("SpawnZone")).Select(x => x.Value.Split(',').Select(y => Convert.ToInt32(y)).ToArray()).ToList();

            //Load and initialize objectives
            Objective.ObjectiveTiles = map.Layers[0].Properties.Where(x => x.Key.StartsWith("Objective")).Select(x => x.Value.Split(',').Select(y => Convert.ToInt32(y)).ToArray()).ToList();
            Objective.Initialize(game);

            //Initialize UI for Main game screen
            PanelTabs tabs = new PanelTabs();
            PanelTabs RightTabs = new PanelTabs();
            Panel movePanel = new Panel(new Vector2(400, 750), PanelSkin.Default, Anchor.TopRight, new Vector2(0, 50));
            Panel playerPanel = new Panel(new Vector2(400, 750), PanelSkin.Default, Anchor.TopLeft, new Vector2(0, 50));
            playerPanel.AddChild(tabs);
            movePanel.AddChild(RightTabs);

            TabData EquipmentTab = RightTabs.AddTab("Equipment");
            Item MH = game.ActiveCharacter.MainHandSlot;
            MainHand = new Image(MH.Texture, new Vector2(MH.Size.X, MH.Size.Y), anchor: Anchor.TopLeft);
            MainHand.OutlineWidth = 3;
            var mainWeap = (Weapon)game.ActiveCharacter.MainHandSlot;
            MainHand.ToolTipText = "Damage: " + mainWeap.Damage + "\nDice: " + mainWeap.Dice + "\nHit Value: " + mainWeap.DiceThreshold + "\nRange: " + mainWeap.MinRange + "-" + mainWeap.MaxRange;
            MainHand.OnClick = (Entity pgh) =>
            {
                game.ActiveCharacter.SetActiveWeapon((Weapon)game.ActiveCharacter.MainHandSlot);
            };
            OffHand = new Image(Character.EmptyHand, new Vector2(MH.Size.X, MH.Size.Y), anchor: Anchor.TopRight);
            OffHand.OutlineWidth = 3;
            OffHand.OutlineColor = Color.White;
            OffHand.OnClick = (Entity pgh) =>
            {
                if (game.ActiveCharacter.OffHandSlot != null)
                {
                    game.ActiveCharacter.SetActiveWeapon((Weapon)game.ActiveCharacter.OffHandSlot);
                }
            };
            ArmorSlot = new Image(Character.EmptyArmor, new Vector2(MH.Size.X, MH.Size.Y), anchor: Anchor.AutoCenter, offset: new Vector2(0, 50));
            ArmorSlot.OutlineWidth = 3;
            ArmorSlot.OutlineColor = Color.White;
            EquipmentTab.button.Padding = new Vector2(0, 0);
            EquipmentTab.button.Size = new Vector2(100, 50);
            EquipmentTab.button.Offset = new Vector2(0, -50);
            EquipmentTab.panel.Offset = new Vector2(0, -50);
            EquipmentTab.button.ButtonParagraph.Scale = .9f;
            EquipmentTab.panel.AddChild(MainHand);
            EquipmentTab.panel.AddChild(OffHand);
            EquipmentTab.panel.AddChild(ArmorSlot);

            TabData SkillTab = RightTabs.AddTab("Skills");
            BlueSkill = new Paragraph(game.ActiveCharacter.BlueSkill.SkillName, Anchor.Auto, Color.DeepSkyBlue);
            YellowSkill = new Paragraph(game.ActiveCharacter.YellowSkill.SkillName, Anchor.Auto, Color.Yellow);
            OrangeSkill1 = new Paragraph(game.ActiveCharacter.OrangeSkills.First().SkillName, Anchor.Auto, Color.Orange);
            OrangeSkill2 = new Paragraph(game.ActiveCharacter.OrangeSkills.Last().SkillName, Anchor.Auto, Color.Orange);
            RedSkill1 = new Paragraph(game.ActiveCharacter.RedSkills.First().SkillName, Anchor.Auto, Color.Red);
            RedSkill2 = new Paragraph(game.ActiveCharacter.RedSkills.ElementAt(1).SkillName, Anchor.Auto, Color.Red);
            RedSkill3 = new Paragraph(game.ActiveCharacter.RedSkills.Last().SkillName, Anchor.Auto, Color.Red);
            SkillTab.panel.AddChild(BlueSkill);
            SkillTab.panel.AddChild(YellowSkill);
            SkillTab.panel.AddChild(OrangeSkill1);
            SkillTab.panel.AddChild(OrangeSkill2);
            SkillTab.panel.AddChild(RedSkill1);
            SkillTab.panel.AddChild(RedSkill2);
            SkillTab.panel.AddChild(RedSkill3);
            SkillTab.button.Padding = new Vector2(0, 0);
            SkillTab.button.Size = new Vector2(100, 50);
            SkillTab.panel.Offset = new Vector2(0, -50);
            SkillTab.button.ButtonParagraph.Scale = .9f;



            TabData tab1 = tabs.AddTab("Player");
            life = new Paragraph("Life: " + (game.ActiveCharacter.DeathThreshold - game.ActiveCharacter.GetDamageTaken()).ToString());
            moves = new Paragraph("Moves Left: " + (game.ActiveCharacter.movesLeft).ToString());
            level = new Paragraph("Level: " + (game.ActiveCharacter.Level).ToString());
            experience = new Paragraph("Experience: " + (game.ActiveCharacter.Experience).ToString());
            forfeitMove = new Button("Forfeit Move", anchor: Anchor.BottomCenter);
            forfeitMove.OnClick = (Entity btn) =>
            {
                game.ActiveCharacter.movesLeft--;
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
            tab1.panel.AddChild(forfeitMove);
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

            //Initialize Dice and Zombies
            Zombie.Initialize(game);
            Dice.Initialize(game);
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
            if (game.ActiveCharacter.movesLeft == 0) { whosTurn = MoveState.ZombieTurn; }
        }



        public void DicePopup()
        {
            PopupFlag = true;

            Panel dicePanel = new Panel(new Vector2(400, 400), PanelSkin.Simple, Anchor.Center);
            var hitText = new Header("", Anchor.TopCenter);
            dicePanel.AddChild(hitText);
            int hits = 0;
            foreach (Dice D in Dice.DiceList)
            {
                dicePanel.AddChild(new Image(D.Texture, new Vector2(D.Size.X, D.Size.Y), ImageDrawMode.Stretch, Anchor.AutoInline));
                if (D.value >= game.ActiveCharacter.ActiveWeapon.DiceThreshold)
                { hits++; }
            }
            hitText.Text = hits.ToString() + " Hits";
            var okButton = new Button("OK", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            okButton.OnClick = (Entity btn) =>
            {
                PopupFlag = false;
                LastPopupFlag = true;
                Dice.Clear();
                UserInterface.Active.RemoveEntity(dicePanel);
                dicePanel.Dispose();


            };
            dicePanel.AddChild(okButton);
            UserInterface.Active.AddEntity(dicePanel);
        }

        public void EndGamePopup(bool win)
        {
            EndGameFlag = true;
            Panel endPanel = new Panel(new Vector2(400, 400), PanelSkin.Simple, Anchor.Center);
            string result = (win) ? "Win!" : "Lose";
            endPanel.AddChild(new Header("You " + result, Anchor.TopCenter));
            var okButton = new Button("OK", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            okButton.OnClick = (Entity btn) =>
            {
                game.Reset(this);
            };
            endPanel.AddChild(okButton);
            UserInterface.Active.AddEntity(endPanel);
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
                            PopupFlag = true;
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Locked Door", "Break Down Door?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {breakDoor(game, T,"left");
                                    PopupFlag = false;
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    PopupFlag = false;
                                    return true;
                                }) });
                            return;
                        }
                        else if (mouseClickRect.Intersects(topdoorRect) && T.TopSide == RoomSide.closeddoor && Math.Abs(T.row - game.ActiveCharacter.PlayerTile.row) <= 1 && Math.Abs(T.column - game.ActiveCharacter.PlayerTile.column) <= 1)
                        {
                            PopupFlag = true;
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Locked Door", "Break Down Door?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {breakDoor(game, T,"top");
                                    PopupFlag = false;
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    PopupFlag = false;
                                    return true;
                                }) });
                            return;
                        }
                    }
                    foreach (Objective O in Objective.ObjectiveList)
                    {
                        if (mouseClickRect.Intersects(O.Area) && O.flipped == false && game.ActiveCharacter.PlayerTile.row == O.Tile[0] && game.ActiveCharacter.PlayerTile.column == O.Tile[1])
                        {
                            O.FlipCard(game);
                            return;
                        }
                    }
                    foreach (Tile T in litTiles)
                    {
                        Rectangle tileRect = new Rectangle(mapX + T.column * tileWidth, mapY + T.row * tileHeight, tileWidth, tileHeight);
                        if (mouseClickRect.Intersects(tileRect))
                        {
                            game.ActiveCharacter.Move(new string[] { T.row.ToString(), T.column.ToString() });
                            applyMoveTiles(game);
                            if (game.ActiveCharacter.movesLeft == 0) { whosTurn = MoveState.ZombieTurn; }
                            return;
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
                        tileZombies = Zombie.zombieList.FindAll(z => z.ZombieTile[0] == T.row && z.ZombieTile[1] == T.column);
                        if (tileZombies.Count > 0)
                        {
                            PopupFlag = true;
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Zombie", "Attack Zombie In This Tile?", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("Yes", () => {
                                    var hits=game.ActiveCharacter.Attack(game,tileZombies.First());
                                    for (int i =0;i< hits;i++)
                                    {
                                        if(tileZombies.Count > 0)
                                        {
                                            Zombie.zombieList.RemoveAt(Zombie.zombieList.FindIndex(z => z.ZombieTile[0] == T.row && z.ZombieTile[1] == T.column));
                                            tileZombies.RemoveAt(0);
                                            game.ActiveCharacter.ApplyExperience(1);
                                        }

                                    }
                                    PopupFlag = false;
                                    return true;
                                }),
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("no", () => {
                                    PopupFlag = false;
                                    return true; }) });
                        }

                    }
                }
            }
        }

    }
}
