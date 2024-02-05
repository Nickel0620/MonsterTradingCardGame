using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.user
{
    public class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            users = new List<User>();
        }

        public void Register(string name, string password)
        {
            // Check if the username is already taken
            if (IsUsernameTaken(name))
            {
                Console.WriteLine("Username is already taken. Please choose a different username.");
                return;
            }

            // Generate a unique ID for the user (you might have a more sophisticated method in a real application)
            int id = users.Count + 1;

            // Create a new user
            User newUser = new User(id, 1000, 0, name, password); // Assuming initial Elo and Coins values

            // Add the user to the list of users
            users.Add(newUser);

            Console.WriteLine("Registration successful!");
        }

        public void Login(string name, string password)
        {
            // Find the user with the specified username
            User user = users.Find(u => u.Name == name);

            // Check if the user exists
            if (user != null)
            {
                // Check if the entered password matches the stored password (in a real-world scenario, compare hashed passwords)
                if (user.Password == password)
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

        private bool IsUsernameTaken(string name)
        {
            // Check if the username is already in use
            return users.Any(u => u.Name == name);
        }
    }
}
