using System;

namespace FantasySimulator.Simulator.Soccer
{
    public struct FixtureScore
    {
        public int GoalsForHomeTeam { get; private set; }

        public int GoalsForAwayTeam { get; private set; }

        public int PointsForHomeTeam { get; private set; }

        public int PointsForAwayTeam { get; private set; }

        public FixtureOutcome Outcome { get; private set; }


        public override string ToString()
        {
            return string.Format("{0}-{1}", GoalsForHomeTeam, GoalsForAwayTeam);
        }


        public static FixtureScore Create(int goalsForHomeTeam, int goalsForAwayTeam)
        {
            goalsForHomeTeam = Math.Max(0, goalsForHomeTeam);
            goalsForAwayTeam = Math.Max(0, goalsForAwayTeam);

            var draw = goalsForHomeTeam == goalsForAwayTeam;
            var homeWin = goalsForHomeTeam > goalsForAwayTeam;
            var awayWin = goalsForHomeTeam < goalsForAwayTeam;
            var ptsForHome = draw ? 1 : homeWin ? 3 : 0;
            var ptsForAway = draw ? 1 : awayWin ? 3 : 0;
            var outcome = draw
                ? FixtureOutcome.Draw
                : homeWin
                    ? FixtureOutcome.HomeTeam
                    : awayWin
                        ? FixtureOutcome.AwayTeam
                        : FixtureOutcome.Undetermined;

            var score = Create(goalsForHomeTeam, goalsForAwayTeam, ptsForHome, ptsForAway, outcome);
            return score;
        }


        public static FixtureScore Create(int goalsForHomeTeam, int goalsForAwayTeam, int? pointsForHomeTeam, int? pointsForAwayTeam)
        {
            goalsForHomeTeam = Math.Max(0, goalsForHomeTeam);
            goalsForAwayTeam = Math.Max(0, goalsForAwayTeam);

            var draw = goalsForHomeTeam == goalsForAwayTeam;
            var homeWin = goalsForHomeTeam > goalsForAwayTeam;
            var awayWin = goalsForHomeTeam < goalsForAwayTeam;
            var outcome = draw
                ? FixtureOutcome.Draw
                : homeWin
                    ? FixtureOutcome.HomeTeam
                    : awayWin
                        ? FixtureOutcome.AwayTeam
                        : FixtureOutcome.Undetermined;

            var score = Create(goalsForHomeTeam, goalsForAwayTeam, pointsForHomeTeam, pointsForAwayTeam, outcome);
            return score;
        }

        public static FixtureScore Create(int goalsForHomeTeam, int goalsForAwayTeam, int? pointsForHomeTeam, int? pointsForAwayTeam, FixtureOutcome fixtureOutcome)
        {
            goalsForHomeTeam = Math.Max(0, goalsForHomeTeam);
            goalsForAwayTeam = Math.Max(0, goalsForAwayTeam);


            if (!pointsForHomeTeam.HasValue && !pointsForAwayTeam.HasValue)
            {
                switch (fixtureOutcome)
                {
                    case FixtureOutcome.HomeTeam:
                        pointsForHomeTeam = 3;
                        pointsForAwayTeam = 1;
                        break;
                    case FixtureOutcome.AwayTeam:
                        pointsForHomeTeam = 1;
                        pointsForAwayTeam = 3;
                        break;
                    case FixtureOutcome.Draw:
                        pointsForHomeTeam = 1;
                        pointsForAwayTeam = 1;
                        break;
                }
            }

            var score = new FixtureScore
            {
                GoalsForHomeTeam = goalsForHomeTeam,
                GoalsForAwayTeam = goalsForAwayTeam,
                PointsForHomeTeam = pointsForHomeTeam ?? 0,
                PointsForAwayTeam = pointsForAwayTeam ?? 0,
                Outcome = fixtureOutcome,
            };
            return score;
        }
    }
}
