using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    internal class Monster : Cards
    {
        public Monster(string name, string type, string element, int dmg) : base(name, type = "Monster", element, dmg)
        {
        }
    }
}
