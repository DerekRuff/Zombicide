using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public interface IDrawableGameObject
    {
        Texture2D Texture { get; }
        Point Position { get; }
        Point Size { get; }
        void Update(Zombicide game);
        void Draw(Zombicide game);
    }
}
