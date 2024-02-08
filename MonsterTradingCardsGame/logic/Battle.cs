using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.logic
{
    class Battle
    {
        private User player1;
        private User player2;
        private const int MaxRounds = 100;
        private List<string> battleLog;

        public Battle(User player1, User player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            battleLog = new List<string>();
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

        private void ConductRound()
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
            int card1Damage = CalculateDamage(card1, card2);
            int card2Damage = CalculateDamage(card2, card1);

            // Determine round outcome
            string roundResult;
            if (card1Damage > card2Damage)
            {
                roundResult = $"Player 1's {card1} defeats Player 2's {card2}";
                TransferCard(player1, player2, card2); // Transfer card2 from player2 to player1
            }
            else if (card2Damage > card1Damage)
            {
                roundResult = $"Player 2's {card2} defeats Player 1's {card1}";
                TransferCard(player2, player1, card1); // Transfer card1 from player1 to player2
            }
            else
            {
                roundResult = "The round is a draw.";
            }

            // Add the round result to the battle log
            battleLog.Add(roundResult);
        }

        private int CalculateDamage(Card attacker, Card defender)
        {
            // Start with the base damage of the attacker
            int damage = attacker.Damage;

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
            else if (attacker.CreatureName == "WaterSpell" && defender.CreatureName == "Knight")
            {
                // The armor of Knights is so heavy that WaterSpells make them drown instantly
                damage = int.MaxValue; // Assign a very high value to ensure defeat
            }
            else if (attacker.Type == "Spell" && defender.CreatureName == "Kraken")
            {
                // The Kraken is immune against spells
                damage = 0;
            }
            else if (attacker.CreatureName == "FireElf" && defender.CreatureName == "Dragon")
            {
                // FireElves can evade Dragon attacks
                damage = 0;
            }

            return damage;
        }

        private void TransferCard(User winner, User loser, Card card)
        {
            // Remove the card from the loser's deck and add it to the winner's deck
            loser.Deck.Remove(card);
            winner.Deck.Add(card);
        }
        /// <summary>

        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <returns></returns>
        private string ResolveBattle(Card card1, Card card2)
        {
            // Calculate damage considering element type and special rules
            int card1Damage = CalculateDamage(card1, card2);
            int card2Damage = CalculateDamage(card2, card1);

            // Determine the outcome of the battle
            string roundResult;
            if (card1Damage > card2Damage)
            {
                roundResult = $"Player 1's {card1.CardName} (Damage: {card1Damage}) defeats Player 2's {card2.CardName} (Damage: {card2Damage})";
                TransferCard(player1, player2, card2); // Transfer card2 from player2 to player1
            }
            else if (card2Damage > card1Damage)
            {
                roundResult = $"Player 2's {card2.CardName} (Damage: {card2Damage}) defeats Player 1's {card1.CardName} (Damage: {card1Damage})";
                TransferCard(player2, player1, card1); // Transfer card1 from player1 to player2
            }
            else
            {
                roundResult = "The round is a draw.";
            }

            return roundResult;
        }

        private void UpdateDecks(Card card1, Card card2, string roundResult)
        {
            // Determine the winner and loser based on the round result
            if (roundResult.Contains("defeats"))
            {
                User winner, loser;
                Card cardToTransfer;

                if (roundResult.Contains("Player 1's"))
                {
                    winner = player1;
                    loser = player2;
                    cardToTransfer = card2;
                }
                else
                {
                    winner = player2;
                    loser = player1;
                    cardToTransfer = card1;
                }

                // Perform the transfer of the card
                TransferCard(winner, loser, cardToTransfer);
            }
            // In case of a draw, no action is needed
        }

        private void UpdateStats()
        {
            // Increment the number of games played for both users
            player1.gamesPlayed++;
            player2.gamesPlayed++;

            // Determine the winner and update ELO ratings
            // This is a simplistic ELO update logic, you might want to use a more complex formula
            int winEloChange = 3; // The value of ELO points to be exchanged, this can be adjusted
            int loseEloChange = 1; // The value of ELO points to be exchanged, this can be adjusted
            if (player1.Deck.Count > 0 && player2.Deck.Count == 0)
            {
                // Player 1 wins
                player1.Elo += winEloChange;
                player2.Elo -= loseEloChange;
            }
            else if (player2.Deck.Count > 0 && player1.Deck.Count == 0)
            {
                // Player 2 wins
                player2.Elo += winEloChange;
                player1.Elo -= loseEloChange;
            }
            // In case of a draw, you might want to update the stats differently
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
