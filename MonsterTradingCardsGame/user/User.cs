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


        private const string ConnectionString = "Host=myHost;Username=postgres;Password=myPassword;Database=myDatabase";
        // Constructor
        public User(string bio, string image, int elo, int coins, string username, string name, string password, int gamesPlayed, int gamesWon, int gamesLost)
        {
            Bio = bio ?? "";
            Image = image ?? "";
            Elo = elo;
            Coins = coins;
            Username = username;
            Name = name;
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
            ConnectAndSetupUser();
        }


        private void ConnectAndSetupUser()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                // Check if the user already exists
                using (var checkUserCmd = new NpgsqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @username", conn))
                {
                    checkUserCmd.Parameters.AddWithValue("username", Username);
                    int userExists = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                    if (userExists == 0)
                    {
                        // Insert new user since they do not exist
                        using (var insertUserCmd = new NpgsqlCommand("INSERT INTO Users (Bio, Image, Elo, Coins, Username, Name, Password, GamesPlayed, GamesWon, GamesLost) VALUES (@bio, @image, @elo, @coins, @username, @name, @password, @gamesPlayed, @gamesWon, @gamesLost)", conn))
                        {
                            insertUserCmd.Parameters.AddWithValue("bio", Bio);
                            insertUserCmd.Parameters.AddWithValue("image", Image);
                            insertUserCmd.Parameters.AddWithValue("elo", Elo);
                            insertUserCmd.Parameters.AddWithValue("coins", Coins);
                            insertUserCmd.Parameters.AddWithValue("username", Username);
                            insertUserCmd.Parameters.AddWithValue("name", Name);
                            insertUserCmd.Parameters.AddWithValue("password", Password); // Ensure this is hashed
                            insertUserCmd.Parameters.AddWithValue("gamesPlayed", gamesPlayed);
                            insertUserCmd.Parameters.AddWithValue("gamesWon", gamesWon);
                            insertUserCmd.Parameters.AddWithValue("gamesLost", gamesLost);

                            insertUserCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Console.WriteLine("User already exists.");
                    }
                }

                conn.Close();
            }
        }

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

        // Method to add cards from the stack to the deck
        public void AddCardsToDeck()
        {
            if (Stack.Count < 4)
            {
                Console.WriteLine("Not enough cards in the stack to fill the deck.");
                return;
            }

            // Display the cards in the stack
            Console.WriteLine("Cards in the stack:");
            for (int i = 0; i < Stack.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Stack[i].CardInfo()}"); // Assuming CardInfo method exists
            }

            Console.WriteLine("Select 4 cards to add to the deck (enter card numbers separated by spaces):");
            string[] selectedCardIndices = Console.ReadLine().Split(' ');

            if (selectedCardIndices.Length != 4)
            {
                Console.WriteLine("Please select exactly 4 cards.");
                return;
            }

            if (selectedCardIndices.All(index => int.TryParse(index, out _)))
            {
                List<int> indices = selectedCardIndices.Select(int.Parse).ToList();

                if (indices.All(index => index >= 1 && index <= Stack.Count))
                {
                    Deck.Clear();
                    List<Card> selectedCards = new List<Card>();
                    foreach (int index in indices)
                    {
                        Card selectedCard = Stack[index - 1];
                        Deck.Add(selectedCard);
                        selectedCards.Add(selectedCard);
                    }

                    // Remove selected cards from the stack
                    Stack.RemoveAll(card => selectedCards.Contains(card));

                    // Update the InDeck status in the database
                    UpdateCardDeckStatus(selectedCards, true);

                    Console.WriteLine("Cards added to the deck successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid card indices selected.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter valid numbers.");
            }
        }

        private void UpdateCardDeckStatus(List<Card> cards, bool inDeck)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                foreach (var card in cards)
                {
                    string updateSql = "UPDATE UserCards SET InDeck = @InDeck WHERE UserID = (SELECT UserID FROM Users WHERE Username = @Username) AND CardID = @CardID";
                    using (var cmd = new NpgsqlCommand(updateSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@InDeck", inDeck);
                        cmd.Parameters.AddWithValue("@Username", this.Username);
                        cmd.Parameters.AddWithValue("@CardID", card.CardID); // Ensure CardID is correctly set
                        cmd.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }


    }


}



