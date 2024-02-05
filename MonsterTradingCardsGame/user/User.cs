using MonsterTradingCardsGame.cards;
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
        public int Id { get; set; }
        public int Elo { get; set; }
        public int Coins { get; set; }
        public string Name { get; set; }
        public string Password { get; set; } // In a real-world scenario, you would store hashed passwords.

        public List<Card> Stack { get; set; }
        public List<Card> Deck { get; set; }

        // Constructor
        public User(int id, int elo, int coins, string name, string password)
        {
            Id = id;
            Elo = elo;
            Coins = coins;
            Name = name;
            Password = password;
            Stack = new List<Card>();
            Deck = new List<Card>();
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
                Console.WriteLine($"{i + 1}. {Stack[i].CardInfo()}"); // Assuming there's a method CardInfo in Card class
            }

            // Select 4 cards to add to the deck
            Console.WriteLine("Select 4 cards to add to the deck (enter card numbers separated by spaces):");
            string[] selectedCardIndices = Console.ReadLine().Split(' ');

            if (selectedCardIndices.Length != 4)
            {
                Console.WriteLine("Please select exactly 4 cards.");
                return;
            }

            // Convert selected indices to integers
            if (selectedCardIndices.All(index => int.TryParse(index, out _)))
            {
                List<int> indices = selectedCardIndices.Select(int.Parse).ToList();

                // Check if selected indices are valid
                if (indices.All(index => index >= 1 && index <= Stack.Count))
                {
                    // Add selected cards to the deck
                    Deck.Clear();
                    foreach (int index in indices)
                    {
                        Deck.Add(Stack[index - 1]);
                    }

                    // Remove selected cards from the stack
                    Stack.RemoveAll(card => Deck.Contains(card));

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


    }


}



