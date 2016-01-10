using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Poker.Models;
using NUnit.Framework;

namespace FantasySimulator.Simulator.Poker.Tests
{
    [TestFixture]
    public class HandCalculatorTests
    {
        private HandCalculator GetSUT()
        {
            return new HandCalculator();
        }


        [TestCase]
        public void CalculatePossibleHands_PocketAces()
        {
            var sut = GetSUT();
            
            var player = new Player();
            player.Cards = new List<Card>();
            player.Cards.Add(new Card(Suits.Hearts, CardDenomination.Ace));
            player.Cards.Add(new Card(Suits.Clubs, CardDenomination.Ace));

            var game = new PokerGame();
            game.Players = new List<Player>
            {
                player,
            };
            game.PokerType = PokerType.TexasHoldEm;
            game.CommunityCards = null;


            var actual = sut.CalculatePossibleHands(player, game.CommunityCards, game).ToList();
            Assert.IsTrue(actual.Count > 0);
            Assert.AreEqual(1, actual.Count(x => x.Strength == HandStrength.FourOfAKind));
            Assert.AreEqual(1, actual.Count(x => x.Strength == HandStrength.ThreeOfAKind));
            Assert.AreEqual(1, actual.Count(x => x.Strength == HandStrength.OnePair));

            foreach (var possibleHand in actual)
            {
                if (possibleHand.Strength == HandStrength.FourOfAKind)
                {
                    var deadCards = 2;
                    var expectedOuts = 2;
                    var expectedProbability = sut.CalculateOutsProbability(expectedOuts, game.CardsInDeck, 2, deadCards);
                    Assert.AreEqual(expectedOuts, possibleHand.Outs);
                    Assert.AreEqual(expectedProbability, possibleHand.Probability);
                }
                if (possibleHand.Strength == HandStrength.ThreeOfAKind)
                {
                    var deadCards = 2;
                    var expectedOuts = 2;
                    var expectedProbability = sut.CalculateOutsProbability(expectedOuts, game.CardsInDeck, 1, deadCards);
                    Assert.AreEqual(expectedOuts, possibleHand.Outs);
                    Assert.AreEqual(expectedProbability, possibleHand.Probability);
                }
                //if (possibleHand.Strength == HandStrength.OnePair)
                //{
                //    var deadCards = 2;
                //    var expectedOuts = 0;
                //    var expectedProbability = 1d;
                //    //var expectedProbability = sut.CalculateOutsProbability(expectedOuts, game.CardsInDeck, 1, deadCards);
                //    Assert.AreEqual(expectedOuts, possibleHand.Outs);
                //    Assert.AreEqual(expectedProbability, possibleHand.Probability);
                //}
            }
        }

    }
}
