using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ZomCide.GamePlayers
{
    public class Walker : Zombie
    {
        public Walker(int row, int column, Tile loc) : base(row, column, loc)
        {
            Texture = WalkerIcon;
            NumberOffset = new Point(5, 8);
            PositionOffset = new Point(0, 10);
            numOfMoves = 1;
        }

        public override void Draw(Zombicide game)
        {
            base.Draw(game);
            game.SpriteBatch.DrawString(MainGameScreen.Impact, zombieList.Where(x => x.ZombieTile[0] == ZombieTile[0] && x.ZombieTile[1] == ZombieTile[1] && x is Walker).Count().ToString(), new Vector2(Position.X + NumberOffset.X, Position.Y + NumberOffset.Y), Color.Black);
        }
    }
}
