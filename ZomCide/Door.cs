using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZomCide
{
    class Door : IGameObject
    {
        public Texture2D Texture => throw new NotImplementedException();

        public Vector2 Position => throw new NotImplementedException();

        public void Click(Zombicide game, bool RightClick)
        {
            throw new NotImplementedException();
        }

        public void Draw(ZomCideGame game, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Update(ZomCideGame game, KeyboardState keyboardState, MouseState mouseState)
        {
            throw new NotImplementedException();
        }
    }
}
