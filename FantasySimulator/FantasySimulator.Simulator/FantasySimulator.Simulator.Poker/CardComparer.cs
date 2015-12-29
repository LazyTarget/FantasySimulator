using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class CardComparer : IComparer<Card>, IComparer<Hand>
    {
        public int Compare(Card x, Card y)
        {
            if (x == y)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            var c = ((int)x.Denomination).CompareTo((int)y.Denomination);
            return c;
        }

        public int Compare(Hand x, Hand y)
        {
            if (x == y)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            var c = ((int)x.Strength).CompareTo((int)y.Strength);
            if (c != 0)
                return c;

            //var kickersX = x.GetKickers().GetEnumerator();
            //var kickersY = y.GetKickers().GetEnumerator();
            var kickersX = x.Cards.OrderByDescending(k => k, new CardComparer()).GetEnumerator();
            var kickersY = y.Cards.OrderByDescending(k => k, new CardComparer()).GetEnumerator();

            while (kickersX.MoveNext() &&
                   kickersY.MoveNext())
            {
                var kickX = kickersX.Current;
                var kickY = kickersY.Current;
                c = Compare(kickX, kickY);
                if (c != 0)
                    return c;
            }
            return 0;
        }
    }
}