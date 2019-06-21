using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public class Tile
    {
        public int row { get; set;}
        public int column { get; set;}
        public bool room { get; set;}
        public bool spawn { get; set; }
        public bool objective { get; set; }
        public RoomSide LeftSide { get; set; }
        public RoomSide RighSide { get; set; }
        public RoomSide TopSide { get; set; }
        public RoomSide BottomSide { get; set; }
        public VaultType containsVault;

        public Tile(string[] tileData)
        {
            row = Convert.ToInt32(tileData[0]);
            column = Convert.ToInt32(tileData[1]);
            room = bool.Parse(tileData[2]);
            spawn = bool.Parse(tileData[3]);
            containsVault = (VaultType)Enum.Parse(typeof(VaultType),tileData[4]);
            objective = bool.Parse(tileData[5]);
            TopSide = (RoomSide)Enum.Parse(typeof(RoomSide), tileData[6]);
            BottomSide = (RoomSide)Enum.Parse(typeof(RoomSide), tileData[7]);
            LeftSide = (RoomSide)Enum.Parse(typeof(RoomSide), tileData[8]);
            RighSide= (RoomSide)Enum.Parse(typeof(RoomSide), tileData[9]);
        }

        public Tile() : this(new string[] { "0", "0", "false", "true", "none", "false", "street", "street", "street", "street" })
        { 
        }

    }
}
