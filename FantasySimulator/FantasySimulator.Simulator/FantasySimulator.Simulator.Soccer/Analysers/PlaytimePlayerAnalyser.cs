using System.Collections.Generic;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PlaytimePlayerAnalyser : PlayerAnalyserBase
    {
        public override string Name { get { return nameof(PlaytimePlayerAnalyser); } }
        

        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {


            //if (Settings.MinimumFixturesForPlaytimeRecommendationBonus <= 0 ||
            //        playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForPlaytimeRecommendationBonus)
            //{
            //    var teamPlayedMinutes = playerTeam.GetPlayedMinutesBeforeFixtureForTeam(fixture);
            //    var playerMinutes = player.GetPlayedMinutesBeforeFixtureForPlayer(fixture);
            //    if (teamPlayedMinutes > 0)
            //    {
            //        var playedPercentage = (double)playerMinutes / teamPlayedMinutes;

            //        // Positives
            //        if (playedPercentage >= (80d / 90))
            //            res.AddRecommendation(RecommendationType.PlayerPlaytime, 2);
            //        //else if (playedPercentage >= (70d / 90))
            //        //    res.AddRecommendation(RecommendationType.PlayerPlaytime, 2);
            //        else if (playedPercentage >= (60d / 90))
            //            res.AddRecommendation(RecommendationType.PlayerPlaytime, 1);
            //        // Negatives
            //        else if (playedPercentage <= 0.15)
            //            res.AddRecommendation(RecommendationType.PlayerPlaytime, -1);
            //    }
            //}
            yield break;
        }


        public override void Configure(XElement element)
        {
            base.Configure(element);
        }
    }
}
