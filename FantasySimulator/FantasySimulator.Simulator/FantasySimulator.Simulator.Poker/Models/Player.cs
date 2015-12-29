using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Poker.Models
{
    public class Player : IXmlConfigurable
    {
        public Player()
        {
            Cards = new List<Card>();
        }

        public string Name { get; set; }

        public BlindType BlindType { get; set; }

        public IList<Card> Cards { get; set; }


        public void Configure(XElement element)
        {
            Name = element.GetAttributeValue("name");
            BlindType = element.Element("blindType")?.GetAttributeValue("value").SafeConvert<BlindType>() ?? BlindType.None;
            
            var cardsElem = element.Element("cards");
            if (cardsElem != null)
            {
                var cardElems = cardsElem.Elements("card").Where(x => x != null).ToList();
                if (cardElems.Any())
                {
                    Cards = new List<Card>();
                    foreach (var cardElem in cardElems)
                    {
                        var card = new Card();
                        card.Configure(cardElem);
                        Cards.Add(card);
                    }
                }
            }
        }

    }
}
