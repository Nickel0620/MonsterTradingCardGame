using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame
{
    internal abstract class Cards
    {
        protected string _cardName;
        protected string _cardType;
        protected int _damage;
        protected string _elementType;

        protected Cards(string name, string type, string element, int dmg)
        {
        
            _cardName = name;
            _cardType = type;
            _damage = dmg;
            _elementType = element;
        }
    }
}
