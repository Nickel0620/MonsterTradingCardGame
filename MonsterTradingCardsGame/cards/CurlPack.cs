﻿using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Npgsql;
using System.Data;

namespace MonsterTradingCardsGame.cards
{
    namespace MonsterTradingCardsGame.cards
    {
        public class CurlPack
        {
            private string connectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";

            public List<Card> CreatePackage(List<CardInput> cardInputs)
            {
                var cards = new List<Card>();

                foreach (var input in cardInputs)
                {
                    (string type, string element, string creatureName) = DetermineCardAttributes(input.Name);

                    var card = new Card(
                        input.CardID,
                        type,
                        creatureName,
                        element,
                        input.CurlId ?? "",
                        input.Damage,
                        input.Name
                    );
                    cards.Add(card);
                    AddPackageToDatabase(cards);
                }

                return cards;
            }

            private (string type, string element, string creatureName) DetermineCardAttributes(string name)
            {
                string type, element, creatureName;

                switch (name)
                {
                    case "Dragon":
                    case "Kraken":
                    case "Ork":
                    case "Knight":
                        type = "monster";
                        element = "normal";
                        creatureName = name;
                        break;
                    case "WaterGoblin":
                        type = "monster";
                        element = "water";
                        creatureName = "Goblin";
                        break;
                    case "FireElf":
                        type = "monster";
                        element = "fire";
                        creatureName = "Elf";
                        break;
                    case "FireSpell":
                    case "WaterSpell":
                    case "RegularSpell":
                        type = "spell";
                        element = name.StartsWith("Water") ? "water" : (name.StartsWith("Fire") ? "fire" : "normal");
                        creatureName = "Spell";
                        break;
                    default:
                        type = "unknown";
                        element = "unknown";
                        creatureName = "unknown";
                        break;
                }

                return (type, element, creatureName);
            }

            private void AddPackageToDatabase(List<Card> cards)
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        // Assuming you want to insert the first 5 cards into the package.
                        // Modify as needed for your logic.
                        command.CommandText = "INSERT INTO Packages (CardID1, CardID2, CardID3, CardID4, CardID5) VALUES (@card1, @card2, @card3, @card4, @card5)";
                        command.Parameters.AddWithValue("@card1", cards[0].CardID);
                        command.Parameters.AddWithValue("@card2", cards[1].CardID);
                        command.Parameters.AddWithValue("@card3", cards[2].CardID);
                        command.Parameters.AddWithValue("@card4", cards[3].CardID);
                        command.Parameters.AddWithValue("@card5", cards[4].CardID);

                        command.ExecuteNonQuery();
                    }
                }
            }

            public class CardInput
            {
                public int CardID { get; set; }
                public string? Name { get; set; } // Full name from JSON
                public string? CurlId { get; set; }
                public double Damage { get; set; }
                public string? CardName { get; set; }
            }
        }
    }
}

