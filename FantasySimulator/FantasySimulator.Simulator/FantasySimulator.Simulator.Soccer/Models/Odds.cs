using System;
using System.ComponentModel.DataAnnotations;

namespace FantasySimulator.Simulator.Soccer
{
    public struct Odds
    {
        [Range(0, Int32.MaxValue)]
        public double Value { get; set; }



        public static implicit operator Odds(int v)
        {
            var rating = new Odds();
            rating.Value = v;
            return rating;
        }


        public static bool operator ==(Odds a, Odds b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Odds a, Odds b)
        {
            return !(a == b);
        }

        public static bool operator >(Odds a, Odds b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Odds a, Odds b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(Odds a, Odds b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Odds a, Odds b)
        {
            return a.Value <= b.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
