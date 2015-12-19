using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulator : ISimulator
    {
        private ISoccerSimulatorSettings _settings;


        public SoccerSimulator()
        {
            TypeContainer = new TypeContainer();
            TypeContainer.SetInstance<IFixtureOddsProvider>(new ThirdParty.Kambi.KambiAPI());
        }


        public ITypeContainer TypeContainer { get; set; }

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
            var context = new SimulationContext(data, Settings);
            foreach (var league in data.Leagues)
            {
                foreach (var gameweek in league.Gameweeks)
                {
                    foreach (var fixture in gameweek.Fixtures)
                    {
                        if (Settings.CalculateOddsWhenSimulating)
                            fixture.Odds = CalculateOdds(fixture);
                    }

                    if (gameweek.Ended)
                    {
                        context.LastPlayedGameweek = gameweek;
                    }

                    var temp = new List<SoccerSimulationPlayerResult>();
                    var players = gameweek.GetPlayers();
                    foreach (var player in players)
                    {
                        if (Settings.FilterUnavailablePlayers)
                        {
                            if (player.Fantasy.Unavailable)
                                continue;
                        }

                        var res = AnalysePlayerResult(player, gameweek, context);
                        temp.Add(res);
                    }


                    var top = new List<SoccerSimulationPlayerResult>();
                    var recommended = temp.Where(x => x.RecommendationPoints > 0)
                                  .OrderBy(x => (int) x.Player.Fantasy.Position)
                                  .ThenByDescending(x => x.RecommendationPoints)
                                  .ThenByDescending(x => x.EstimatedPoints)
                                  .ToList();
                    var groups = recommended.GroupBy(x => x.Player.Fantasy.Position);
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
            }
            return result;
        }


        private FixtureOdds CalculateOdds(Fixture fixture)
        {
            FixtureOdds odds = null;
            var provider = TypeContainer.GetInstance<IFixtureOddsProvider>();
            if (provider != null)
            {
                odds = provider.GetFixtureOdds(fixture).WaitForResult();
            }
            return odds;


            //var odds = new FixtureOdds();
            //odds.Fixture = fixture;

            //// todo: implement odds algorithm
            //odds.HomeWin = fixture.HomeTeam.Team.Rating > fixture.AwayTeam.Team.Rating ? 1 : 3;
            //odds.Draw = fixture.HomeTeam.Team.Rating == fixture.AwayTeam.Team.Rating ? 1 : 5;
            //odds.AwayWin = fixture.AwayTeam.Team.Rating > fixture.HomeTeam.Team.Rating ? 1 : 3;

            //// todo: implement prediction
            ////fixtureOdds.PredictedScore = 
            //return odds;
        }



        

        private SoccerSimulationPlayerResult AnalysePlayerResult(Player player, Gameweek gameweek, SimulationContext context)
        {
            var res = new SoccerSimulationPlayerResult();
            res.Player = player;
            
            
            var fixtures = gameweek.Fixtures.Where(x => x.HomeTeam.Team.ID == player.Team.ID || x.AwayTeam.Team.ID == player.Team.ID);
            foreach (var fixture in fixtures)
            {
                if (!Settings.SimulateFinishedGames)
                {
                    if (fixture.Statistics.GameFinished)
                        continue;
                }


                if (Settings.PlayerAnalysers != null)
                {
                    foreach (var analyser in Settings.PlayerAnalysers)
                    {
                        if (!analyser.Enabled)
                            continue;
                        var rec = analyser.Analyse(player, fixture, context);
                        if (rec != null)
                        {
                            foreach (var r in rec)
                            {
                                if (r == null || r.Type == PlayerRecommendationType.None)
                                    continue;
                                res.AddRecommendation(r);
                            }
                        }
                    }
                }

                if (Settings.TeamAnalysers != null)
                {
                    foreach (var analyser in Settings.TeamAnalysers)
                    {
                        if (!analyser.Enabled)
                            continue;
                        var rec = analyser.Analyse(player, fixture, context);
                        if (rec != null)
                        {
                            foreach (var r in rec)
                            {
                                if (r == null || r.Type == TeamRecommendationType.None)
                                    continue;
                                res.AddRecommendation(r);
                            }
                        }
                    }
                }



                #region OLD
                /*

                // Algorithms
                // todo: give recommendation if is not Forward and tends to score/assist often
                // todo: give recommendation if is defender, but plays as mid and tends to score (points for CS and goals)
                // todo: logical number algorithm
                // todo: remove magic numbers

                
                var playerTeam = player.GetLeagueTeam(fixture);
                var opposingTeam = player.GetOpposingTeam(fixture);
                var homeTeamAdvantage = player.HasHomeTeamAdvantage(fixture);
                var gameweeksFromLastPlayedGW = gameweek.Number - context.LastPlayedGameweek.Number;

                
                //var odds = CalculateOdds(fixture);
                var odds = fixture.Odds;
                if (odds != null)
                {
                    var teamBetter = homeTeamAdvantage
                    ? odds.HomeWin < 2
                    : odds.AwayWin < 2;
                    var teamWorse = homeTeamAdvantage
                        ? odds.HomeWin > 3
                        : odds.AwayWin > 3;


                    // todo: better team-vs-team quality check (with weight)
                    if (teamBetter)
                        res.AddRecommendation(RecommendationType.FixtureRatingDiff, 1);
                    else if (teamWorse)
                        res.AddRecommendation(RecommendationType.FixtureRatingDiff, -1);
                }



                // Positives
                if (homeTeamAdvantage)
                    res.AddRecommendation(RecommendationType.HomeTeamAdvantage, 1);


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



                // Form
                if (gameweeksFromLastPlayedGW > 0 &&
                    gameweeksFromLastPlayedGW <= Settings.LengthOfFormWhenSimulating)
                {
                    // Good player form
                    if (Settings.MinimumFixturesForFormRecommendationBonus <= 0 ||
                        playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForFormRecommendationBonus)
                    {
                        // Positives
                        if (player.Statistics.Form >= 15)
                            res.AddRecommendation(RecommendationType.PlayerForm, 3);
                        if (player.Statistics.Form >= 10)
                            res.AddRecommendation(RecommendationType.PlayerForm, 2);
                        else if (player.Statistics.Form >= 6)
                            res.AddRecommendation(RecommendationType.PlayerPlaytime, 1);
                    }


                    // Good team form
                    if (Settings.MinimumFixturesForFormRecommendationBonus <= 0 ||
                        playerTeam.Statistics.PlayedGames >= Settings.MinimumFixturesForFormRecommendationBonus)
                    {
                        // Positives
                        if (playerTeam.Statistics.Form >= 15)
                            res.AddRecommendation(RecommendationType.TeamForm, 3);
                        if (playerTeam.Statistics.Form >= 10)
                            res.AddRecommendation(RecommendationType.TeamForm, 2);
                        else if (playerTeam.Statistics.Form >= 6)
                            res.AddRecommendation(RecommendationType.TeamForm, 1);
                    }
                }



                // Negatives
                if (player.Fantasy.ChanceOfPlayingNextFixture >= 0)
                {
                    if (player.Fantasy.ChanceOfPlayingNextFixture <= 0)
                        res.AddRecommendation(RecommendationType.ChanceOfPlaying, -10);
                    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.25)
                        res.AddRecommendation(RecommendationType.ChanceOfPlaying, -3);
                    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.50)
                        res.AddRecommendation(RecommendationType.ChanceOfPlaying, -2);
                    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.75)
                        res.AddRecommendation(RecommendationType.ChanceOfPlaying, -1);
                }
                if (player.Fantasy.Unavailable)
                    res.AddRecommendation(RecommendationType.PlayerUnavailable, -10);
                */

                #endregion

            }
            
            return res;
        }
        
    }
    
}
