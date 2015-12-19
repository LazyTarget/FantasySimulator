using System.Collections.Generic;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class HomeAdvantageTeamAnalyser : TeamAnalyserBase
    {
        public HomeAdvantageTeamAnalyser()
        {
            PointsWhenHomeTeam = 1;
            PointsWhenAwayTeam = -1;
        }

        public override string Name { get { return nameof(HomeAdvantageTeamAnalyser); } }

        public int PointsWhenHomeTeam
        {
            get { return Properties["PointsWhenHomeTeam"].SafeConvert<int>(); }
            set { Properties["PointsWhenHomeTeam"] = value; }
        }

        public int PointsWhenAwayTeam
        {
            get { return Properties["PointsWhenAwayTeam"].SafeConvert<int>(); }
            set { Properties["PointsWhenAwayTeam"] = value; }
        }

        public override IEnumerable<TeamRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            var res = new TeamRecommendation();
            res.Type = TeamRecommendationType.HomeTeamAdvantage;

            var playerTeam = player.GetLeagueTeam(fixture);
            if (fixture.HomeTeam.Team.ID == playerTeam.Team.ID ||
                player.HasHomeTeamAdvantage(fixture))
            {
                res.Points = PointsWhenHomeTeam;
            }
            else if (fixture.AwayTeam.Team.ID == player.Team.ID)
            {
                res.Points = PointsWhenAwayTeam;
            }
            yield return res;
        }

        public override void Configure(XElement element)
        {
            base.Configure(element);
        }

        public override void ConfigureDefault()
        {
            base.ConfigureDefault();
        }
    }
}