using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace ZomCide
{
    class Zombie
    {
        public static Texture2D Icon { get; set; }
        public int ZombieX { get; set; }
        public int ZombieY { get; set; }
        public int[] ZombieTile { get; set; } = new int[2];
        public Tile locationTile { get; set; }
        List<Tile> solutionPath;
        List<Tile> visitedTiles;
        List<List<Tile>> solutions;
        bool pFound;
        Zombicide Game;

        public Zombie(int row, int column, Tile loc, Zombicide game)
        {
            ZombieTile[0] = row;
            ZombieTile[1] = column;
            locationTile = loc;
            Game = game;
        }

        public void move(List<Tile> tileData, Character active)
        {
            solutionPath = new List<Tile>();
            solutions = new List<List<Tile>>();
            visitedTiles = new List<Tile>();
            pFound = false;
            FindNextTile(tileData, active, ZombieTile[0], ZombieTile[1], ZombieTile[0], ZombieTile[1]);
            solutions.Sort((a, b) => a.Count.CompareTo(b.Count));
            if (solutions.First().Count > 1)
            {
                ZombieTile[0] = solutions.First()[1].row;
                ZombieTile[1] = solutions.First()[1].column;
            }
            else
            {
                ZombieTile[0] = solutions.First()[0].row;
                ZombieTile[1] = solutions.First()[0].column;
            }
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
    }
}
