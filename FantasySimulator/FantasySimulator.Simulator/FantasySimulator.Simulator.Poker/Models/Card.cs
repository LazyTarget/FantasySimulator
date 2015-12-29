using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Poker.Models
{
    public class Card : IXmlConfigurable
    {
        public CardDenomination Denomination { get; set; }

        public Suits Suit { get; set; }

        public bool Valid
        {
            get { return Denomination != CardDenomination.None && Suit != Suits.None; }
        }

        public override string ToString()
        {
            return String.Format("{0}{1}", (int) Denomination, Suit);
        }

        public void Configure(XElement element)
        {
            var value = element.GetAttributeValue("value");
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.ToUpper();
                if (value == "J" || value == "JJ")
                {
                    Suit = Suits.Joker;
                    Denomination = CardDenomination.Joker;
                }
                else
                {
                    var digits = value.Where(char.IsDigit).ToList();
                    var denomValStr = string.Join("", digits);
                    CardDenomination denom;
                    Enum.TryParse(denomValStr, out denom);
                    Denomination = denom;

                    var suitStr = value.Substring(digits.Count);
                    if (suitStr == "H" || suitStr == Suits.Hearts.ToString().ToUpper())
                        Suit = Suits.Hearts;
                    else if (suitStr == "S" || suitStr == Suits.Spades.ToString().ToUpper())
                        Suit = Suits.Spades;
                    else if (suitStr == "D" || suitStr == Suits.Diamonds.ToString().ToUpper())
                        Suit = Suits.Diamonds;
                    else if (suitStr == "C" || suitStr == Suits.Clubs.ToString().ToUpper())
                        Suit = Suits.Clubs;
                }
            }
            else
            {
                Suit = Suits.None;
                Denomination = CardDenomination.None;
            }
        }
    }
}
