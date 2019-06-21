using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ZomCide.GamePlayers
{
    public class Fatty : Zombie
    {
        public Fatty(int row, int column, Tile loc) : base(row, column, loc)
        {
            Texture = FattyIcon;
            NumberOffset = new Point(25, 3);
            PositionOffset = new Point(MainGameScreen.tileWidth / 3,10);
            numOfMoves = 1;
        }

        public override void Draw(Zombicide game)
        {
            base.Draw(game);
            game.SpriteBatch.DrawString(MainGameScreen.Impact, zombieList.Where(x => x.ZombieTile[0] == ZombieTile[0] && x.ZombieTile[1] == ZombieTile[1]&& x is Fatty).Count().ToString(), new Vector2(Position.X + NumberOffset.X, Position.Y + NumberOffset.Y), Color.Black);
        }
    }
}
