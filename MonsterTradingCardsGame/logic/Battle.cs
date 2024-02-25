using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.logic
{
    public class Battle
    {
        private User player1;
        private User player2;
        private const int MaxRounds = 100;
        private List<string> battleLog = new List<string>();        
        private User object1;
        private User object2;
        private const string ConnectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";

        public Battle(string player1Username, string player2Username)
        {
            int player1Id = GetUserIdByUsername(player1Username);
            int player2Id = GetUserIdByUsername(player2Username);

            player1 = GetUserAndDeck(player1Id);
            player2 = GetUserAndDeck(player2Id);
            
        }

        //for testing 

        public Battle() { }

        public Battle(User object1, User object2)
        {
            this.object1 = object1;
            this.object2 = object2;
        }

        public void setPlayer1(string username)
        {
            int player1Id = GetUserIdByUsername(username);
            player1 = GetUserAndDeck(player1Id);
        }

        public void setPlayer2(string username)
        {
            int player1Id = GetUserIdByUsername(username);
            player2 = GetUserAndDeck(player1Id);
        }

        private int GetUserIdByUsername(string username)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT UserID FROM Users WHERE Username = @Username", conn))
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
                            throw new Exception("User not found");
                        }
                    }
                }
            }
        }

        private User GetUserAndDeck(int userId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                User user = null;
                // Fetch user details
                using (var cmd = new NpgsqlCommand("SELECT * FROM Users WHERE UserID = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                                user = new User(
                                reader["Bio"] as string,
                                reader["Image"] as string,
                                (int)reader["Elo"],
                                (int)reader["Coins"],
                                reader["Username"] as string,
                                reader["Name"] as string,
                                reader["Password"] as string, // Remember to handle hashed passwords
                                (int)reader["GamesPlayed"],
                                (int)reader["GamesWon"],
                                (int)reader["GamesLost"]
                            );                          
                           

                            
                        }
                    }
                }
                if (user != null)
                {
                    // Now fetch the deck for this user
                    user.Deck = GetDeckForUser(userId, conn);
                    return user;
                }
            }

            throw new Exception("User not found");
        }

        private List<Card> GetDeckForUser(int userId, NpgsqlConnection conn)
        {
            List<Card> deck = new List<Card>();

            string query = @"SELECT C.CardID, C.Type, C.CreatureName, C.Element, C.CurlID, C.Damage, C.CardName
	          FROM UserDeck UD 
              JOIN Cards C ON UD.Card1 = C.CardID OR UD.Card2 = C.CardID OR UD.Card3 = C.CardID OR UD.Card4 = C.CardID
              WHERE UD.UserID = @UserId";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Card card = new Card(
                            reader.GetInt32(reader.GetOrdinal("CardID")),
                            reader["Type"] as string,
                            reader["CreatureName"] as string,
                            reader["Element"] as string,
                            reader["CurlId"] as string,
                            (double)reader["Damage"],
                            reader["CardName"] as string
                        );

                        deck.Add(card);
                    }
                }

                return deck;
            }
        }

        public void PlayGame()
        {
            int round = 0;
            while (round < MaxRounds && player1.Deck.Count > 0 && player2.Deck.Count > 0)
            {
                battleLog.Add($"Round {round + 1}:");
                ConductRound();
                round++;
            }

            UpdateStats();
            PrintBattleLog();
        }

        public void ConductRound()
        {
            // Ensure both players have cards in their deck
            if (player1.Deck.Count == 0 || player2.Deck.Count == 0)
            {
                battleLog.Add("One of the players has no cards left.");
                return;
            }

            // Randomly select a card from each player's deck
            Random rnd = new Random();
            Card card1 = player1.Deck[rnd.Next(player1.Deck.Count)];
            Card card2 = player2.Deck[rnd.Next(player2.Deck.Count)];

            // Calculate damage considering element type and special rules
            double card1Damage = CalculateDamage(card1, card2);
            double card2Damage = CalculateDamage(card2, card1);

            // Determine round outcome
            string roundResult;
            if (card1Damage > card2Damage)
            {
                roundResult = $"Player 1's {card1.CardName} defeats Player 2's {card2.CardName}";
                TransferCard(player1, player2, card2); // Transfer card2 from player2 to player1
            }
            else if (card2Damage > card1Damage)
            {
                roundResult = $"Player 2's {card2.CardName} defeats Player 1's {card1.CardName}";
                TransferCard(player2, player1, card1); // Transfer card1 from player1 to player2
            }
            else
            {
                roundResult = $"The round is a draw between Player 1's {card1.CardName} and Player 2's {card2.CardName} cards";
            }

            // Add the round result to the battle log
            battleLog.Add(roundResult);
        }

        public double CalculateDamage(Card attacker, Card defender)
        {
            // Start with the base damage of the attacker
            double damage = attacker.Damage;

            // Apply elemental effectiveness
            if (attacker.Element == "Water")
            {
                if (defender.Element == "Fire") damage *= 2;
                else if (defender.Element == "Normal") damage /= 2;
            }
            else if (attacker.Element == "Fire")
            {
                if (defender.Element == "Normal") damage *= 2;
                else if (defender.Element == "Water") damage /= 2;
            }
            else if (attacker.Element == "Normal")
            {
                if (defender.Element == "Water") damage *= 2;
                else if (defender.Element == "Fire") damage /= 2;
            }

            // Apply special rules
            if (attacker.CreatureName == "Goblin" && defender.CreatureName == "Dragon")
            {
                // Goblins are too afraid of Dragons to attack
                damage = 0;
            }
            else if (attacker.CreatureName == "Wizard" && defender.CreatureName == "Ork")
            {
                // Wizard can control Orks so they are not able to damage them
                damage = 0;
            }
            else if (attacker.Element + attacker.Type == "WaterSpell" && defender.CreatureName == "Knight")
            {
                // The armor of Knights is so heavy that WaterSpells make them drown instantly
                damage = int.MaxValue; // Assign a very high value to ensure defeat
            }
            else if (attacker.Type == "Spell" && defender.CreatureName == "Kraken")
            {
                // The Kraken is immune against spells
                damage = 0;
            }
            else if (attacker.CreatureName == "Dragon" && defender.Element + defender.CreatureName == "FireElf")
            {
                // FireElves can evade Dragon attacks
                damage = 0;
            }

          //  Monster Fights -WaterGoblin vs FireTroll and vice versa
             if ((attacker.Element + attacker.CreatureName == "WaterGoblin" && defender.Element + defender.CreatureName == "FireTroll") ||
                 (attacker.Element + attacker.CreatureName == "FireTroll" && defender.Element + defender.CreatureName == "WaterGoblin"))
            {
                // In both cases, the Troll defeats the Goblin
                if (attacker.CreatureName == "Goblin")
                {
                    damage = 0; // Goblin loses, no damage
                }
                else if (attacker.CreatureName == "Troll")
                {
                    damage = defender.Damage + 1; // Ensure Troll's damage is always higher
                }
            }

            // New Rule: Spell Fights
            if (attacker.Type == "Spell" && defender.Type == "Spell")
            {
                double attackerMultiplier = 1;
                double defenderMultiplier = 1;

                if (attacker.Element == "Fire" && defender.Element == "Water")
                {
                    attackerMultiplier = 1; // FireSpell retains its damage
                    defenderMultiplier = 2; // WaterSpell doubles its damage
                }
                else if (attacker.Element == "Water" && defender.Element == "Fire")
                {
                    attackerMultiplier = 2; // WaterSpell doubles its damage
                    defenderMultiplier = 1; // FireSpell retains its damage
                }

                double adjustedAttackerDamage = damage * attackerMultiplier;
                double adjustedDefenderDamage = defender.Damage * defenderMultiplier;

                // Determine the outcome
                if (adjustedAttackerDamage > adjustedDefenderDamage)
                {
                    damage = adjustedAttackerDamage; // Attacker wins
                }
                else if (adjustedAttackerDamage < adjustedDefenderDamage)
                {
                    damage = 0; // Attacker loses
                }
                else
                {
                    // Draw, no action
                    damage = 0;
                    defender.Damage = 0; // Set defender's damage to 0 to indicate a draw
                }
            }

            // New Rule: Mixed Fights
            if ((attacker.Type == "Spell" && defender.Type == "Monster") ||
                (attacker.Type == "Monster" && defender.Type == "Spell"))
            {
                double attackerMultiplier = 1;
                double defenderMultiplier = 1;

                // Adjust multipliers based on the combination of elements and types
                if (attacker.Element == "Fire" && defender.Element == "Water")
                {
                    attackerMultiplier = attacker.Type == "Spell" ? 1 : 2;
                    defenderMultiplier = defender.Type == "Monster" ? 2 : 1;
                }
                else if (attacker.Element == "Water" && defender.Element == "Water")
                {
                    attackerMultiplier = 1;
                    defenderMultiplier = 1; // Equal elements result in no change
                }
                else if (attacker.Element == "Regular" && defender.Element == "Water")
                {
                    attackerMultiplier = 2; // Regular spell gets a boost against Water element
                    defenderMultiplier = attacker.Type == "Spell" ? 1 : 2;
                }
                else if (attacker.Element == "Regular" && defender.CreatureName == "Knight")
                {
                    attackerMultiplier = 1; // Regular spell against Knight
                    defenderMultiplier = 1; // Knight retains its damage
                }

                double adjustedAttackerDamage = damage * attackerMultiplier;
                double adjustedDefenderDamage = defender.Damage * defenderMultiplier;

                // Determine the outcome
                if (adjustedAttackerDamage > adjustedDefenderDamage)
                {
                    damage = adjustedAttackerDamage; // Attacker wins
                }
                else if (adjustedAttackerDamage < adjustedDefenderDamage)
                {
                    damage = 0; // Attacker loses
                }
                else
                {
                    // Draw, no action
                    damage = 0;
                    defender.Damage = 0; // Set defender's damage to 0 to indicate a draw
                }
            }

            return damage;
        }

        private void TransferCard(User winner, User loser, Card card)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                int loserId = GetUserIdByUsername(loser.Username);
                int winnerId = GetUserIdByUsername(winner.Username);

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                    string addToWinnerStackSql = @"
                    UPDATE UserCards 
                    SET UserId = @WinnerId  
                    WHERE CardId = @CardId"; // Adjust based on your schema
                        using (var cmd = new NpgsqlCommand(addToWinnerStackSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@WinnerId", winnerId);
                            cmd.Parameters.AddWithValue("@CardId", card.CardID);
                            cmd.ExecuteNonQuery();
                        }

                        // SQL to remove card from loser's UserDeck
                        string removeFromLoserDeckSql = @"
                UPDATE UserDeck 
                SET Card1 = CASE WHEN Card1 = @CardId THEN NULL ELSE Card1 END,
                    Card2 = CASE WHEN Card2 = @CardId THEN NULL ELSE Card2 END,
                    Card3 = CASE WHEN Card3 = @CardId THEN NULL ELSE Card3 END,
                    Card4 = CASE WHEN Card4 = @CardId THEN NULL ELSE Card4 END
                WHERE UserID = @LoserId";
                        using (var cmd = new NpgsqlCommand(removeFromLoserDeckSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CardId", card.CardID);
                            cmd.Parameters.AddWithValue("@LoserId", loserId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions (e.g., log the error)
                        transaction.Rollback();
                    }
                }
               
                loser.Deck = GetDeckForUser(loserId, conn);
            }
            
           
            // Update the in-memory objects
         //   loser.Deck.Remove(card);
         //   winner.Stack.Add(card);
        }
        /// <summary>

        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <returns></returns>


        private void UpdateStats()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Determine the winner and loser
                        bool isPlayer1Winner = player1.Deck.Count > 0 && player2.Deck.Count == 0;
                        bool isPlayer2Winner = player2.Deck.Count > 0 && player1.Deck.Count == 0;

                        // Increment the number of games played for both users
                        player1.gamesPlayed++;
                        player2.gamesPlayed++;

                        if (isPlayer1Winner)
                        {
                            // Player 1 wins
                            player1.Elo += 3;
                            player1.gamesWon++;
                            player2.Elo -= 5;
                            player2.gamesLost++;
                            player1.Coins += 1; // Add a coin to the winner
                        }
                        else if (isPlayer2Winner)
                        {
                            // Player 2 wins
                            player2.Elo += 3;
                            player2.gamesWon++;
                            player1.Elo -= 5;
                            player1.gamesLost++;
                            player2.Coins += 1; // Add a coin to the winner
                        }

                        // Update stats for both players
                        UpdateUserStats(conn, player1, isPlayer1Winner);
                        UpdateUserStats(conn, player2, isPlayer2Winner);

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions (e.g., log the error)
                        transaction.Rollback();
                        throw; // Re-throw the exception after rollback
                    }
                }
            }
        }

        private void UpdateUserStats(NpgsqlConnection conn, User player, bool isWinner)
        {
            string sql = @"
        UPDATE Users 
        SET Elo = @Elo, 
            GamesPlayed = GamesPlayed + 1, 
            GamesWon = GamesWon + @GamesWon, 
            GamesLost = GamesLost + @GamesLost,
            Coins = Coins + @CoinIncrement
        WHERE Username = @Username";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Elo", player.Elo);
                cmd.Parameters.AddWithValue("@GamesWon", player.gamesWon);
                cmd.Parameters.AddWithValue("@GamesLost", player.gamesLost);
                cmd.Parameters.AddWithValue("@CoinIncrement", isWinner ? 1 : 0); // Increment coins by 1 for the winner
                cmd.Parameters.AddWithValue("@Username", player.Username);
                cmd.ExecuteNonQuery();
            }
        }

        private void PrintBattleLog()
        {
            foreach (string logEntry in battleLog)
            {
                Console.WriteLine(logEntry);
            }
        }
    }
}
