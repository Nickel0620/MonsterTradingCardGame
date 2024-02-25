using MonsterTradingCardsGame.cards;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.user
{
    public class User
    {
        // Properties

        public string Bio { get; set; }

        public string Image { get; set; }
        public int Elo { get; set; }
        public int Coins { get; set; }
        public string Username { get; set; }

        public string Name { get; set; }
        public string Password { get; set; } // In a real-world scenario, you would store hashed passwords.

        public int gamesPlayed { get; set; }

        public int gamesWon { get; set; } 
        public int gamesLost { get; set; } 

        public List<Card> Stack { get; set; }
        public List<Card> Deck { get; set; }

        public bool admin = false;


        private const string ConnectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";
        // Constructor
        public User(string bio, string image, int elo, int coins, string username, string name, string password, int gamesPlayed, int gamesWon, int gamesLost)
        {
            Bio = bio ?? "";
            Image = image ?? "";
            Elo = elo;
            Coins = coins;
            Username = username;
            Name = name ?? "";
            Password = password;
            Stack = new List<Card>();
            Deck = new List<Card>();
            this.gamesPlayed = gamesPlayed;
            this.gamesWon = gamesWon;
            this.gamesLost = gamesLost;
            if (username == "admin")
            {
                admin = true;
            }
           
        }


        //BuyPackage Method ist für die Ursprüngliche Implementierung des Kaufens von Kartenpaketen 
        //bzw für das Unique feature relevant. wird aber zur Zeit nicht verwendet. Deshalb habe ich es hier gelassen.  
        //müsste nach UseCardManager verschoben werden.

        public bool BuyPackage()
        {
            int packageCost = 5;
            if (this.Coins < packageCost)
            {
                // Not enough coins to buy a package
                return false;
            }

            this.Coins -= packageCost; // Deduct coins

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Deduct coins in the database
                        string updateCoinsSql = "UPDATE Users SET Coins = Coins - @Cost WHERE Username = @Username";
                        using (var cmd = new NpgsqlCommand(updateCoinsSql, connection))
                        {
                            cmd.Parameters.AddWithValue("@Cost", packageCost);
                            cmd.Parameters.AddWithValue("@Username", this.Username);
                            cmd.ExecuteNonQuery();
                        }

                        Package newPackage = new Package(); // Create a new package
                        this.Stack.AddRange(newPackage.Cards); // Add the new cards to the user's stack

                        foreach (var card in newPackage.Cards)
                        {
                            // Retrieve the CardID based on the card details
                            string getCardIdSql = "SELECT CardID FROM Cards WHERE Type = @Type AND CreatureName = @CreatureName AND Element = @Element AND Damage = @Damage AND CardName = @CardName";
                            int cardId;
                            using (var cmd = new NpgsqlCommand(getCardIdSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@Type", card.Type);
                                cmd.Parameters.AddWithValue("@CreatureName", card.CreatureName);
                                cmd.Parameters.AddWithValue("@Element", card.Element);
                                cmd.Parameters.AddWithValue("@Damage", card.Damage);
                                cmd.Parameters.AddWithValue("@CardName", card.CardName);

                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (!reader.Read())
                                    {
                                        throw new InvalidOperationException("Card not found in database.");
                                    }
                                    cardId = (int)reader["CardID"];
                                }
                            }

                            // Insert new card into the UserCards table
                            string insertCardSql = "INSERT INTO UserCards (UserID, CardID) VALUES ((SELECT UserID FROM Users WHERE Username = @Username), @CardID)";
                            using (var cmd = new NpgsqlCommand(insertCardSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@Username", this.Username);
                                cmd.Parameters.AddWithValue("@CardID", cardId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true; // Successful purchase
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Handle exception (e.g., log the error)
                        return false;
                    }
                }
            }
        }

              
    }


}



