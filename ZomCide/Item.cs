using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public class Item
    {
        public string Name { get; protected set; }
        /// <summary>
        /// Indicates that this item is a consumable that should be removed from the player
        /// after use.
        /// </summary>
        public bool IsSingleUse { get; private set; }
        /// <summary>
        /// Indicates how much noise using this item will cause
        /// </summary>
        public int Noise { get; private set; }
        /// <summary>
        /// Indicates if this item can be used when a matching item is in 
        /// the player's other hand slot
        /// </summary>
        public bool DualWield { get; private set; }
        /// <summary>
        /// List indicating if this item can be used at a given level
        /// Index = Level
        /// 0 = Blue
        /// 1 = Yellow
        /// 2 = Orange
        /// 3 = Red
        /// </summary>
        public List<bool> AvailableLevel { get; private set; }
        /// <summary>
        /// Indicates the item must be in the player's hand slot to use
        /// </summary>
        public bool MustBeInHand { get; private set; }
        
        /// <summary>
        /// Indicates the minimum value that must be rolled on a die to successfully use this item
        /// </summary>
        public int DiceRollSuccessValue { get; private set; }
        
        /// <summary>
        /// Indicates how many actions are required to use this item
        /// </summary>
        public int ActionCost { get; private set; }
        

    }
}
