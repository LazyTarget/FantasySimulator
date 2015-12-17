using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using FantasySimulator.Core.Diagnostics;
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


        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog _unhandledLog = Log.GetLog("Logger.UnhandledExceptions");
        private static readonly ILog _firstChanceLog = Log.GetLog("Logger.FirstChanceExceptions");


        static void Main(string[] args)
        {
            _log.Info("Program started...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_OnFirstChanceException;


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
            _log.Info("Program exited...");
        }


        private static void CurrentDomain_OnFirstChanceException(object sender, FirstChanceExceptionEventArgs args)
        {
            _firstChanceLog.Error("OnFirstChanceException", args.Exception);
        }

        private static void CurrentDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _unhandledLog.Fatal("OnUnhandledException", args.ExceptionObject as Exception);
        }
    }
}
