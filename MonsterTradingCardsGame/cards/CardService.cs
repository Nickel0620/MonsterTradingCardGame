using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    public class CardService
    {
        private string connectionString = "Host=myHost;Username=postgres;Password=postgres;Database=mtcg";

        public Card CreateCard(string type, string creatureName, string element, string id, double dmg, string cardname)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Insert the new card and retrieve the CardID
                string insertSql = "INSERT INTO Cards (Type, CreatureName, Element, CurlID, Damage, CardName) VALUES (@Type, @CreatureName, @Element, @CurlID, @Damage, @CardName) RETURNING CardID";
                using (var cmd = new NpgsqlCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@CreatureName", creatureName);
                    cmd.Parameters.AddWithValue("@Element", element);
                    cmd.Parameters.AddWithValue("@CurlID", id);
                    cmd.Parameters.AddWithValue("@Damage", dmg);
                    cmd.Parameters.AddWithValue("@CardName", cardname);

                    int cardId = (int)cmd.ExecuteScalar();
                    return new Card(cardId, type, creatureName, element, id, dmg, cardname);
                }
            }
        }
    }
}
