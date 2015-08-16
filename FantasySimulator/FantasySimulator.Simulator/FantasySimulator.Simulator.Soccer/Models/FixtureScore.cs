using System;

namespace FantasySimulator.Simulator.Soccer
{
    public struct FixtureScore
    {
        private int _goalsForHomeTeam;
        private int _goalsForAwayTeam;


        public int GoalsForHomeTeam
        {
            get { return Math.Max(0, _goalsForHomeTeam); }
            set { _goalsForHomeTeam = value; }
        }

        public int GoalsForAwayTeam
        {
            get { return Math.Max(0, _goalsForAwayTeam); }
            set { _goalsForAwayTeam = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", GoalsForHomeTeam, GoalsForAwayTeam);
        }
    }
}
