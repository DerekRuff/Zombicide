using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace ZomCide
{
    public class Item
    {

        public static List<Item> ItemList { get; set; }
        public static List<Weapon> StarterList { get; private set; }
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

        public static void Initialize()
        {
            ItemList = new List<Item>();
            StarterList = new List<Weapon>();
            XmlDocument doc = new XmlDocument();
            string fileName = "Items.xml";
            string ItemPath = Path.Combine(Directory.GetCurrentDirectory(), @"Content\Data\", fileName);
            doc.Load(ItemPath);
            foreach (XmlNode I in doc.DocumentElement.ChildNodes)
            {
                Weapon item;
                switch (I.Name)
                {
                    
                    case ("Weapon"):
                        item = new Weapon(I);
                        if (Boolean.Parse(I.ChildNodes.Item(1).InnerText) == true)
                        { StarterList.Add(item); }
                        else
                        { ItemList.Add(item); }
                        
                        break;
                }
                

            }

        }


    }
}
