/*using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace MonsterTradingCardsGame.cards
{
    namespace MonsterTradingCardsGame.cards
    {
        public class CurlPack
        {
            public List<Card> Cards { get; private set; }

            public CurlPack(string json)
            {
                Cards = new List<Card>();
                ParseJson(json);
            }

            private void ParseJson(string jsonData)
            {
                // Assuming JSON data is in the format you provided
                var cardData = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);
                foreach (var item in cardData)
                {
                    string id = item.Id;
                    string name = item.Name;
                    double damage = item.Damage;
                    

                    if(name == "Dragon" || name == "Kraken" || name == "Ork" || name == "Knight")
                    {
                        string type = "monster";
                        string element = "normal";
                        string creatureName = name;
                    } else if (name == "WaterGoblin")
                    {
                        string type = "monster";
                        string element = "water";
                        string creatureName = "Goblin";
                    } else if (name == "FireElf")
                    {
                        string type = "monster";
                        string element = "fire";
                        string creatureName = "Elf";
                    } else if (name == "FireSpell")
                    {
                        string type = "spell";
                        string element = "fire";
                        string creatureName = "Spell";
                    } else if (name == "WaterSpell")
                    {
                        string type = "spell";
                        string element = "water";
                        string creatureName = "Spell";
                    } else if (name == "RegularSpell")
                    {
                        string type = "spell";
                        string element = "normal";
                        string creatureName = "Spell";
                    }

                    
                    Card card = new Card(type, element, creatureName, id, (double)damage, name);
                    Cards.Add(card);
                }
            }

            public void SaveToDatabase(string connectionString)
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (var card in Cards)
                    {
                        var cmd = new NpgsqlCommand("INSERT INTO cards (type, creature_name, element, curl_id, damage, card_name) VALUES (@type, @creature_name, @element, @curl_id, @damage, @card_name)", conn);
                        cmd.Parameters.AddWithValue("type", card.Type);
                        cmd.Parameters.AddWithValue("creature_name", card.CreatureName);
                        cmd.Parameters.AddWithValue("element", card.Element);
                        cmd.Parameters.AddWithValue("curl_id", card.CurlId);
                        cmd.Parameters.AddWithValue("damage", card.Damage);
                        cmd.Parameters.AddWithValue("card_name", card.CardName);
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
        }
    }
}

*/