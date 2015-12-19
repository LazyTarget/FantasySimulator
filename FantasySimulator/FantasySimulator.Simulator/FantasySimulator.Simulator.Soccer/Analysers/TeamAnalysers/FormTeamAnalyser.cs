using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class FormTeamAnalyser : TeamAnalyserBase
    {
        public FormTeamAnalyser()
        {
            
        }

        public override string Name { get { return nameof(FormTeamAnalyser); } }


        public int MinGamesPlayed
        {
            get { return Properties["MinGamesPlayed"].SafeConvert<int>(); }
            set { Properties["MinGamesPlayed"] = value; }
        }

        public int NumberOfRecentGames
        {
            get { return Properties["NumberOfRecentGames"].SafeConvert<int>(); }
            set { Properties["NumberOfRecentGames"] = value; }
        }

        public PointMapper PointMapper
        {
            get { return Properties["PointMapper"].SafeConvert<PointMapper>(); }
            set { Properties["PointMapper"] = value; }
        }


        public override IEnumerable<TeamRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointMapper == null)
                throw new ArgumentException("Invalid property", nameof(PointMapper));

            var res = new TeamRecommendation();
            res.Type = TeamRecommendationType.TeamForm;
            
            var playerTeam = player.GetLeagueTeam(fixture);

            var leagueFixtures = playerTeam.GetFixturesFromLeagueTeam().ToList();
            var recentFixtures = leagueFixtures
                .Where(x => x.Statistics.GameStarted)
                .Where(x => x.Statistics.GameFinished)
                .OrderByDescending(x => x.Time)
                .Take(NumberOfRecentGames)
                .ToList();
            
            if (recentFixtures.Count < MinGamesPlayed)
            {
                yield return res;
            }
            else
            {
                double points = recentFixtures
                    .Sum(fixt => playerTeam.Team.HasHomeTeamAdvantage(fixt)
                        ? fixt.Statistics.Score.PointsForHomeTeam
                        : fixt.Statistics.Score.PointsForAwayTeam);
                double totalPoints = leagueFixtures
                    .Where(x => x.Statistics.GameStarted && x.Statistics.GameFinished)
                    .Sum(fixt => playerTeam.Team.HasHomeTeamAdvantage(fixt)
                        ? fixt.Statistics.Score.PointsForHomeTeam
                        : fixt.Statistics.Score.PointsForAwayTeam);

                var valueMap = new ValueMap();
                valueMap["team-wins"] = recentFixtures.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Win);
                valueMap["team-draws"] = recentFixtures.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Draw);
                valueMap["team-losses"] = recentFixtures.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Loss);
                valueMap["games-played"] = recentFixtures.Count;
                valueMap["total-games-played"] = leagueFixtures.Where(x => x.Statistics.GameFinished);
                valueMap["points"] = points;
                valueMap["points-per-game"] = points / recentFixtures.Count;
                valueMap["total-points"] = totalPoints;
                valueMap["total-points-per-game"] = totalPoints / recentFixtures.Count;
                
                res.Points = PointMapper.Test(valueMap).Sum(x => x.Points);
                yield return res;
            }
        }


        public override void Configure(XElement element)
        {
            base.Configure(element);
        }

        public override void ConfigureDefault()
        {
            base.ConfigureDefault();

            MinGamesPlayed = 5;
            NumberOfRecentGames = 5;
        }
        
    }
}
