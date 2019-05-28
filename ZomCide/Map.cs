using Microsoft.Xna.Framework;
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
    //public class Map : IDrawableGameObject
    //{
    //    public TmxMap map { get; set; }
    //    public Texture2D MapPic { get; set; }
    //    public int MapWidth { get; set; } = 800;
    //    public int MapHeight { get; set; } = 800;
    //    public int MapX { get; set; }
    //    public int MapY { get; set; }
    //    public int MapSpeed { get; set; }
    //    public int TileWidth { get; set; }
    //    public int TileHeight { get; set; }
    //    public int TilesHigh { get; set; }
    //    public int TilesWide { get; set; }
    //    public List<Tile> TileData { get; set; }
    //    public List<Tile> LitTiles { get; set; }
    //    public List<Tile> DoorTiles { get; set; }
    //    public int ResetMapX { get; set; }
    //    public int ResetMapY { get; set; }
    //    public int WindowWidth { get; set; }
    //    public int WindowHeight { get; set; }

    //    public Texture2D Texture { get; set; }
    //    public Vector2 Position { get; set; }

    //    private MouseState previousMouseState;

    //    public Map()
    //    {
    //        TileData = new List<Tile>();
    //        LitTiles = new List<Tile>();
    //        DoorTiles = new List<Tile>();
    //    }

    //    void LoadTileData(string nameOfGame)
    //    {
    //        bool firstLine = true;
    //        using (var reader = new StreamReader(@"Content/TileData_" + nameOfGame + ".csv"))
    //        {
    //            while (!reader.EndOfStream)
    //            {
    //                if (!firstLine)
    //                {
    //                    string line = reader.ReadLine();
    //                    var vals = line.Split(',');
    //                    Tile newTile = new Tile(vals);
    //                    TileData.Add(newTile);

    //                }
    //                else
    //                {
    //                    firstLine = false;
    //                    reader.ReadLine();
    //                }

    //            }
    //        }
    //    }
        

    //    public void applyMoveTiles(int cRow, int cColumn)
    //    {
    //        Tile curLocation = TileData.Find(x => x.row == cRow && x.column == cColumn);
    //        LitTiles.Clear();
    //        Tile.Sides[] sides = new Tile.Sides[4] { curLocation.LeftSide, curLocation.RighSide, curLocation.TopSide, curLocation.BottomSide };
    //        for(int i = 0; i < sides.Length; i++)
    //        {
    //            int moveRow = curLocation.row;
    //            int moveColumn = curLocation.column;
    //            if(IsValidMove(sides[i]))
    //            {
    //                switch(i)
    //                {
    //                    case 0:
    //                        moveColumn -= 1;
    //                        break;
    //                    case 1:
    //                        moveColumn += 1;
    //                        break;
    //                    case 2:
    //                        moveRow -= 1;
    //                        break;
    //                    case 3:
    //                        moveRow += 1;
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                LitTiles.Add(TileData.Find(x => x.row == moveRow && x.column == moveColumn));
    //            }
    //        }
    //    }

    //    private bool IsValidMove(Tile.Sides targetTile)
    //    {
    //        switch(targetTile)
    //        {
    //            case RoomSide.street:
    //            case RoomSide.opendoor:
    //            case RoomSide.room:
    //                return true;
    //            default:
    //                return false;
    //        }
    //    }


    //    public void ZoomMap(int val)
    //    {
    //        int zoomVal = 30;
    //        if (MapWidth >= 200 && val < 0)
    //        {
    //            MapWidth -= zoomVal;
    //            MapHeight -= zoomVal;
    //            MapX = MapX + (zoomVal / 2);
    //            MapY = MapY + (zoomVal / 2);

    //        }
    //        if (MapWidth <= 2000 && val >= 0)
    //        {
    //            MapWidth += zoomVal;
    //            MapHeight += zoomVal;
    //            MapX = MapX - (zoomVal / 2);
    //            MapY = MapY - (zoomVal / 2);
    //        }
    //    }

    //    public void Update(ZomCideGame game, KeyboardState keyboardState, MouseState mouseState)
    //    {
    //        if (game.State == GameState.Playing)
    //        {
    //            if (mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue)
    //            {
    //                ZoomMap(mouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue);
    //            }
    //            int mapCenterX = MapX + (MapWidth / 2);
    //            int mapCenterY = MapY + (MapHeight / 2);

    //            if (keyboardState.IsKeyDown(Keys.Up) && mapCenterY > 0) { MapY -= MapSpeed; }
    //            if (keyboardState.IsKeyDown(Keys.Left) && mapCenterX > 400) { MapX -= MapSpeed; }
    //            if (keyboardState.IsKeyDown(Keys.Right) && mapCenterX < WindowWidth - 400) { MapX += MapSpeed; }
    //            if (keyboardState.IsKeyDown(Keys.Down) && mapCenterY < WindowHeight) { MapY += MapSpeed; }
    //            TileHeight = MapHeight / TilesHigh;
    //            TileWidth = MapWidth / TilesWide;

    //            if (game.WhosTurn == MoveState.PlayerTurn)
    //            {

    //                applyMoveTiles(game.ActiveCharacter.PlayerTile.row, game.ActiveCharacter.PlayerTile.column);

    //            }
    //            previousTurnState = whosTurn;
    //            if (whosTurn == MoveState.ZombieTurn)
    //            {
    //                zombieList[0].move(map, ActiveCharacter);
    //                whosTurn = MoveState.PlayerTurn;
    //                ActiveCharacter.resetMoves(moves);
    //            }

    //        }

    //        previousMouseState = mouseState;
    //    }

    //    public void Draw(ZomCideGame game, SpriteBatch spriteBatch)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
