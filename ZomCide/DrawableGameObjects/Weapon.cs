using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZomCide
{
    public class Weapon : Item
    {
        public int Damage;
        public int DiceThreshold;
        public bool CanBreakDoors;
        public int MinRange;
        public int MaxRange;
        public bool Active;
        public bool Starter;
        public DoorOpener DoorStatus;
        public SkillLevel MinimumLevel;
        public WeaponType Type;
        public bool DualWield;
        

        /// <summary>
        /// Indicates the number of dice that can be rolled when using this item
        /// </summary>
        public int Dice { get; private set; }

        public Weapon(string name,int damage, int threshold, bool doors, int minRange, int maxRange, int dice, bool active = false)  :base ()
        {
            Name = name;
            Damage = damage;
            DiceThreshold = threshold;
            CanBreakDoors = doors;
            Dice = dice;
            MinRange = minRange;
            MaxRange = maxRange;
            Active = active;
        }

        public Weapon(Zombicide game, XmlNode i) : base(game,i)
        {
            Name = i.ChildNodes.Item(0).InnerText;
            Starter = Boolean.Parse(i.ChildNodes.Item(1).InnerText);
            Enum.TryParse(i.ChildNodes.Item(2).InnerText,out Type);
            MinRange = Convert.ToInt32(i.ChildNodes.Item(3).InnerText);
            MaxRange = Convert.ToInt32(i.ChildNodes.Item(4).InnerText);
            Dice = Convert.ToInt32(i.ChildNodes.Item(5).InnerText);
            DiceThreshold = Convert.ToInt32(i.ChildNodes.Item(6).InnerText);
            Damage = Convert.ToInt32(i.ChildNodes.Item(7).InnerText);
            Enum.TryParse(i.ChildNodes.Item(8).InnerText, out DoorStatus);
            Enum.TryParse(i.ChildNodes.Item(9).InnerText, out MinimumLevel);
            DualWield = Boolean.Parse(i.ChildNodes.Item(10).InnerText);
        }

        
    }
}
