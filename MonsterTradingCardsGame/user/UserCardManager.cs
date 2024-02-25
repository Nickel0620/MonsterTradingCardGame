using MonsterTradingCardsGame.cards;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.user
{
    public class UserCardManager
    {
        private const string ConnectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";

        // Constructor
        public UserCardManager(string connectionString)
        {
           
        }

        // Method to list all cards of a user
        public List<Card> GetAllUserCards(string username)
        {
            List<Card> userCards = new List<Card>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                // First, get the UserID from the Users table
                int userId = GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    // Handle case where user is not found
                    return userCards;
                }

                // Join UserCards with Cards table to retrieve card details
                string query = @"
                SELECT c.*, uc.InDeck 
                FROM UserCards uc 
                JOIN Cards c ON uc.CardID = c.CardID 
                WHERE uc.UserID = @userId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int cardId = reader.GetInt32(reader.GetOrdinal("CardID"));
                            string type = reader.GetString(reader.GetOrdinal("Type"));
                            string creatureName = reader.GetString(reader.GetOrdinal("CreatureName"));
                            string element = reader.GetString(reader.GetOrdinal("Element"));
                            string curlId = reader.GetString(reader.GetOrdinal("CurlId"));
                            double damage = reader.GetDouble(reader.GetOrdinal("Damage"));
                            string cardName = reader.GetString(reader.GetOrdinal("CardName"));

                            Card card = new Card(cardId, type, creatureName, element, curlId, damage, cardName);
                            userCards.Add(card);
                        }
                    }
                }
                connection.Close();
            }

            return userCards;
        }
        private int GetUserIdByUsername(NpgsqlConnection connection, string username)
        {
            using (var command = new NpgsqlCommand("SELECT UserID FROM Users WHERE Username = @username", connection))
            {
                command.Parameters.AddWithValue("@username", username);

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return (int)result;
                }
            }

            return -1; // Return -1 if user not found
        }

        // Method to get the user's deck
        public List<Card> GetUserDeck(string username)
        {
            List<Card> userDeck = new List<Card>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Get the UserID from the Users table
                int userId = GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    // Handle case where user is not found
                    return userDeck;
                }

                // Retrieve deck details from UserDeck table
                string query = @"
                  SELECT c.*, uc.InDeck 
                  FROM UserDeck ud
                  JOIN UserCards uc ON uc.UserCardID = ud.Card1 OR uc.UserCardID = ud.Card2 OR uc.UserCardID = ud.Card3 OR uc.UserCardID = ud.Card4
                  JOIN Cards c ON uc.CardID = c.CardID 
                  WHERE ud.UserID = @userId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int cardId = reader.GetInt32(reader.GetOrdinal("CardID"));
                            string type = reader.GetString(reader.GetOrdinal("Type"));
                            string creatureName = reader.GetString(reader.GetOrdinal("CreatureName"));
                            string element = reader.GetString(reader.GetOrdinal("Element"));
                            string curlId = reader.GetString(reader.GetOrdinal("CurlId"));
                            double damage = reader.GetDouble(reader.GetOrdinal("Damage"));
                            string cardName = reader.GetString(reader.GetOrdinal("CardName"));

                            Card card = new Card(cardId, type, creatureName, element, curlId, damage, cardName);
                            userDeck.Add(card);
                        }
                    }
                }
                

                connection.Close();
            }

            return userDeck;
        }

        //diese Funktion wurde von ChatGPT anhand GetUserDeck methode angepasst damit eine schöne Tabelle gezeigt wird. 
        public List<Card> GetUserDeckAsTableRows(string username)
        {
            List<Card> userDeck = new List<Card>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Get the UserID from the Users table
                int userId = GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    // Handle case where user is not found
                    return userDeck;
                }

                // Retrieve deck details from UserDeck table
                string query = @"
          SELECT c.*, uc.InDeck 
          FROM UserDeck ud
          JOIN UserCards uc ON uc.UserCardID = ud.Card1 OR uc.UserCardID = ud.Card2 OR uc.UserCardID = ud.Card3 OR uc.UserCardID = ud.Card4
          JOIN Cards c ON uc.CardID = c.CardID 
          WHERE ud.UserID = @userId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        // Print the table header
                        Console.WriteLine("CardID | Type | CreatureName | Element | CurlId | Damage | CardName");
                        Console.WriteLine(new string('-', 80)); // Separator

                        while (reader.Read())
                        {
                            int cardId = reader.GetInt32(reader.GetOrdinal("CardID"));
                            string type = reader.GetString(reader.GetOrdinal("Type"));
                            string creatureName = reader.GetString(reader.GetOrdinal("CreatureName"));
                            string element = reader.GetString(reader.GetOrdinal("Element"));
                            string curlId = reader.GetString(reader.GetOrdinal("CurlId"));
                            double damage = reader.GetDouble(reader.GetOrdinal("Damage"));
                            string cardName = reader.GetString(reader.GetOrdinal("CardName"));

                            // Print each card as a row in the table
                            Console.WriteLine($"{cardId} | {type} | {creatureName} | {element} | {curlId} | {damage} | {cardName}");

                            Card card = new Card(cardId, type, creatureName, element, curlId, damage, cardName);
                            userDeck.Add(card);
                        }
                    }
                }

                connection.Close();
            }

            return userDeck;
        }

        public bool ConfigureUserDeck(string username, List<string> selectedCurlIds)
            {
                    if (selectedCurlIds.Count != 4)
                    {
                        // The user must select exactly 4 cards
                        return false;
                    }

                    using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Get the UserID from the Users table
                int userId = GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    // User not found
                    return false;
                }

                // Verify that the user owns the selected cards and get UserCardIDs
                int[] userCardIds = new int[4];
                for (int i = 0; i < selectedCurlIds.Count; i++)
                {
                    int userCardId = GetUserCardIdByUserIdAndCurlId(connection, userId, selectedCurlIds[i]);
                    if (userCardId == -1)
                    {
                        // User does not own one of the selected cards
                        return false;
                    }
                    userCardIds[i] = userCardId;
                }

                // Update the UserDeck table
                string updateDeckQuery = @"
                    INSERT INTO UserDeck (UserID, Card1, Card2, Card3, Card4)
                    VALUES (@userId, @card1, @card2, @card3, @card4)
                    ON CONFLICT (UserID) DO UPDATE 
                    SET Card1 = EXCLUDED.Card1, Card2 = EXCLUDED.Card2, Card3 = EXCLUDED.Card3, Card4 = EXCLUDED.Card4";

                using (var command = new NpgsqlCommand(updateDeckQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    for (int i = 0; i < userCardIds.Length; i++)
                    {
                        command.Parameters.AddWithValue($"@card{i + 1}", userCardIds[i]);
                    }

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return true;
        }

        private int GetUserCardIdByUserIdAndCurlId(NpgsqlConnection connection, int userId, string curlId)
        {
            // Query to select the UserCardID based on the userId and curlId
            string query = @"
        SELECT uc.UserCardID 
        FROM UserCards uc
        JOIN Cards c ON uc.CardID = c.CardID
        WHERE uc.UserID = @userId AND c.CurlID = @curlId";

            using (var command = new NpgsqlCommand(query, connection))
            {
                // Add parameters to avoid SQL injection
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@curlId", curlId);

                // Execute the command and process the result
                object result = command.ExecuteScalar();

                // If a result is found, cast it to int, otherwise return -1
                return result != null ? (int)result : -1;
            }
        }

        public bool SellCard(string username, int userCardId)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                // First, get the UserID from the Users table
                int userId = GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    // Handle case where user is not found
                    return false;
                }

                // Check if the user owns the card and get card details
                Card card = GetUserCardById(connection, userId, userCardId);
                if (card == null)
                {
                    // Handle case where the user does not own the card or card does not exist
                    return false;
                }

                int coinsEarned = CalculateCoinValue(card);

                // Start a transaction
                using (var transaction = connection.BeginTransaction())
                {
                    // Update the user's coin balance
                    if (!UpdateUserCoins(connection, userId, coinsEarned))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // Delete the sold card
                    if (!DeleteSoldCard(connection, userCardId))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                }

                return true;
            }
        }

        private bool DeleteSoldCard(NpgsqlConnection connection, int userCardId)
        {
            string deleteQuery = "DELETE FROM UserCards WHERE UserCardID = @userCardId";

            using (var command = new NpgsqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@userCardId", userCardId);

                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        private int CalculateCoinValue(Card card)
        {
            string element = card.Element;

            // Treat null or empty element as "normal"
            if (string.IsNullOrEmpty(element))
            {
                element = "normal";
            }

            // Calculate coins based on element and damage
            if (element == "normal")
            {
                return card.Damage > 50 ? 2 : 1;
            }
            else
            {
                return card.Damage > 50 ? 3 : 1;
            }
        }

        private bool UpdateUserCoins(NpgsqlConnection connection, int userId, int coinsToAdd)
        {
            string updateQuery = "UPDATE Users SET Coins = Coins + @coinsToAdd WHERE UserID = @userId";

            using (var command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@coinsToAdd", coinsToAdd);
                command.Parameters.AddWithValue("@userId", userId);

                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }


        private Card GetUserCardById(NpgsqlConnection connection, int userId, int userCardId)
        {
            string query = @"
        SELECT c.* 
        FROM UserCards uc 
        JOIN Cards c ON uc.CardID = c.CardID 
        WHERE uc.UserID = @userId AND uc.UserCardID = @userCardId";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@userCardId", userCardId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Card(
                            reader.GetInt32(reader.GetOrdinal("CardID")),
                            reader.GetString(reader.GetOrdinal("Type")),
                            reader.GetString(reader.GetOrdinal("CreatureName")),
                            reader.GetString(reader.GetOrdinal("Element")),
                            reader.GetString(reader.GetOrdinal("CurlId")),
                            reader.GetDouble(reader.GetOrdinal("Damage")),
                            reader.GetString(reader.GetOrdinal("CardName"))
                        );
                    }
                }
            }

            return null;
        }



    }

}
