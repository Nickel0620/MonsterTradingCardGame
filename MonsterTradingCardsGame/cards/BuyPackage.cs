using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    internal class BuyPackage
    {
        private const string connectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";
        private const int PackageCost = 5;

        public int FetchFirstPackage()
        {
            int? packageId = null;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT PackageID FROM Packages LIMIT 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            packageId = (int)reader["PackageID"];
                        }
                    }
                }
            }

            return packageId ?? -1; // Return -1 if no package is found
        }

        public bool AssignPackageToUser(string username, int packageId)
        {
            int userId = FetchUserId(username);
            if (userId == -1)
            {
                return false; // User not found
            }

            if (DeductCoins(username))
            {
                bool packageExists = false;
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Check if the package exists
                        using (var checkCommand = new NpgsqlCommand("SELECT COUNT(*) FROM Packages WHERE PackageID = @packageId", connection))
                        {
                            checkCommand.Parameters.AddWithValue("@packageId", packageId);
                            int packageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (packageCount > 0)
                            {
                                packageExists = true;

                            }
                        }

                        // Add cards from the package to the user if the package exists
                        if (packageExists && AddCardsToUser(userId, packageId, connection, transaction))
                        {
                            // Delete the package from the database
                            using (var deleteCommand = new NpgsqlCommand("DELETE FROM Packages WHERE PackageID = @packageId", connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@packageId", packageId);
                                deleteCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool HasEnoughCoins(string username)
        {
            int packageCost = 5; // The cost of the package
            int userCoins = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand($"SELECT Coins FROM Users WHERE Username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userCoins = reader.GetInt32(0);
                        }
                    }
                }
            }

            return userCoins >= packageCost;
        }

        private bool DeductCoins(string username)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT Coins FROM Users WHERE Username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int currentCoins = (int)reader["Coins"];
                            if (currentCoins >= PackageCost)
                            {
                                reader.Close();
                                using (var updateCommand = new NpgsqlCommand("UPDATE Users SET Coins = Coins - @cost WHERE Username = @username", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@cost", PackageCost);
                                    updateCommand.Parameters.AddWithValue("@username", username);
                                    updateCommand.ExecuteNonQuery();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private int FetchUserId(string username)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT UserID FROM Users WHERE Username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (int)reader["UserID"];
                        }
                    }
                }
            }

            return -1; // Or handle this case as you see fit
        }

        private bool AddCardsToUser(int userId, int packageId, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            List<int> cardIds = new List<int>();

            using (var packageCommand = new NpgsqlCommand("SELECT CardID1, CardID2, CardID3, CardID4, CardID5 FROM Packages WHERE PackageID = @packageId", connection, transaction))
            {
                packageCommand.Parameters.AddWithValue("@packageId", packageId);

                using (var reader = packageCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            cardIds.Add((int)reader[$"CardID{i}"]);
                        }
                    }
                }
            }

            foreach (int cardId in cardIds)
            {
                using (var userCardCommand = new NpgsqlCommand("INSERT INTO UserCards (UserID, CardID) VALUES (@userId, @cardId)", connection, transaction))
                {
                    userCardCommand.Parameters.AddWithValue("@userId", userId);
                    userCardCommand.Parameters.AddWithValue("@cardId", cardId);
                    userCardCommand.ExecuteNonQuery();
                }
            }

            return cardIds.Count > 0;
        }
    }
}

