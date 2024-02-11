using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    public class Package
    {
        public List<Card> Cards { get; set; }

        public Package()
        {
            Cards = GenerateCards();
        }

        public List<Card> GenerateCards()
        {
            List<Card> generatedCards = new List<Card>();
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                string type = (random.Next(2) == 0) ? "monster" : "spell";
                string creatureName = (type == "monster") ? GetRandomCreatureName() : null;
                string element = GetRandomElement();
                int id = i + 1;
                int damage = random.Next(0, 101);

                Card newCard = new Card(type, creatureName, element, id, damage);
                generatedCards.Add(newCard);
            }

            return generatedCards;
        }

        public string GetRandomCreatureName()
        {
            string[] creatureNames = { "Goblin", "Dragon", "Wizard", "Orc", "Knight", "Kraken", "Elves" };
            Random random = new Random();
            return creatureNames[random.Next(creatureNames.Length)];
        }

        public string GetRandomElement()
        {
            string[] elements = { "water", "fire", "normal" };
            Random random = new Random();
            return elements[random.Next(elements.Length)];
        }
    }

    
}
