using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide
{
    public class Skill
    {
        public string SkillName {get; private set;}

        public Skill(string name)
        {
            SkillName = name;
        }
    }
}
