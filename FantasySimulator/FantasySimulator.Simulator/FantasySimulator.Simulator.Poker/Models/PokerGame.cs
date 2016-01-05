using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Poker.Models
{
    public class PokerGame : IXmlConfigurable
    {
        public readonly int NUMBER_OF_CARDS_IN_TEXAS_HOLD_EM = 14 * 4;


        public PokerGame()
        {
            CommunityCards = new List<Card>();
            Players = new List<Player>();
        }

        public PokerType PokerType { get; set; }

        public double SmallBlind { get; set; }

        public double BigBlind { get; set; }

        public IList<Card> CommunityCards { get; set; } 

        public IList<Player> Players { get; set; }



        public int CardsInDeck
        {
            get { return NUMBER_OF_CARDS_IN_TEXAS_HOLD_EM - KnownCards; }
        }

        public int KnownCards
        {
            get { return CommunityCards?.Count ?? 0 + Players?.Sum(x => x.Cards.Count) ?? 0; }
        }

        public void Configure(XElement element)
        {
            PokerType = element.GetAttributeValue("type").SafeConvert<PokerType>();

            var blindsElem = element.Element("blinds");
            if (blindsElem != null)
            {
                SmallBlind = blindsElem.Element("smallBlind")?.GetAttributeValue("value").SafeConvert<double>() ?? 0;
                BigBlind = blindsElem.Element("bigBlind")?.GetAttributeValue("value").SafeConvert<double>() ?? 0;
            }


            var communityCardsElem = element.Element("communityCards");
            if (communityCardsElem != null)
            {
                var cardElems = communityCardsElem.Elements("card").Where(x => x != null).ToList();
                if (cardElems.Any())
                {
                    CommunityCards = new List<Card>();
                    foreach (var cardElem in cardElems)
                    {
                        var card = new Card();
                        card.Configure(cardElem);
                        CommunityCards.Add(card);
                    }
                }
            }


            var playersElem = element.Element("players");
            if (playersElem != null)
            {
                var playerElems = playersElem.Elements("player").Where(x => x != null).ToList();
                if (playerElems.Any())
                {
                    Players = new List<Player>();
                    foreach (var playerElem in playerElems)
                    {
                        var player = new Player();
                        player.Configure(playerElem);
                        Players.Add(player);
                    }
                }
            }

        }
    }
}
