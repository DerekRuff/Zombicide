﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZomCide.GamePlayers
{
    public class Zombie : IDrawableGameObject
    {
        
        public static Zombicide Game { get; set; }
        public static List<int[]> SpawnTiles { get; set; }
        public static List<Zombie> zombieList;
        public static Texture2D WalkerIcon;
        public static Texture2D RunnerIcon;
        public static Texture2D FattyIcon;

        public bool Moving { get; private set; }
        public int[] ZombieTile { get; set; } = new int[2];
        public int[] LastTile { get; set; } = new int[2];
        public Tile locationTile { get; set; }
        public Texture2D Texture { get; set; }
        public Point Size { get; set; }
        public Point Position { get; set; }
        public int animationOffset { get; private set; }
        protected Point NumberOffset { get; set; }
        protected Point PositionOffset { get; set; }
        protected int numOfMoves { get; set; }

        List<Tile> solutionPath;
        List<Tile> visitedTiles;
        List<List<Tile>> solutions;
        bool pFound;

        protected Zombie(int row, int column, Tile loc)
        {
            ZombieTile[0] = row;
            ZombieTile[1] = column;
            LastTile[0] = row;
            LastTile[1] = column;
            locationTile = loc;
            Size = new Point(40, 40);
            Position = new Point(MainGameScreen.mapX +PositionOffset.X+ ZombieTile[1] * MainGameScreen.tileWidth, MainGameScreen.mapY + PositionOffset.Y + ZombieTile[0] * MainGameScreen.tileHeight);
        }

        public static void Initialize(Zombicide game)
        {
            zombieList = new List<Zombie>();
            WalkerIcon = game.Content.Load<Texture2D>(@"ZombieIcons/Walker");
            RunnerIcon = game.Content.Load<Texture2D>(@"ZombieIcons/Runner");
            FattyIcon = game.Content.Load<Texture2D>(@"ZombieIcons/Fatty");
        }

        public void Move(List<Tile> tileData, Character active)
        {
            
            LastTile[0] = ZombieTile[0];
            LastTile[1] = ZombieTile[1];
            solutionPath = new List<Tile>();
            solutions = new List<List<Tile>>();
            visitedTiles = new List<Tile>();
            pFound = false;
            FindNextTile(tileData, active, ZombieTile[0], ZombieTile[1], ZombieTile[0], ZombieTile[1]);
            solutions.Sort((a, b) => a.Count.CompareTo(b.Count));
            if (solutions.First().Count > 2 && numOfMoves ==2)
            {
                ZombieTile[0] = solutions.First()[2].row;
                ZombieTile[1] = solutions.First()[2].column;
            }
            else if (solutions.First().Count > 1 && numOfMoves == 2)
            {
                ZombieTile[0] = solutions.First()[1].row;
                ZombieTile[1] = solutions.First()[1].column;
                attackPlayer();
            }
            else if (solutions.First().Count > 1)
            {
                ZombieTile[0] = solutions.First()[1].row;
                ZombieTile[1] = solutions.First()[1].column;
            }
            else
            {
                ZombieTile[0] = solutions.First()[0].row;
                ZombieTile[1] = solutions.First()[0].column;
            }
            Moving = true;
        }


        //This is a recursive function that will find a path to the player. There is a bug right now when enough 
        //doors are opened to allow multiple paths to the player.
        //This was modeled off what I found here https://www.cs.bu.edu/teaching/alg/maze/
        public void FindNextTile(List<Tile> tileData, Character active, int nRow, int nCol, int cRow, int cCol)
        {
            Tile Tile = tileData.Find(x => x.row == cRow && x.column == cCol); //this is the current tile in the iteration
            Tile nextTile = tileData.Find(x => x.row == nRow && x.column == nCol); //This is the next tile that is being looked at as a possible path
            if (nextTile == null || visitedTiles.Exists(x => x.row == nextTile.row && x.column == nextTile.column)) { return; } //next tile doesnt exist or zombie has already tried that path
            visitedTiles.Add(Tile);

            var dr = nextTile.row - Tile.row; //delta row
            var dc = nextTile.column - Tile.column; // delta column
            if (dr == 1 && dc == 0 && (Tile.BottomSide == RoomSide.closeddoor || Tile.BottomSide == RoomSide.wall)) { return; } //Next tile is below and not availible
            if (dr == 0 && dc == 1 && (Tile.RighSide == RoomSide.closeddoor || Tile.RighSide == RoomSide.wall)) { return; } //Next tile is right and not availible
            if (dr == -1 && dc == 0 && (Tile.TopSide == RoomSide.closeddoor || Tile.TopSide == RoomSide.wall)) { return; } //Next tile is above and not availible
            if (dr == 0 && dc == -1 && (Tile.LeftSide == RoomSide.closeddoor || Tile.LeftSide == RoomSide.wall)) { return; } //Next tile is left and not availible
            if (nextTile.row == active.PlayerTile.row && nextTile.column == active.PlayerTile.column)
            { //Player was found in next tile
              //pFound = true;
                solutionPath.Add(nextTile);
                solutions.Add(solutionPath);
                solutionPath = new List<Tile>(solutionPath);
                solutionPath.RemoveAt(solutionPath.Count - 1);
                return;
            }
            solutionPath.Add(nextTile); //temporarily adds tile being looked at until a dead end is found
            FindNextTile(tileData, active, nRow + 1, nCol, nRow, nCol); //look again below
            if (pFound) { return; }
            FindNextTile(tileData, active, nRow, nCol + 1, nRow, nCol); //look again right
            if (pFound) { return; }
            FindNextTile(tileData, active, nRow - 1, nCol, nRow, nCol); //look again above
            if (pFound) { return; }
            FindNextTile(tileData, active, nRow, nCol - 1, nRow, nCol); //look again left
            if (!pFound)
            {
                solutionPath.RemoveAt(solutionPath.Count - 1);
                visitedTiles.RemoveAt(visitedTiles.Count - 1);
                return;
            } //Removes all tiles in branch that hit dead end


        }

        public void attackPlayer()
        {
            GeonBit.UI.Utils.MessageBox.ShowMsgBox("lol", "Gotcha Bitch", new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                new GeonBit.UI.Utils.MessageBox.MsgBoxOption("ok", () => {
                                    return true;
                                })
            });
            Game.ActiveCharacter.ApplyDamage(1);
            var mainScreen = (MainGameScreen)Game.CurrentScreen;
            mainScreen.life.Text = "Life: " + (Game.ActiveCharacter.DeathThreshold - Game.ActiveCharacter.GetDamageTaken()).ToString();
        }

        public void Update(Zombicide game)
        {
            if (Moving == true)
            {
                if (!new Rectangle(new Point(MainGameScreen.mapX+PositionOffset.X + ZombieTile[1] * MainGameScreen.tileWidth, MainGameScreen.mapY + PositionOffset.Y + ZombieTile[0] * MainGameScreen.tileHeight), new Point(15, 15)).Contains(Position))
                {
                    animationOffset++;
                    int Ydir = ZombieTile[0] - LastTile[0];
                    int Xdir = ZombieTile[1] - LastTile[1];
                    Position = new Point((MainGameScreen.mapX +PositionOffset.X+ LastTile[1] * MainGameScreen.tileWidth) + (animationOffset * Xdir), (MainGameScreen.mapY + PositionOffset.Y + LastTile[0] * MainGameScreen.tileHeight) + (animationOffset * Ydir));
                }
                else { Moving = false; animationOffset = 0; }

            }
            else { Position = new Point(MainGameScreen.mapX + PositionOffset.X + ZombieTile[1] * MainGameScreen.tileWidth, MainGameScreen.mapY + PositionOffset.Y + ZombieTile[0] * MainGameScreen.tileHeight); }
        }

        public virtual void Draw(Zombicide game)
        {

            game.SpriteBatch.Draw(Texture, new Rectangle(Position, Size), Color.White);
            //game.SpriteBatch.DrawString(MainGameScreen.Impact, zombieList.Where(x => x.ZombieTile[0] == ZombieTile[0] && x.ZombieTile[1] == ZombieTile[1]).Count().ToString(), new Vector2(Position.X + NumberOffset.X, Position.Y + NumberOffset.Y), Color.Black);
        }

        

        public static bool CheckMoving()
        {
            return zombieList.Any(x => x.Moving == true);
        }


    }
}
