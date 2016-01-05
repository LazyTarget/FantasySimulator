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
            if (communityCards != null)
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


        public virtual IEnumerable<PossibleHand> CalculatePossibleHands(Player player, IList<Card> communityCards, PokerGame game)
        {
            var cards = new List<Card>();
            cards.AddRange(player.Cards.Where(x => x != null));
            if (communityCards != null)
                cards.AddRange(communityCards.Where(x => x != null));

            var suitCounts = cards.GroupBy(x => x.Suit).OrderByDescending(x => x.Count()).ToList();
            var denomCounts = cards.GroupBy(x => x.Denomination).OrderByDescending(x => x.Count()).ThenByDescending(x => (int)x.Key).ToList();
            var denomList = cards.OrderByDescending(x => (int)x.Denomination).ToList();
            
            var otherPlayers = game.Players.Where(x => x != player).ToList();


            var strengths = Enum.GetValues(typeof (HandStrength))
                                .Cast<HandStrength>()
                                .OrderByDescending(x => (int) x)
                                .ToList();
            foreach (var handStrength in strengths)
            {
                switch (handStrength)
                {
                    case HandStrength.FourOfAKind:
                    case HandStrength.ThreeOfAKind:
                    case HandStrength.OnePair:
                        var cardsNeeded = handStrength == HandStrength.FourOfAKind
                            ? 4
                            : handStrength == HandStrength.ThreeOfAKind
                                ? 3
                                : 2;

                        var dcl = denomCounts.Where(x => x.Count() >= 1).ToList();
                        if (dcl.Any())
                        {
                            foreach (var dc in dcl)
                            {
                                var usedCards = new List<Card>();
                                usedCards.AddRange(dc);

                                var kickers = denomList.Where(x => !usedCards.Contains(x)).Take(1);
                                usedCards.AddRange(kickers);

                                var pairStrength = dc.Count();
                                var remainingCardsNeeded = cardsNeeded - pairStrength;
                                var remainingCards = Card.GetByDenominator(dc.Key).Where(x => dc.All(y => y.Suit != x.Suit)).ToList();
                                double probability;
                                if (remainingCardsNeeded <= 0)
                                {
                                    probability = 1.0d;
                                }
                                else
                                {
                                    var otherPlayerMatchingCards = otherPlayers.SelectMany(x => x.Cards).Where(x => x.Denomination == dc.Key).ToList();
                                    var deadCards = pairStrength + otherPlayerMatchingCards.Count;
                                    probability = CalculateOutsProbability(remainingCards.Count, game.CardsInDeck, remainingCardsNeeded, deadCards);
                                }
                                //var probability = remainingCardsNeeded > 0
                                //    ? CalculateOutsProbability(remainingCards.Count, pairStrength)
                                //    : 1.0d;

                                // todo: 
                                var combinations = new List<Hand>();
                                //for(var temp = 0; temp < remainingCards.Count; temp++)
                                //{
                                //    var hand = new Hand
                                //    {
                                //        Cards = dc.ToArray(),
                                //        Strength = handStrength,
                                //    };
                                //    combinations.Add(hand);
                                //    for (var x = 0; x < remainingCardsNeeded; x++)
                                //    {
                                //        for (var y = 0; y < remainingCards.Count; y++)
                                //        {
                                //            var c = remainingCards[y];
                                //            hand.Cards = hand.Cards.Concat(new[] {c}).ToArray();
                                //        }
                                //    }
                                //}

                                yield return new PossibleHand
                                {
                                    Cards = usedCards,
                                    Player = player,
                                    Strength = handStrength,
                                    Outs = remainingCards.Count,
                                    Probability = probability,
                                    Combinations = combinations,
                                };
                            }
                        }
                        break;
                }
            }
        }


        public virtual double CalculateOutsProbability(int outs, int cardsInDeck, int cardsToBeDrawn, int numberOfDeadCards)
        {
            if (outs == 0)
                return 1.0d;        // has hit the out
            if (outs < 0)
                return 0.0d;        // has no outs to improve the hand
            double cards = (cardsInDeck - numberOfDeadCards) * cardsToBeDrawn;
            if (cards <= 0)
                return 0.0d;        // no cards left
            var perc = outs / cards;
            return perc;
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
