using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.logic
{
    public class TradingDeal
    {
        public Card OfferedCard { get; private set; }
        public string DesiredType { get; private set; }
        public int MinimumDamage { get; private set; }
        public User OfferedBy { get; private set; }
        private const string ConnectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";
        public TradingDeal(User offeredBy, Card offeredCard, string desiredType, int minimumDamage)
        {
            if (offeredBy.Deck.Contains(offeredCard))
            {
                throw new InvalidOperationException("The card is currently in the deck and cannot be offered for trade.");
            }

            OfferedCard = offeredCard;
            DesiredType = desiredType;
            MinimumDamage = minimumDamage;
            OfferedBy = offeredBy;

            // Lock the card for deck usage
            offeredBy.Stack.Remove(offeredCard);
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        // Method to accept a trade
        public bool AcceptTrade(User acceptingUser, Card offeredInExchange)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (!ValidateTrade(offeredInExchange, connection))
                        {
                            Console.WriteLine("The offered card does not meet the trade requirements.");
                            return false;
                        }

                        // Update the UserCards table to reflect the trade
                        UpdateUserCards(acceptingUser, offeredInExchange, connection);
                        UpdateUserCards(OfferedBy, OfferedCard, connection);

                        transaction.Commit();
                        Console.WriteLine("Trade accepted and completed.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred during the trade: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        private bool ValidateTrade(Card offeredInExchange, NpgsqlConnection connection)
        {
            string query = "SELECT Type, Damage FROM Cards WHERE CardID = @CardID";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CardID", offeredInExchange.CardID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string type = reader.GetString(reader.GetOrdinal("Type"));
                        double damage = reader.GetDouble(reader.GetOrdinal("Damage"));

                        return type == DesiredType && damage >= MinimumDamage;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private void UpdateUserCards(User user, Card card, NpgsqlConnection connection)
        {
            // Retrieve the UserID for the offering user
            int offeringUserId = GetUserIdByUsername(OfferedBy.Username, connection);
            // Retrieve the UserID for the accepting user
            int acceptingUserId = GetUserIdByUsername(user.Username, connection);

            // Remove the offered card from the offering user
            string removeCardQuery = "UPDATE UserCards SET UserID = @NewUserID WHERE CardID = @CardID AND UserID = @CurrentUserID";

            using (var removeCmd = new NpgsqlCommand(removeCardQuery, connection))
            {
                removeCmd.Parameters.AddWithValue("@NewUserID", acceptingUserId);
                removeCmd.Parameters.AddWithValue("@CardID", card.CardID);
                removeCmd.Parameters.AddWithValue("@CurrentUserID", offeringUserId);
                removeCmd.ExecuteNonQuery();
            }

            // Add the offered card to the accepting user
            string addCardQuery = "UPDATE UserCards SET UserID = @NewUserID WHERE CardID = @CardID AND UserID = @CurrentUserID";

            using (var addCmd = new NpgsqlCommand(addCardQuery, connection))
            {
                addCmd.Parameters.AddWithValue("@NewUserID", offeringUserId);
                addCmd.Parameters.AddWithValue("@CardID", OfferedCard.CardID);
                addCmd.Parameters.AddWithValue("@CurrentUserID", acceptingUserId);
                addCmd.ExecuteNonQuery();
            }
        }

        private int GetUserIdByUsername(string username, NpgsqlConnection connection)
        {
            string query = "SELECT UserID FROM Users WHERE Username = @Username";

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(reader.GetOrdinal("UserID"));
                    }
                    else
                    {
                        throw new InvalidOperationException("User not found.");
                    }
                }
            }
        }
    }
}
