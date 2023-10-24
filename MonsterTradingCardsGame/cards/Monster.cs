using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    internal class Monster : Cards
    {
        public Monster(string name, string type, string element, int dmg) : base(name, type = "Monster", element, dmg)
        {
        }
    }
}
