using System;
using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer.Models
{
    public class Gameweek
    {
        public Gameweek()
        {
            Fixtures = new List<Fixture>().ToArray();
        }

        public int Number { get; set; }

        public Fixture[] Fixtures { get; set; }
        

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
