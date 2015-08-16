using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulator : ISimulator
    {
        private ISoccerSimulatorSettings _settings;


        public SoccerSimulator()
        {
            Settings = SoccerSimulatorSettings.Default;
        }


        public ISoccerSimulatorSettings Settings
        {
            get { return _settings; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _settings = value;
            }
        }



        public SoccerSimulationResult Simulate(SoccerSimulationData data)
        {
            var result = new SoccerSimulationResult();
            foreach (var gameweek in data.League.Gameweeks)
            {
                var temp = new List<SoccerSimulationPlayerResult>();
                var players = gameweek.GetPlayers();
                foreach (var player in players)
                {
                    var res = AnalysePlayerResult(player, gameweek);
                    temp.Add(res);
                }


                var top = new List<SoccerSimulationPlayerResult>();
                var recommended = temp.Where(x => x.RecommendationPoints > 0)
                              .OrderBy(x => (int) x.Player.Position)
                              .ThenByDescending(x => x.RecommendationPoints)
                              .ThenByDescending(x => x.EstimatedPoints)
                              .ToList();
                var groups = recommended.GroupBy(x => x.Player.Position);
                foreach (var group in groups)
                {
                    var topForPosition = group.ToList();
                    if (Settings.TopRecommendationsPerPosition > 0)
                        topForPosition = topForPosition.Take(Settings.TopRecommendationsPerPosition).ToList();
                    top.AddRange(topForPosition);

                    // todo: if many with same recommendation points, then set a "uncertainty level" on player?
                }
                result.PlayerResults.Add(gameweek, top);

                // todo: if players are recommended for several gameweeks (in a row), then recommend them even more
            }
            return result;
        }


        private FixtureOdds CalculateOdds(Fixture fixture)
        {
            var fixtureOdds = new FixtureOdds();
            fixtureOdds.Fixture = fixture;

            // todo: implement odds algorithm
            fixtureOdds.HomeWin = fixture.HomeTeam.Rating > fixture.AwayTeam.Rating ? 1 : 3;
            fixtureOdds.Draw    = fixture.HomeTeam.Rating == fixture.AwayTeam.Rating ? 1 : 5;
            fixtureOdds.AwayWin = fixture.AwayTeam.Rating > fixture.HomeTeam.Rating ? 1 : 3;

            // todo: implement prediction
            //fixtureOdds.PredictedScore = 
            return fixtureOdds;
        }



        

        private SoccerSimulationPlayerResult AnalysePlayerResult(Player player, Gameweek gameweek)
        {
            var res = new SoccerSimulationPlayerResult();
            res.Player = player;


            var fixtures = gameweek.Fixtures.Where(x => x.HomeTeam.ID == player.Team.ID || x.AwayTeam.ID == player.Team.ID);
            foreach (var fixture in fixtures)
            {
                if (!Settings.SimulateFinishedGames)
                {
                    if (fixture.Statistics.GameFinished)
                        continue;
                }
                if (Settings.FilterUnavailablePlayers)
                {
                    if (player.Unavailable)
                        break;
                }

                var playerTeam = player.Team;
                var opposingTeam = fixture.GetOpposingTeam(player);

                var homeTeamAdvantage = fixture.HasHomeTeamAdvantage(player);
                var odds = CalculateOdds(fixture);
                var teamBetter = homeTeamAdvantage
                    ? odds.HomeWin < 2
                    : odds.AwayWin < 2;
                var teamWorse = homeTeamAdvantage
                    ? odds.HomeWin > 3
                    : odds.AwayWin > 3;


                // Algorithms
                // todo: give recommendation if is not Forward and tends to score/assist often
                // todo: give recommendation if is defender, but plays as mid and tends to score (points for CS and goals)
                // todo: logical number algorithm
                // todo: remove magic numbers


                // Positives
                if (homeTeamAdvantage)
                    res.AddRecommendation(RecommendationType.HomeTeamAdvantage, 1);

                // todo: better team-vs-team quality check
                if (teamBetter)
                    res.AddRecommendation(RecommendationType.FixtureRatingDiff, 1);
                else if (teamWorse)
                    res.AddRecommendation(RecommendationType.FixtureRatingDiff, -1);
                
                if (Settings.MinimumFixturesForPlaytimeRecommendationBonus <= 0 ||
                    playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForPlaytimeRecommendationBonus)
                {
                    var teamPlayedMinutes = playerTeam.GetPlayedMinutesBeforeFixtureForTeam(fixture);
                    var playerMinutes = player.GetPlayedMinutesBeforeFixtureForPlayer(fixture);
                    if (teamPlayedMinutes > 0)
                    {
                        var playedPercentage = (double)playerMinutes / teamPlayedMinutes;

                        // Positives
                        if (playedPercentage >= (80d / 90))
                            res.AddRecommendation(RecommendationType.PlayerPlaytime, 2);
                        //else if (playedPercentage >= (70d / 90))
                        //    res.AddRecommendation(RecommendationType.PlayerPlaytime, 2);
                        else if (playedPercentage >= (60d / 90))
                            res.AddRecommendation(RecommendationType.PlayerPlaytime, 1);
                        // Negatives
                        else if (playedPercentage <= 0.15)
                            res.AddRecommendation(RecommendationType.PlayerPlaytime, -1);
                    }
                }
                

                // Good player form
                if (Settings.MinimumFixturesForFormRecommendationBonus <= 0 ||
                    playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForFormRecommendationBonus)
                {
                    // todo: don't give points for games too long in the future, as the form could end

                    // Positives
                    if (player.Statistics.Form >= 15)
                        res.AddRecommendation(RecommendationType.PlayerForm, 3);
                    if (player.Statistics.Form >= 10)
                        res.AddRecommendation(RecommendationType.PlayerForm, 2);
                    else if (player.Statistics.Form >= 6)
                        res.AddRecommendation(RecommendationType.PlayerPlaytime, 1);
                }


                //// Good team form
                //if (Settings.MinimumFixturesForFormRecommendationBonus <= 0 ||
                //    playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForFormRecommendationBonus)
                //{
                //    // Positives
                //    if (playerTeam.Statistics.Form >= 15)
                //        res.AddRecommendation(RecommendationType.TeamForm, 3);
                //    if (playerTeam.Statistics.Form >= 10)
                //        res.AddRecommendation(RecommendationType.TeamForm, 2);
                //    else if (playerTeam.Statistics.Form >= 6)
                //        res.AddRecommendation(RecommendationType.TeamForm, 1);
                //}




                // Negatives
                if (player.ChanceOfPlayingNextFixture >= 0)
                {
                    if (player.ChanceOfPlayingNextFixture <= 0)
                        res.AddRecommendation(RecommendationType.LoweredChanceOfPlaying, -10);
                    else if (player.ChanceOfPlayingNextFixture <= 0.25)
                        res.AddRecommendation(RecommendationType.LoweredChanceOfPlaying, -3);
                    else if (player.ChanceOfPlayingNextFixture <= 0.50)
                        res.AddRecommendation(RecommendationType.LoweredChanceOfPlaying, -2);
                    else if (player.ChanceOfPlayingNextFixture <= 0.75)
                        res.AddRecommendation(RecommendationType.LoweredChanceOfPlaying, -1);
                }
                if (player.Unavailable)
                    res.AddRecommendation(RecommendationType.PlayerUnavailable, -10);
            }


            if (Settings.IgnoreRecommendationTypes != null && Settings.IgnoreRecommendationTypes.Any())
            {
                foreach (var type in Settings.IgnoreRecommendationTypes)
                {
                    res.Recommendations.Remove(type);
                }
            }
            return res;
        }
        
    }
}
