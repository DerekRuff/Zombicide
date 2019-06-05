using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZomCide
{
    public class Dice : IDrawableGameObject
    {

        Zombicide Game;
        MainGameScreen gameScreen;
        public int value;

        public Texture2D Texture { get; set; }

        public Vector2 Position { get; set; }

        public bool rolling;

        public Dice(Zombicide game)
        {
            Game = game;
            gameScreen = (MainGameScreen)game.CurrentScreen;
            value = 1;
            Texture = game.Content.Load<Texture2D>(@"Dice/die_face_1_T");
            rolling = false;

        }

        public void Update(Zombicide game)
        {
            throw new NotImplementedException();
        }

        public void Draw(Zombicide game)
        {
            throw new NotImplementedException();
        }

        public void Roll()
        {
            
            gameScreen.addDice(this);
            
            rolling = true;
            value = MainGameScreen.RNG.Next(1, 7);
            Texture = Game.Content.Load<Texture2D>(@"Dice/die_face_"+value.ToString()+"_T");
        }

        
    }
}
