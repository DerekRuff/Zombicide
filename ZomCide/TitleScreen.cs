using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ZomCide
{
    class TitleScreen : DrawableGameComponent
    {
        SpriteBatch _batch;

        SpriteFont TitleFont;
        Texture2D NewGameButtonTexture;
        Texture2D background;

        Rectangle backgroundPosition;
        Vector2 titleMiddlePoint;
        Vector2 titlePosition;
        Vector2 newGamePosition;

        private const float NGButtonWidth = 172f;
        private const float NGButtonHeight = 35f;

        public TitleScreen(Game game) : base(game)
        {
            TitleFont = game.Content.Load<SpriteFont>("Zombicide Font");
            NewGameButtonTexture = game.Content.Load<Texture2D>(@"New Game");
            background = game.Content.Load<Texture2D>(@"Zombicide Background");

            titleMiddlePoint = TitleFont.MeasureString("Zombicide") / 2;
            titlePosition = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            backgroundPosition = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width,game.GraphicsDevice.Viewport.Height);
            newGamePosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2) - (NGButtonWidth / 2), (game.GraphicsDevice.Viewport.Height - (game.GraphicsDevice.Viewport.Height / 5)));
            _batch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();
            _batch.Draw(background, backgroundPosition, Color.White);
            _batch.Draw(NewGameButtonTexture, newGamePosition, Color.White);
            _batch.DrawString(TitleFont, "Zombicide", titlePosition, Color.Black, 0, titleMiddlePoint, 2f, SpriteEffects.None, 0.5f);
            _batch.End();

            base.Draw(gameTime);

        }
    }
}
