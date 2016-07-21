using System;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class Player// : IPerson
    {
        private string _fullName;


        public string ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_fullName))
                    return _fullName;
                return string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            set { _fullName = value; }
        }

        public string DisplayName { get; set; }

        public int Age { get; set; }

        public Rating Rating { get; set; }

        public Team Team { get; set; }

        public PlayerStatistics Statistics { get; set; }

        public FantasyPlayer Fantasy { get; set; }


        public override string ToString()
        {
            return DisplayName;
        }
    }
}
