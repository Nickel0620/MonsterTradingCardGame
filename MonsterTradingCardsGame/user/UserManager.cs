using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.user
{
    public class UserManager
    {
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

            // Generate a unique ID for the user (you might have a more sophisticated method in a real application)
            int id = users.Count + 1;

            string hashedPassword = PasswordHasher.HashPassword(password);

            User newUser = new User("", "", 100, 20, username, name, hashedPassword, 0 , 0, 0); // Use hashed password

            // Add the user to the list of users
            users.Add(newUser);

            Console.WriteLine("Registration successful!");
        }



        public void Login(string username, string password)
        {
            // Find the user with the specified username
            User user = users.Find(u => u.Username == username);

            // Check if the user exists
            if (user != null)
            {
                // Check if the entered password matches the stored password (in a real-world scenario, compare hashed passwords)
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

        public User ValidateUserLogin(string username, string password)
        {
            return users.Find(u => u.Username == username && u.Password == password);
        }

        private bool IsUsernameTaken(string username)
        {
            // Check if the username is already in use
            return users.Any(u => u.Username == username);
        }
    }
}
