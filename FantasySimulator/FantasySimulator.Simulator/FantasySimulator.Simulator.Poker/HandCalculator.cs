using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class HandCalculator
    {
        public HandCalculator()
        {
            CardComparer = new CardComparer();
        }


        public IComparer<Card> CardComparer { get; protected set; }


        public virtual IEnumerable<PlayerHand> CalculateAvailableHands(Player player, IList<Card> communityCards)
        {
            var cards = new List<Card>();
            cards.AddRange(player.Cards.Where(x => x != null));
            cards.AddRange(communityCards.Where(x => x != null));

            var suitCounts = cards.GroupBy(x => x.Suit).OrderByDescending(x => x.Count()).ToList();
            var denomCounts = cards.GroupBy(x => x.Denomination).OrderByDescending(x => x.Count()).ThenByDescending(x => (int) x.Key).ToList();
            var denomList = cards.OrderByDescending(x => (int) x.Denomination).ToList();


            // Royal Straight Flush
            // todo:


            // Straight Flush
            // todo:


            // Four of a Kind
            var dcl = denomCounts.Where(x => x.Count() >= 4).ToList();
            if (dcl.Any())
            {
                foreach (var quads in dcl)
                {
                    var usedCards = new List<Card>();
                    usedCards.AddRange(quads);

                    var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(1);
                    usedCards.AddRange(kickers);

                    yield return new PlayerHand
                    {
                        Cards = usedCards,
                        Player = player,
                        Strength = HandStrength.FourOfAKind,
                    };
                }
            }


            // Full House
            dcl = denomCounts.Where(x => x.Count() >= 3).ToList();
            if (dcl.Any())
            {
                foreach (var trips in dcl)
                {
                    var pairsDcl = denomCounts.Where(x => x.Count() >= 2 && x.Key != trips.Key).ToList();
                    if (pairsDcl.Any())
                    {
                        foreach (var pair in pairsDcl)
                        {
                            var usedCards = new List<Card>();
                            usedCards.AddRange(trips);
                            usedCards.AddRange(pair);

                            var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(1);
                            usedCards.AddRange(kickers);

                            yield return new PlayerHand
                            {
                                Cards = usedCards,
                                Player = player,
                                Strength = HandStrength.FullHouse,
                            };
                        }
                    }
                    else
                    {
                        
                    }
                }
            }


            // Flush
            var scl = suitCounts.Where(x => x.Count() >= 5).ToList();
            if (scl.Any())
            {
                foreach (var flush in scl)
                {
                    var usedCards = new List<Card>();
                    usedCards.AddRange(flush);

                    //var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(1);
                    //usedCards.AddRange(kickers);

                    yield return new PlayerHand
                    {
                        Cards = usedCards,
                        Player = player,
                        Strength = HandStrength.Flush,
                    };
                }
            }


            // Straight
            // todo:


            // Tree of a Kind
            dcl = denomCounts.Where(x => x.Count() >= 3).ToList();
            if (dcl.Any())
            {
                foreach (var trips in dcl)
                {
                    var usedCards = new List<Card>();
                    usedCards.AddRange(trips);

                    var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(2);
                    usedCards.AddRange(kickers);

                    yield return new PlayerHand
                    {
                        Cards = usedCards,
                        Player = player,
                        Strength = HandStrength.ThreeOfAKind,
                    };
                }
            }


            // Two pair
            dcl = denomCounts.Where(x => x.Count() >= 2).ToList();
            if (dcl.Count() >= 2)
            {
                var firstPair = dcl.ElementAt(0);
                var secondPair = dcl.ElementAt(1);

                var usedCards = new List<Card>();
                usedCards.AddRange(firstPair);
                usedCards.AddRange(secondPair);
                
                var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(1);
                usedCards.AddRange(kickers);

                yield return new PlayerHand
                {
                    Cards = usedCards,
                    Player = player,
                    Strength = HandStrength.TwoPair,
                };
            }


            // One pair
            dcl = denomCounts.Where(x => x.Count() >= 2).ToList();
            if (dcl.Any())
            {
                foreach (var pair in dcl)
                {
                    var usedCards = new List<Card>();
                    usedCards.AddRange(pair);

                    var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(3);
                    usedCards.AddRange(kickers);

                    yield return new PlayerHand
                    {
                        Cards = usedCards,
                        Player = player,
                        Strength = HandStrength.OnePair,
                    };
                }
            }


            // High card
            var highCards = denomList.Take(5).ToArray();
            yield return new PlayerHand
            {
                Cards = highCards,
                Player = player,
                Strength = HandStrength.HighCard,
            };
        }

        public virtual IEnumerable<PossibleHand> CalculatePossibleHands(Player player, IList<Card> communityCards)
        {
            yield break;
        }

        
        public virtual PlayerHand GetBestHand(IList<PlayerHand> hands)
        {
            //var hand = hands.OrderByDescending(x => (int)x.Strength).FirstOrDefault();

            // order by best kicker
            var hand = hands.OrderByDescending(x => (int) x.Strength)
                .ThenByDescending(
                    x => x.Cards.OrderByDescending(c => (int) c.Denomination).Take(5).Sum(c => (int) c.Denomination))
                .FirstOrDefault();
            return hand;
        }
    }
}
