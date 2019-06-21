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

        public static List<Dice> DiceList;
        public static Texture2D Texture1 { get; set; }
        public static Texture2D Texture2 { get; set; }
        public static Texture2D Texture3 { get; set; }
        public static Texture2D Texture4 { get; set; }
        public static Texture2D Texture5 { get; set; }
        public static Texture2D Texture6 { get; set; }

        public Texture2D Texture { get; set; }
        public Point Position { get; set; }
        public int value;
        public bool rolling;
        public Point Size { get; set; }
        
        public Dice()
        {
            value = 1;
            Texture = Texture1;
            rolling = false;
            Size = new Point(50, 50);

        }

        public static void Initialize(Zombicide game)
        {
            DiceList = new List<Dice>();
            Texture1 = game.Content.Load<Texture2D>(@"Dice/die_face_1_T");
            Texture2 = game.Content.Load<Texture2D>(@"Dice/die_face_2_T");
            Texture3 = game.Content.Load<Texture2D>(@"Dice/die_face_3_T");
            Texture4 = game.Content.Load<Texture2D>(@"Dice/die_face_4_T");
            Texture5 = game.Content.Load<Texture2D>(@"Dice/die_face_5_T");
            Texture6 = game.Content.Load<Texture2D>(@"Dice/die_face_6_T");
        }

        public static void Clear()
        {
            DiceList.Clear();
        }

        public static void addDice(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Dice D = new Dice();
                DiceList.Add(D);
            }
        }

        public static void Roll()
        {

            foreach (Dice d in DiceList)
            {
                d.rolling = true;
                d.value = new Random(MainGameScreen.RNG.Next(0, 5000)).Next(1,7);
                switch (d.value)
                {
                    case 1:
                        d.Texture = Texture1;
                        break;
                    case 2:
                        d.Texture = Texture2;
                        break;
                    case 3:
                        d.Texture = Texture3;
                        break;
                    case 4:
                        d.Texture = Texture4;
                        break;
                    case 5:
                        d.Texture = Texture5;
                        break;
                    case 6:
                        d.Texture = Texture6;
                        break;
                    default:
                        break;

                }
            }
        }

        public static int CalculateHits(int Threshold)
        {
            return DiceList.Where(x => x.value >= Threshold).Count(); ;
        }

        public void Update(Zombicide game)
        {
            throw new NotImplementedException();
        }

        public void Draw(Zombicide game)
        {
            throw new NotImplementedException();
        }


    }
}
