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

        private const string ConnectionString = "Host=myHost;Username=postgres;Password=postgres;Database=mtcg";


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



        public void Login(string username, string password)
        {
            User user = GetUserFromDatabase(username);

            // Check if the user exists
            if (user != null)
            {
                // Check if the entered password matches the stored password
                // Assume PasswordHasher.VerifyPassword method exists and compares hashed passwords
                if (PasswordHasher.VerifyPassword(password, user.Password))
                {
                    Console.WriteLine("Login successful!");
                    // Perform any additional login logic if needed
                }
                else
                {
                    Console.WriteLine("Incorrect password. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("User not found. Please check your username or register a new account.");
            }
        }

        private User GetUserFromDatabase(string username)
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

        public User ValidateUserLogin(string username, string password)
        {
            return users.Find(u => u.Username == username && u.Password == password);
        }


    }
}
