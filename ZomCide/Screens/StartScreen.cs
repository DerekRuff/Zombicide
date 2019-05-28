using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public class StartScreen : GameScreen
    {
        private SpriteFont titleFont;
        private Texture2D newGameButton;
        private Vector2 textMiddlePoint;
        private Vector2 titlePosition;
        private Rectangle newGameButtonRectangle;

        public StartScreen() {  }
        public StartScreen(Zombicide game)
        {
            LoadContent(game);
        }

        public override void LoadContent(Zombicide game)
        {
            titleFont = game.Content.Load<SpriteFont>("Zombicide Font");
            newGameButton = game.Content.Load<Texture2D>(@"New Game");
            textMiddlePoint = titleFont.MeasureString("Zombicide") / 2;
            titlePosition = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            newGameButtonRectangle = new Rectangle((int)((game.GraphicsDevice.Viewport.Width / 2) - (Zombicide.NGButtonWidth / 2)), (int)(game.GraphicsDevice.Viewport.Height - (game.GraphicsDevice.Viewport.Height / 5)), (int)Zombicide.NGButtonWidth, (int)Zombicide.NGButtonHeight);
        }

        public override void Update(Zombicide game)
        {
            if(game.MouseState.LeftButton == ButtonState.Pressed
                && newGameButtonRectangle.Contains(game.MouseState.Position))
            {
                game.SetNextScreen(nameof(CharacterSelectScreen));
            }
        }

        public override void Draw(Zombicide game)
        {
            game.SpriteBatch.DrawString(titleFont, "Zombicide", titlePosition, Color.Black, 0, textMiddlePoint, 2f, SpriteEffects.None, 0.5f);
            game.SpriteBatch.Draw(newGameButton, newGameButtonRectangle, Color.White);
        }
    }
}
