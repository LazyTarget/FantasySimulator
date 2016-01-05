using System;

namespace FantasySimulator.Simulator.Poker
{
    public enum PokerType
    {
        None,
        TexasHoldEm,
        Omaha,
    }

    public enum Suits
    {
        None,
        Hearts,
        Spades,
        Diamonds,
        Clubs,
        Joker,
    }

    [Flags]
    public enum BlindType
    {
        None        = 0,
        Dealer      = 1 << 0,
        BigBlind    = 1 << 1,
        SmallBlind  = 1 << 2,
    }

    public enum CardDenomination
    {
        None,
        Duece = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
        Joker = 15,
    }

    public enum HandStrength
    {
        None,
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalStraightFlush,
    }
}
