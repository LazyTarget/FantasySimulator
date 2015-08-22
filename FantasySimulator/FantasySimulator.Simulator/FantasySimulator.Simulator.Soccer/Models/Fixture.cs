using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer
{
    public class Fixture
    {
        public Gameweek Gameweek { get; set; }
        public LeagueTeam HomeTeam { get; set; }
        public LeagueTeam AwayTeam { get; set; }
        public DateTime Time { get; set; }
        public FixtureOdds Odds { get; set; }
        public FixtureStatistics Statistics { get; set; }


        public IEnumerable<Player> GetPlayers()
        {
            if (HomeTeam?.Team?.Players != null)
            {
                foreach (var player in HomeTeam.Team.Players)
                {
                    // todo: validate is at club for gameweek (transfered in January window?)
                    yield return player;
                }
            }

            if (AwayTeam?.Team?.Players != null)
            {
                foreach (var player in AwayTeam.Team.Players)
                {
                    // todo: validate is at club for gameweek (transfered in January window?)
                    yield return player;
                }
            }
        }

        public IEnumerable<PlayerFixtureStats> GetPlayerStats()
        {
            var stats = GetPlayers().Select(GetPlayerStats);
            return stats;
        }

        public PlayerFixtureStats GetPlayerStats(Player player)
        {
            var randomizer = Randomizer.Default;

            var minutes = 0;
            if (Statistics.PlayedMinutes > 0)
            {
                // todo: present the real data!

                if (Statistics.GameFinished)
                {
                    minutes = !player.Fantasy.Unavailable
                               ? player.Fantasy.ChanceOfPlayingNextFixture != -1
                                   ? player.Fantasy.ChanceOfPlayingNextFixture == 1
                                       ? Statistics.PlayedMinutes
                                       : player.Fantasy.ChanceOfPlayingNextFixture >= 0.75
                                           ? randomizer.Next(70, Statistics.PlayedMinutes)
                                           : player.Fantasy.ChanceOfPlayingNextFixture >= 0.50
                                               ? randomizer.Next(50, Statistics.PlayedMinutes)
                                               : 0
                                   : randomizer.Next(45, Statistics.PlayedMinutes)
                               : 0;
                }
                else
                    minutes = !player.Fantasy.Unavailable
                        ? randomizer.Next(0, Statistics.PlayedMinutes)
                        : 0;
            }

            var s = new PlayerFixtureStats
            {
                Player = player,
                PlayedMinutes = minutes,
            };
            return s;
        }


        public override string ToString()
        {
            return string.Format("{0}-{1} ({2})", HomeTeam.Team.ShortName, AwayTeam.Team.ShortName, Statistics);
        }
    }
}