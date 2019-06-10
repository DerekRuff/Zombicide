using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZomCide
{
    public class Objective : IDrawableGameObject
    {
        public static List<int[]> ObjectiveTiles { get; set; }
        public static List<Objective> ObjectiveList { get; set; }
        public static Texture2D RedX { get; set; }
        public static Texture2D BlueX { get; set; }
        public static Texture2D GreenX { get; set; }

        public Point Position { get; set; }
        public Point Size { get; set; }
        public Rectangle Area { get; set; }
        public Texture2D Texture { get; set; }
        public string UndersideColor { get; set; }
        public int[] Tile { get; set; }
        
        

        public bool flipped;

        public Objective(int[] tile)
        {
            flipped = false;
            Texture = RedX;
            UndersideColor = "green";
            Tile = tile;
            Size = new Point(50, 50);
            Area = new Rectangle(MainGameScreen.mapX + (Tile[1] * MainGameScreen.tileWidth) + (MainGameScreen.tileWidth / 2) - 25, MainGameScreen.mapY + Tile[0] * MainGameScreen.tileHeight + (MainGameScreen.tileWidth / 2) - 25,Size.X,Size.Y);
            Position = new Point(Area.X, Area.Y);
        }

        public void Update(Zombicide game)
        {
            
        }

        public void Draw(Zombicide game)
        {
            game.SpriteBatch.Draw(Texture, new Rectangle(MainGameScreen.mapX + (Tile[1] * MainGameScreen.tileWidth)+(MainGameScreen.tileWidth/2)-25 , MainGameScreen.mapY + Tile[0] * MainGameScreen.tileHeight + (MainGameScreen.tileWidth / 2) - 25, 50, 50), Color.White);
            
        }

        public static void LoadTextures(Zombicide game)
        {
            RedX = game.Content.Load<Texture2D>("redX");
            BlueX = game.Content.Load<Texture2D>("blueX");
            GreenX = game.Content.Load<Texture2D>("greenX");
        }

        public static void Initialize(Zombicide game)
        {
            ObjectiveList = new List<Objective>();
            LoadTextures(game);
            foreach (var O in ObjectiveTiles)
            {
                ObjectiveList.Add(new Objective(O));
            }
            ObjectiveList.ElementAt(MainGameScreen.RNG.Next(0,ObjectiveList.Count)).UndersideColor = "blue";
        }

        public void FlipCard(Zombicide game)
        {
            switch (UndersideColor)
            {
                case "green":
                    Texture = GreenX;
                    break;
                case "blue":
                    Texture = BlueX;
                    break;
                default:
                    break;
            }
            flipped = true;
            game.ActiveCharacter.PickupObjective();
        }
    }
}
