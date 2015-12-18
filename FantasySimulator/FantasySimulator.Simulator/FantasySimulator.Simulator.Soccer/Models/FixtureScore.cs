using System;

namespace FantasySimulator.Simulator.Soccer
{
    public struct FixtureScore
    {
        private int _goalsForHomeTeam;
        private int _goalsForAwayTeam;


        public int GoalsForHomeTeam
        {
            get { return _goalsForHomeTeam; }
            private set { _goalsForHomeTeam = Math.Max(0, value); }
        }

        public int GoalsForAwayTeam
        {
            get { return _goalsForAwayTeam; }
            private set { _goalsForAwayTeam = Math.Max(0, value); }
        }

        public int PointsForHomeTeam { get; private set; }

        public int PointsForAwayTeam { get; private set; }


        public override string ToString()
        {
            return string.Format("{0}-{1}", GoalsForHomeTeam, GoalsForAwayTeam);
        }


        public static FixtureScore Create(int goalsForHomeTeam, int goalsForAwayTeam)
        {
            var score = new FixtureScore
            {
                GoalsForHomeTeam = goalsForHomeTeam,
                GoalsForAwayTeam = goalsForAwayTeam,
            };
            
            score.PointsForHomeTeam = score.GoalsForHomeTeam == score.GoalsForAwayTeam
                ? 1
                : score.GoalsForHomeTeam > score.GoalsForAwayTeam
                    ? 3
                    : 0;
            score.PointsForAwayTeam = score.GoalsForHomeTeam == score.GoalsForAwayTeam
                ? 1
                : score.GoalsForHomeTeam > score.GoalsForAwayTeam
                    ? 0
                    : 3;
            return score;
        }

        public static FixtureScore Create(int goalsForHomeTeam, int goalsForAwayTeam, int pointsForHomeTeam, int pointsForAwayTeam)
        {
            var score = new FixtureScore
            {
                GoalsForHomeTeam = goalsForHomeTeam,
                GoalsForAwayTeam = goalsForAwayTeam,
                PointsForHomeTeam = pointsForHomeTeam,
                PointsForAwayTeam = pointsForAwayTeam,
            };
            return score;
        }
    }
}
