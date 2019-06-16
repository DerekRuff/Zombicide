using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using GeonBit.UI;

namespace ZomCide
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Zombicide : Game
    {
        GraphicsDeviceManager graphics;
        Texture2D defaultBackground;
        Texture2D mouseCursor;
        Point mousePointerSize = new Point(40, 40);
        public SpriteBatch @SpriteBatch;
        private List<IDrawableGameObject> drawList = new List<IDrawableGameObject>();
        public GameState State { get; set; }
        public MoveState WhosTurn { get; set; }
        //public Character ActiveCharacter { get; set; }
        public KeyboardState @KeyboardState { get; private set; }
        public MouseState @MouseState { get; private set; }
        public MouseState PreviousMouseState { get; private set; }
        public GameScreen CurrentScreen { get; private set; }
        public const float NGButtonWidth = 172f;
        public const float NGButtonHeight = 35f;
        public GameTime @GameTime { get; private set; }
        public Character ActiveCharacter { get; set; }

        public List<IDrawableGameObject> DrawList
        {
            get
            {
                return drawList;
            }
        }

        public void AddToDrawList(IDrawableGameObject visibleObject)
        {
            drawList.Add(visibleObject);
        }

        public Zombicide()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1600;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            IsMouseVisible = false;
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            drawList = new List<IDrawableGameObject>();
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentScreen = new StartScreen(this);
            mouseCursor = Content.Load<Texture2D>("GeonBit.UI/themes/hd/textures/cursor_default");
            defaultBackground = Content.Load<Texture2D>(@"Zombicide Background");
            Zombie.Icon = Content.Load<Texture2D>(@"ZombieIcon");
            Zombie.Game = this;

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            PreviousMouseState = MouseState;
            MouseState = Mouse.GetState();
            drawList.Clear();
            drawList.Add(CurrentScreen);
            foreach(IDrawableGameObject gameObject in drawList)
            {
                gameObject.Update(this);
            }
            UserInterface.Active.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();
            SpriteBatch.Draw(defaultBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            foreach (IDrawableGameObject gameObject in drawList)
            {
                gameObject.Draw(this);
            }
            SpriteBatch.Draw(mouseCursor, new Rectangle(MouseState.Position, mousePointerSize), Color.White);
            SpriteBatch.End();
            UserInterface.Active.Draw(SpriteBatch);
            base.Draw(gameTime);
        }

        public void SetNextScreen(string screenName)
        {
            switch(screenName)
            {
                case nameof(StartScreen):
                    CurrentScreen = new StartScreen(this);
                break;
                case nameof(CharacterSelectScreen):
                    CurrentScreen = new CharacterSelectScreen(this);
                    break;
                case nameof(MainGameScreen):
                    if (typeof(CharacterSelectScreen) == CurrentScreen.GetType())
                    {
                        ActiveCharacter = ((CharacterSelectScreen)CurrentScreen).SelectedCharacter;
                    }
                    CurrentScreen = new MainGameScreen(this);
                    break;

            }
        }

        public void Reset(GameScreen screen)
        {
            SetNextScreen("StartScreen");
            screen = null;
            UserInterface.Active.Clear();

        }

    }
}
