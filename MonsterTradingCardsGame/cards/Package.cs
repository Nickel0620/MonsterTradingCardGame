using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    public class Package
    {
        private string connectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";

        public List<Card> Cards { get; set; }

        public Package()
        {
            Cards = GenerateCards();
        }

        private List<Card> GenerateCards()
        {
            List<Card> generatedCards = new List<Card>();
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                string type = (random.Next(2) == 0) ? "monster" : "spell";
                string creatureName = (type == "monster") ? GetRandomCreatureName() : "Spell";
                string element = GetRandomElement();
                string id = ""; // Assuming CurlID is not critical in card generation
                double damage = random.Next(0, 101);
                string cardname = element + creatureName;

                // Create card in database and retrieve CardID
                Card newCard = CreateCard(type, creatureName, element, id, damage, cardname);
                generatedCards.Add(newCard);
            }

            return generatedCards;
        }

        private string GetRandomCreatureName()
        {
            string[] creatureNames = { "Goblin", "Dragon", "Wizard", "Orc", "Knight", "Kraken", "Elves" };
            Random random = new Random();
            return creatureNames[random.Next(creatureNames.Length)];
        }

        private string GetRandomElement()
        {
            string[] elements = { "Water", "Fire", "Normal" };
            Random random = new Random();
            return elements[random.Next(elements.Length)];
        }

        private Card CreateCard(string type, string creatureName, string element, string curlId, double damage, string cardName)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string insertSql = "INSERT INTO Cards (Type, CreatureName, Element, CurlID, Damage, CardName) VALUES (@Type, @CreatureName, @Element, @CurlId, @Damage, @CardName) RETURNING CardID";
                using (var cmd = new NpgsqlCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@CreatureName", creatureName);
                    cmd.Parameters.AddWithValue("@Element", element);
                    cmd.Parameters.AddWithValue("@CurlId", curlId);
                    cmd.Parameters.AddWithValue("@Damage", damage);
                    cmd.Parameters.AddWithValue("@CardName", cardName);

                    int cardId = (int)cmd.ExecuteScalar();
                    return new Card(cardId, type, creatureName, element, curlId, damage, cardName);
                }
            }
        }
    }




}
