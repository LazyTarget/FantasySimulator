using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.DebugConsole.Config;
using FantasySimulator.DebugConsole.Data;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole
{
    class Program
    {
        //private static ISoccerSimulationDataFactory DataFactory = new SampleDataFactory();
        private static ISoccerSimulationDataFactory DataFactory = new FantasyPremierLeagueDataFactory();
        //private static ISoccerSimulatorSettingsFactory SettingsFactory = new DefaultSoccerSimulatorSettingsFactory();
        private static ISoccerSimulatorSettingsFactory SettingsFactory = new SoccerSimulatorSettingsXmlConfigFactory();


        static void Main(string[] args)
        {
            var simulator = new SoccerSimulator();
            simulator.Settings = SettingsFactory.GetSettings();
            var simulationData = DataFactory.Generate().WaitForResult();
            var result = simulator.Simulate(simulationData);
            
            for (var i = 0; i < result.PlayerResults.Count; i++)
            {
                var gameweek = result.PlayerResults.Keys.Cast<Gameweek>().ElementAt(i);
                var players = (IEnumerable<SoccerSimulationPlayerResult>) result.PlayerResults.Values.ElementAt(i);
                var positionGroups = players.GroupBy(x => x.Player.Fantasy.Position);

                Console.WriteLine("Gameweek #{0}", gameweek.Number);
                foreach (var group in positionGroups)
                {
                    Console.WriteLine("Position: {0}", group.Key);
                    foreach (var playerRes in group)
                    {
                        var ptsStr = "";
                        foreach (var recType in playerRes.Recommendations.Keys)
                        {
                            var pts = playerRes.Recommendations[recType];
                            ptsStr += string.Format("{0}: {1}  |  ", recType, pts);
                        }

                        //Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2}", playerRes.Player.DisplayName, playerRes.Player.Rating, playerRes.RecommendationPoints);
                        Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2} \t\t\t\t--- {3}", playerRes.Player.DisplayName, playerRes.Player.Rating, playerRes.RecommendationPoints, ptsStr);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            // todo: export as csv/excel? detailed report, with the team-vs-team, estimated points, recommendation points

            Console.ReadLine();
        }
    }
}
