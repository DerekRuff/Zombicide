using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZomCide.GamePlayers;

namespace ZomCide.Skills.Passive
{
    class ExtraDie : PassiveSkill
    {
        public WeaponType DieType { get; set; }

        public ExtraDie(string name) : base(name)
        {
            if (name.ToLower().Contains("magic")) { DieType = WeaponType.CombatSpell; }
            else if (name.ToLower().Contains("melee")) { DieType = WeaponType.Melee; }
            else if (name.ToLower().Contains("ranged")) { DieType = WeaponType.Melee; }
            else if (name.ToLower().Contains("combat")) { DieType = WeaponType.All; }
        }

        internal static bool Find(string name)
        {
            switch (name.ToLower())
            {
                case ("extra magic die"):
                    return true;
                default:
                    return false;
            }
        }

        public override void Activate(Character character)
        {
            
        }
    }
}
