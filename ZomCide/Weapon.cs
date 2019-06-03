using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public class Weapon : Item
    {
        public int Damage;
        public int DiceThreshold;
        public bool CanBreakDoors;
        public int MinRange;
        public int MaxRange;
        /// <summary>
        /// Indicates the number of dice that can be rolled when using this item
        /// </summary>
        public int Dice { get; private set; }

        public Weapon(int damage, int threshold, bool doors, int minRange, int maxRange, int dice) : base()
        {
            Damage = damage;
            DiceThreshold = threshold;
            CanBreakDoors = doors;
            Dice = dice;
            MinRange = minRange;
            MaxRange = maxRange;
        }

    }
}
