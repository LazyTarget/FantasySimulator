using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core.Diagnostics;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class FixtureEloTeamAnalyser : TeamAnalyserBase
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public FixtureEloTeamAnalyser()
        {
            
        }

        public override string Name { get { return nameof(FixtureEloTeamAnalyser); } }


        public IEloProvider EloProvider
        {
            get { return Properties["EloProvider"].SafeConvert<IEloProvider>(); }
            set { Properties["EloProvider"] = value; }
        }

        public PointMapper PointMapper
        {
            get { return Properties["Mapper"].SafeConvert<PointMapper>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<TeamRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointMapper == null)
                throw new ArgumentException("Invalid property", nameof(PointMapper));
            if (EloProvider == null)
                throw new ArgumentException("Invalid property", nameof(EloProvider));

            var res = new TeamRecommendation();
            res.Type = TeamRecommendationType.FixtureOdds;
            
            var playerTeam = player.GetLeagueTeam(fixture);
            var opposingTeam = player.GetOpposingTeam(fixture);
            
            var playerTeamEloRating = EloProvider.GetRating(fixture.Time, player.Team).WaitForResult();
            var opposingTeamEloRating = EloProvider.GetRating(fixture.Time, opposingTeam.Team).WaitForResult();
            if (playerTeamEloRating == null)
            {
                _log.Warn($"Could not get elo rating for team {playerTeam.Team.Name} @{fixture.Time}");
                yield break;
            }
            if (opposingTeamEloRating == null)
            {
                _log.Warn($"Could not get elo rating for team {opposingTeam.Team.Name} @{fixture.Time}");
                yield break;
            }

            float diff = playerTeamEloRating.Rating - opposingTeamEloRating.Rating;
            float diffPercentage = diff / playerTeamEloRating.Rating;

            var valueMap = new ValueMap();
            valueMap[ValueMapKeys.EloRatingPlayerTeam] = playerTeamEloRating.Rating;
            valueMap[ValueMapKeys.EloRatingOpposingTeam] = opposingTeamEloRating.Rating;
            valueMap[ValueMapKeys.EloRatingTeamDifferance] = diff;
            valueMap[ValueMapKeys.EloRatingTeamDifferancePercentage] = diffPercentage;

            res.Points = PointMapper.Test(valueMap).Sum(x => x.Points);
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
