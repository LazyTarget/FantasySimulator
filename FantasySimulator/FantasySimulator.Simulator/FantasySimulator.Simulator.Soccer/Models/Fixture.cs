using System;
using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer.Models
{
    public class Fixture
    {
        public Gameweek Gameweek { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public DateTime Time { get; set; }
        public FixtureStatistics Statistics { get; set; }


        public IEnumerable<Player> GetPlayers()
        {
            if (HomeTeam?.Players != null)
            {
                foreach (var player in HomeTeam.Players)
                {
                    // todo: validate is at club for gameweek (transfered in January window?)
                    yield return player;
                }
            }

            if (AwayTeam?.Players != null)
            {
                foreach (var player in AwayTeam.Players)
                {
                    // todo: validate is at club for gameweek (transfered in January window?)
                    yield return player;
                }
            }
        }


        public override string ToString()
        {
            return string.Format("{0}-{1}", HomeTeam.ID, AwayTeam.ID);
        }
    }
}