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
    public class GameScreen : IDrawableGameObject
    {
        
        public Texture2D Texture { get; private set; }
        public Point Position { get; private set; }
        public Point Size { get; private set; }

        public virtual void LoadContent(Zombicide game)
        {

        }
      
        public virtual void Draw(Zombicide game)
        {
           
        }

        public virtual void Update(Zombicide game)
        {
            
        }
    }
}
