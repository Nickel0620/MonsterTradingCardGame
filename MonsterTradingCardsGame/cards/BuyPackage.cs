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

        public string FetchFirstPackage()
        {
            string packageId = null;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT PackageID FROM Packages WHERE AssignedTo IS NULL LIMIT 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            packageId = reader["PackageID"].ToString();
                        }
                    }
                }
            }

            return packageId;
        }

        public bool AssignPackageToUser(string username, string packageId)
        {
            if (DeductCoins(username))
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("UPDATE Packages SET AssignedTo = @username WHERE PackageID = @packageId", connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@packageId", packageId);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }

            return false;
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
    }
}

