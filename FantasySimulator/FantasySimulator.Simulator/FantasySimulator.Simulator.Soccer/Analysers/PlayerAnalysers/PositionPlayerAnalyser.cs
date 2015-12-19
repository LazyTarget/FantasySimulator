using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PositionPlayerAnalyser : PlayerAnalyserBase
    {
        public PositionPlayerAnalyser()
        {
            
        }

        public override string Name { get { return nameof(PositionPlayerAnalyser); } }
        
        public PointRangeMapper Mapper
        {
            get { return Properties["Mapper"].SafeConvert<PointRangeMapper>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (Mapper == null)
                throw new ArgumentException("Invalid property", nameof(Mapper));

            var res = new PlayerRecommendation();
            res.Type = PlayerRecommendationType.PlayerPosition;

            var playerTeam = player.GetLeagueTeam(fixture);
            double teamPlayedMinutes = playerTeam.GetPlayedMinutesBeforeFixtureForTeam(fixture);
            double playerMinutes = player.GetPlayedMinutesBeforeFixtureForPlayer(fixture);
            var percentage = teamPlayedMinutes > 0
                ? playerMinutes / teamPlayedMinutes
                : 0;
            var playedLeagueFixtues = playerTeam.GetFixturesFromLeagueTeam()
                .Where(x => x.Statistics.GameStarted && x.Statistics.GameFinished)
                .OrderBy(x => x.Time)
                .ToList();


            double cleansheets = playedLeagueFixtues.Count(fixt =>
            {
                var isHome = playerTeam.Team.HasHomeTeamAdvantage(fixt);
                var goalsAgainst = isHome
                    ? fixt.Statistics.Score.GoalsForAwayTeam
                    : fixt.Statistics.Score.GoalsForHomeTeam;
                return goalsAgainst == 0;
            });
            double cleansheetsPer = cleansheets / playedLeagueFixtues.Count;

            var valueMap = new ValueMap();
            valueMap["minutes"] = playerMinutes;
            valueMap["percentage"] = percentage;
            valueMap["player-fantasy-value"] = player.Fantasy.CurrentPrice;
            valueMap["player-position"] = player.Fantasy.Position.ToString();
            valueMap["player-cleansheets"] = cleansheets;
            valueMap["player-cleansheets-percentage"] = cleansheetsPer;
            //valueMap["playedgames"] = ;
            //valueMap["substitutes-in"] = ;
            //valueMap["substitutes-out"] = ;
            // todo: add more data points (subs, recent playtime (5 last team games), recent subs)


            //res.Points = PointMapper.Test(valueMap).Sum(x => x.Points);

            var pointRanges = Mapper.Test(valueMap).ToList();
            var pointMappers = pointRanges.Select(x => x.PointMapper).ToList();
            var pointMappings = pointMappers.SelectMany(x => x.Test(valueMap)).ToList();
            res.Points = pointMappings.Sum(x => x.Points);
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
