using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.user
{
    public class UserManager
    {

        private const string ConnectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";


        public List<User> users;

        public UserManager()
        {
            users = new List<User>();

        }

        public void Register(string username, string name, string password)
        {
            // Check if the username is already taken
            if (IsUsernameTaken(username))
            {
                Console.WriteLine("Username is already taken. Please choose a different username.");
                return;
            }

            string hashedPassword = PasswordHasher.HashPassword(password);

            User newUser = new User("", "", 100, 20, username, name, hashedPassword, 0, 0, 0); // Use hashed password

            // Add the user to the list of users
            users.Add(newUser);

            // Add user to database
            AddUserToDatabase(newUser);

            Console.WriteLine("Registration successful!");
        }

        public bool IsUsernameTaken(string username)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
        }

        private void AddUserToDatabase(User user)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("INSERT INTO Users (Bio, Image, Elo, Coins, Username, Name, Password, GamesPlayed, GamesWon, GamesLost, IsAdmin) VALUES (@Bio, @Image, @Elo, @Coins, @Username, @Name, @Password, @GamesPlayed, @GamesWon, @GamesLost, @IsAdmin)", conn))
                {
                    cmd.Parameters.AddWithValue("@Bio", user.Bio);
                    cmd.Parameters.AddWithValue("@Image", user.Image);
                    cmd.Parameters.AddWithValue("@Elo", user.Elo);
                    cmd.Parameters.AddWithValue("@Coins", user.Coins);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@GamesPlayed", user.gamesPlayed);
                    cmd.Parameters.AddWithValue("@GamesWon", user.gamesWon);
                    cmd.Parameters.AddWithValue("@GamesLost", user.gamesLost);
                    cmd.Parameters.AddWithValue("@IsAdmin", user.admin);

                    cmd.ExecuteNonQuery();
                }
            }
        }



        public User Login(string username, string password)
        {
            User user = GetUserFromDatabase(username);

            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                // Additional login logic if needed
                return user; // Return the User object
            }

            return null; // Return null if login is unsuccessful
        }


        public User GetUserFromDatabase(string username)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT Bio, Image, Elo, Coins, Username, Name, Password, GamesPlayed, GamesWon, GamesLost FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Assuming a method to create a User object from a data reader
                            return CreateUserFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        private User CreateUserFromReader(NpgsqlDataReader reader)
        {
            return new User(
                reader["Bio"].ToString(),
                reader["Image"].ToString(),
                Convert.ToInt32(reader["Elo"]),
                Convert.ToInt32(reader["Coins"]),
                reader["Username"].ToString(),
                reader["Name"].ToString(),
                reader["Password"].ToString(),
                Convert.ToInt32(reader["GamesPlayed"]),
                Convert.ToInt32(reader["GamesWon"]),
                Convert.ToInt32(reader["GamesLost"])
            );
        }

        public bool EditUser(User updatedUser)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                // SQL query to update user details
                string query = "UPDATE Users SET Name = @Name, Bio = @Bio, Image = @Image WHERE Username = @Username";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    // Bind the parameters
                    cmd.Parameters.AddWithValue("@Name", updatedUser.Name);
                    cmd.Parameters.AddWithValue("@Bio", updatedUser.Bio);
                    cmd.Parameters.AddWithValue("@Image", updatedUser.Image);
                    cmd.Parameters.AddWithValue("@Username", updatedUser.Username);

                    // Execute the command and check if any row is affected
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
        }

        public UserStatsModel UserStats(string username)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT GamesPlayed, GamesWon, GamesLost, Elo FROM Users WHERE Username = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserStatsModel
                            {
                                GamesPlayed = reader.GetInt32(reader.GetOrdinal("GamesPlayed")),
                                GamesWon = reader.GetInt32(reader.GetOrdinal("GamesWon")),
                                GamesLost = reader.GetInt32(reader.GetOrdinal("GamesLost")),
                                Elo = reader.GetInt32(reader.GetOrdinal("Elo"))
                            };
                        }
                    }
                }
            }
            return null;
        }

            public class UserStatsModel
        {
            public int GamesPlayed { get; set; }
            public int GamesWon { get; set; }
            public int GamesLost { get; set; }
            public int Elo { get; set; }
        }


        public string GetScores(string callingUsername)
        {
            StringBuilder scoreBoard = new StringBuilder();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT Username, Elo FROM Users ORDER BY Elo DESC";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader.GetString(reader.GetOrdinal("Username"));
                            int elo = reader.GetInt32(reader.GetOrdinal("Elo"));

                            if (username == callingUsername)
                            {
                                // Highlight the calling user
                                scoreBoard.AppendLine($"**{username} ................. {elo}**");
                            }
                            else
                            {
                                scoreBoard.AppendLine($"{username} ................. {elo}");
                            }
                        }
                    }
                }
            }

            return scoreBoard.ToString();
        }





        public User ValidateUserLogin(string username, string password)
        {
            return users.Find(u => u.Username == username && u.Password == password);
        }


    }
}
