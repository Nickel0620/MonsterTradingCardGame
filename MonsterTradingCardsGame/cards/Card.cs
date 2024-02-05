using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    public class Card
    {
        // Properties
        public string Type { get; set; }
        public string CreatureName { get; set; }
        public string Element { get; set; }
        public int Id { get; set; }
        public int Damage { get; set; }
        public string CardName { get { return Element + " " + CreatureName; } }

        // Constructor
        public Card(string type, string creatureName, string element, int id, int dmg)
        {
            Type = type;
            CreatureName = creatureName;
            Element = element;
            Id = id;
            Damage = dmg;
        }
    }
}
