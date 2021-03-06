﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class Gameweek
    {
        public Gameweek(League league)
        {
            League = league;
            Fixtures = new List<Fixture>().ToArray();
        }

        public League League { get; private set; }

        public int Number { get; set; }

        public Fixture[] Fixtures { get; set; }

        public bool Started { get { return Fixtures.Any(x => x.Statistics.GameStarted); } }

        public bool Ended { get { return Fixtures.All(x => x.Statistics.GameFinished); } }


        public IEnumerable<Player> GetPlayers()
        {
            if (Fixtures != null)
            {
                foreach (var fixture in Fixtures)
                {
                    var players = fixture.GetPlayers();
                    foreach (var player in players)
                    {
                        yield return player;
                    }
                }
            }
        } 

        public override string ToString()
        {
            return string.Format("Gameweek #{0}", Number);
        }
    }
}
