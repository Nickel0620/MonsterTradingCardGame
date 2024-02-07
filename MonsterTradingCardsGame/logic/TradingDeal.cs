using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.logic
{
    public class TradingDeal
    {
        public Card OfferedCard { get; private set; }
        public string DesiredType { get; private set; }
        public int MinimumDamage { get; private set; }
        public User OfferedBy { get; private set; }

        public TradingDeal(User offeredBy, Card offeredCard, string desiredType, int minimumDamage)
        {
            if (offeredBy.Deck.Contains(offeredCard))
            {
                throw new InvalidOperationException("The card is currently in the deck and cannot be offered for trade.");
            }

            OfferedCard = offeredCard;
            DesiredType = desiredType;
            MinimumDamage = minimumDamage;
            OfferedBy = offeredBy;

            // Lock the card for deck usage
            offeredBy.Stack.Remove(offeredCard);
        }

        // Method to accept a trade
        public bool AcceptTrade(User acceptingUser, Card offeredInExchange)
        {
            if (offeredInExchange.Type != DesiredType || offeredInExchange.Damage < MinimumDamage)
            {
                Console.WriteLine("The offered card does not meet the trade requirements.");
                return false;
            }

            // Exchange the cards
            OfferedBy.Stack.Add(offeredInExchange);
            acceptingUser.Stack.Remove(offeredInExchange);
            acceptingUser.Stack.Add(OfferedCard);

            Console.WriteLine("Trade accepted and completed.");
            return true;
        }
    }
}
