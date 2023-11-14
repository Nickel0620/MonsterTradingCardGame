using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    internal class Spell : Cards
    {
        public Spell(string name, string type, string element, int dmg) : base(name, type = "Spell", element, dmg)
        {

        }
    }
}
