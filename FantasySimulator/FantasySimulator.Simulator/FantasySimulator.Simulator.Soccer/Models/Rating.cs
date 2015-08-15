using System;
using System.ComponentModel.DataAnnotations;

namespace FantasySimulator.Simulator.Soccer.Models
{
    public struct Rating
    {
        /// <summary>
        /// A value between 1 and 10
        /// </summary>
        [Range(1, 10)]
        public int Value { get; set; }

        

        public static explicit operator Rating(int v)
        {
            var rating = new Rating();
            rating.Value = v;
            return rating;
        }


        public static bool operator ==(Rating a, Rating b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Rating a, Rating b)
        {
            return !(a == b);
        }

        public static bool operator >(Rating a, Rating b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Rating a, Rating b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(Rating a, Rating b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Rating a, Rating b)
        {
            return a.Value <= b.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
