using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.cards
{
    public class Card
{
    // Adding CardID property
    public int CardID { get; private set; }

    // Other properties...
    public string Type { get; set; }
    public string CreatureName { get; set; }
    public string Element { get; set; }
    public string CurlId { get; set; }
    public double Damage { get; set; }
    public string CardName { get; set; }

    // Constructor
    public Card(int cardId, string type, string creatureName, string element, string id, double dmg, string cardname)
    {
        CardID = cardId;
        Type = type ?? "";
        CreatureName = creatureName ?? "";
        Element = element ?? "";
        CurlId = id ?? "";
        Damage = dmg;
        CardName = cardname ?? "";
    }

    public string CardInfo()
    {
        return $"[{Type}] {CardName} (ID: {CurlId}, Damage: {Damage})";
    }

    // Method to set CardID - if necessary
    public void SetCardID(int cardId)
    {
        CardID = cardId;
    }
}

}
