using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZomCide.GamePlayers;

namespace ZomCide.Skills
{
    public class Skill
    {
        public string SkillName {get; set;}
        public bool Active { get; set; }

       public Skill(string name)
        {
            SkillName = name;
            Active = false;
        }

        public virtual void Activate(Character character)
        {
            Active = true;
        }

        public static Skill ParseSkill(string name)
        {
            if (Passive.ExtraDie.Find(name)) { return new Passive.ExtraDie(name); }
            else return new Skill(name);
        }
    }
}
