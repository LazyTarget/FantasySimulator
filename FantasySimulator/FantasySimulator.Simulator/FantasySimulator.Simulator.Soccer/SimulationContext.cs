using System.Linq;
using FantasySimulator.Simulator.Soccer.Analysers;
using FantasySimulator.Simulator.Soccer.Structs;

namespace FantasySimulator.Simulator.Soccer
{
    public class SimulationContext
    {
        private readonly SoccerSimulationData _data;

        internal SimulationContext(SoccerSimulationData data, ISoccerSimulatorSettings settings)
        {
            Settings = settings;
            _data = data;
        }

        public Gameweek LastPlayedGameweek { get; set; }
        public ISoccerSimulatorSettings Settings { get; set; }


        public ValueMap GeneratePlayerValueMap(Player player, Fixture fixture, PlayerAnalyserBase analyser)
        {
            

            var playerTeam = player.GetLeagueTeam(fixture);
            double teamPlayedMinutes = playerTeam.GetPlayedMinutesBeforeFixtureForTeam(fixture);
            double playerMinutes = player.GetPlayedMinutesBeforeFixtureForPlayer(fixture);
            var percentage = teamPlayedMinutes > 0
                ? playerMinutes / teamPlayedMinutes
                : 0;
            var recentLeagueFixtues = playerTeam.GetFixturesFromLeagueTeam()
                .Where(x => x.Statistics.GameStarted && x.Statistics.GameFinished)
                .OrderByDescending(x => x.Time)
                .Take(5)
                .ToList();
            var playedLeagueFixtues = playerTeam.GetFixturesFromLeagueTeam()
                .Where(x => x.Statistics.GameStarted && x.Statistics.GameFinished)
                .OrderBy(x => x.Time)
                .ToList();

            double points = playedLeagueFixtues
                .Sum(fixt => playerTeam.Team.HasHomeTeamAdvantage(fixt)
                    ? fixt.Statistics.Score.PointsForHomeTeam
                    : fixt.Statistics.Score.PointsForAwayTeam);
            double totalPoints = playedLeagueFixtues
                .Where(x => x.Statistics.GameStarted && x.Statistics.GameFinished)
                .Sum(fixt => playerTeam.Team.HasHomeTeamAdvantage(fixt)
                    ? fixt.Statistics.Score.PointsForHomeTeam
                    : fixt.Statistics.Score.PointsForAwayTeam);


            var fixtGoalsAgainst = playedLeagueFixtues.Select(fixt =>
            {
                var isHome = playerTeam.Team.HasHomeTeamAdvantage(fixt);
                var ga = isHome
                    ? fixt.Statistics.Score.GoalsForAwayTeam
                    : fixt.Statistics.Score.GoalsForHomeTeam;
                return ga;
            }).ToList();

            double saves = player.Statistics.Saves;
            double goalsAgainst = fixtGoalsAgainst.Sum(x => x);
            double cleansheets = fixtGoalsAgainst.Count(x => x == 0);
            double cleansheetsPer = cleansheets / playedLeagueFixtues.Count;



            var valueMap = new ValueMap();
            valueMap[ValueMapKeys.GameweekStarted] = fixture.Gameweek.Started;
            valueMap[ValueMapKeys.GameweekEnded] = fixture.Gameweek.Ended;
            valueMap[ValueMapKeys.FixtureStarted] = fixture.Statistics.GameStarted;
            valueMap[ValueMapKeys.FixtureEnded] = fixture.Statistics.GameFinished;

            valueMap[ValueMapKeys.TotalGamesWon] = playedLeagueFixtues.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Win);
            valueMap[ValueMapKeys.TotalGamesDraw] = playedLeagueFixtues.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Draw);
            valueMap[ValueMapKeys.TotalGamesLoss] = playedLeagueFixtues.Count(x => playerTeam.GetTeamFixtureOutcome(fixture) == TeamFixtureOutcome.Loss);
            valueMap[ValueMapKeys.PlayerTotalGamesPlayed] = playedLeagueFixtues.Count;
            valueMap[ValueMapKeys.TotalGamesPlayed] = playedLeagueFixtues.Count;
            valueMap[ValueMapKeys.RecentGamesTeamPlayed] = recentLeagueFixtues.Count;
            valueMap[ValueMapKeys.RecentGamesTeamPoints] = points;
            valueMap[ValueMapKeys.RecentPointsPerGame] = points / recentLeagueFixtues.Count;
            valueMap[ValueMapKeys.TotalPoints] = totalPoints;
            valueMap[ValueMapKeys.TotalPointsPerGame] = totalPoints / playedLeagueFixtues.Count;

            valueMap[ValueMapKeys.PlayerTotalMinutesPlayed] = playerMinutes;
            valueMap[ValueMapKeys.PlayerTotalMinutesPercentageOfTeam] = percentage;
            valueMap[ValueMapKeys.FantasyPlayerCurrentPrice] = player.Fantasy.CurrentPrice;
            valueMap[ValueMapKeys.PlayerPosition] = player.Fantasy.Position.ToString();
            valueMap[ValueMapKeys.PlayerTotalCleansheets] = cleansheets;
            valueMap[ValueMapKeys.PlayerTotalCleansheetsPercentage] = cleansheetsPer;
            valueMap[ValueMapKeys.PlayerTotalSaves] = saves;
            valueMap[ValueMapKeys.PlayerTotalSavesPerGame] = saves / playedLeagueFixtues.Count;
            valueMap[ValueMapKeys.PlayerTotalSavesPercentage] = saves / (saves + goalsAgainst);       // todo: filter out other keepers in the team
            //valueMap["playedgames"] = ;
            //valueMap["substitutes-in"] = ;
            //valueMap["substitutes-out"] = ;
            // todo: add more data points (subs, recent playtime (5 last team games), recent subs)

            return valueMap;
        }
    }
}