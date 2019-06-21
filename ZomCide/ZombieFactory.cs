using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZomCide.GamePlayers
{
    static class ZombieFactory
    {
        public static void Spawn(SkillLevel level, int row, int column)
        {
            Tile loc = MainGameScreen.tileData.Find(x => x.row == row && x.column == column);
            int rand = new Random((MainGameScreen.RNG.Next(0, 500))).Next(0, 3) ;
            switch (level)
            {
                case SkillLevel.Blue:

                    if (rand == 0)
                    {
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                    }
                    if (rand == 1)
                    {
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                    }
                    break;
                case SkillLevel.Yellow:
                    if (rand == 0)
                    {
                        Zombie.zombieList.Add(new Fatty(row, column, loc));
                    }
                    if (rand == 1 || rand == 2)
                    {
                        Zombie.zombieList.Add(new Runner(row, column, loc));
                        Zombie.zombieList.Add(new Runner(row, column, loc));
                    }
                    break;
                case SkillLevel.Orange:
                    if (rand == 0)
                    {
                        Zombie.zombieList.Add(new Runner(row, column, loc));
                        Zombie.zombieList.Add(new Runner(row, column, loc));
                        Zombie.zombieList.Add(new Runner(row, column, loc));
                    }
                    if (rand == 1 || rand == 2)
                    {
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Fatty(row, column, loc));
                    }
                    break;
                case SkillLevel.Red:
                    
                    if (rand == 0)
                    {
                        Zombie.zombieList.Add(new Fatty(row, column, loc));
                        Zombie.zombieList.Add(new Fatty(row, column, loc));
                    }
                    if (rand == 1 || rand == 2)
                    {
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                        Zombie.zombieList.Add(new Walker(row, column, loc));
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
