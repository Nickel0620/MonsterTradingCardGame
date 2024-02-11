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

namespace MonsterTradingCardsGame.logic
{
    public class Battle
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

        public int CalculateDamage(Card attacker, Card defender)
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
            else if (attacker.Element + attacker.CreatureName == "WaterSpell" && defender.CreatureName == "Knight")
            {
                // The armor of Knights is so heavy that WaterSpells make them drown instantly
                damage = int.MaxValue; // Assign a very high value to ensure defeat
            }
            else if (attacker.Type == "Spell" && defender.CreatureName == "Kraken")
            {
                // The Kraken is immune against spells
                damage = 0;
            }
            else if (attacker.Element + attacker.CreatureName == "FireElf" && defender.CreatureName == "Dragon")
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
                int attackerMultiplier = 1;
                int defenderMultiplier = 1;

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

                int adjustedAttackerDamage = damage * attackerMultiplier;
                int adjustedDefenderDamage = defender.Damage * defenderMultiplier;

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
                int attackerMultiplier = 1;
                int defenderMultiplier = 1;

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

                int adjustedAttackerDamage = damage * attackerMultiplier;
                int adjustedDefenderDamage = defender.Damage * defenderMultiplier;

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
            // Remove the card from the loser's deck and add it to the winner's deck
            loser.Deck.Remove(card);
            winner.Stack.Add(card);
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
