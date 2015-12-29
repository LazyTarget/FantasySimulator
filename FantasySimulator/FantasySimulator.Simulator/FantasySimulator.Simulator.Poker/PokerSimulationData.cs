using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    [AutoConfigureProperties(AutoConfigure = false)]
    public class PokerSimulationData : IXmlConfigurable
    {
        public PokerSimulationData()
        {
            Games = new List<PokerGame>();
        }
        
        public IList<PokerGame> Games { get; set; }


        public void Configure(XElement element)
        {
            var gameElems = element.Elements("game").Where(x => x != null).ToList();
            if (gameElems.Any())
            {
                //Games.Clear();
                foreach (var gameElem in gameElems)
                {
                    var game = new PokerGame();
                    game.Configure(gameElem);
                    Games.Add(game);
                }
            }


            var gamesElem = element.Element("games");
            if (gamesElem != null)
            {
                gameElems = gamesElem.Elements("game").Where(x => x != null).ToList();
                if (gameElems.Any())
                {
                    //Games.Clear();
                    foreach (var gameElem in gameElems)
                    {
                        var game = new PokerGame();
                        game.Configure(gameElem);
                        Games.Add(game);
                    }
                }
            }
        }

    }
}
