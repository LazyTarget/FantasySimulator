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
        private static ISoccerSimulationDataFactory DataFactory;
        private static ISoccerSimulatorSettingsFactory SettingsFactory;


        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog _unhandledLog = Log.GetLog("Logger.UnhandledExceptions");
        private static readonly ILog _firstChanceLog = Log.GetLog("Logger.FirstChanceExceptions");


        static Program()
        {
            //DataFactory = new SampleDataFactory();
            DataFactory = new FantasyPremierLeagueDataFactory();
            //SettingsFactory = new DefaultSoccerSimulatorSettingsFactory();
            SettingsFactory = new SoccerSimulatorSettingsXmlConfigFactory();
        }

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
                        var playerRecPtsStr = "";
                        var teamRecPtsStr = "";
                        if (playerRes.PlayerRecommendations.Any())
                        {
                            foreach (var rec in playerRes.PlayerRecommendations)
                            {
                                playerRecPtsStr += string.Format("{0}: {1}  |  ", rec.Type, rec.Points);
                            }
                            playerRecPtsStr = playerRecPtsStr.Substring(0, playerRecPtsStr.LastIndexOf("  |  "));
                        }

                        if (playerRes.TeamRecommendations.Any())
                        {
                            foreach (var rec in playerRes.TeamRecommendations)
                            {
                                teamRecPtsStr += string.Format("{0}: {1}  |  ", rec.Type, rec.Points);
                            }
                            teamRecPtsStr = teamRecPtsStr.Substring(0, teamRecPtsStr.LastIndexOf("  |  "));
                        }
                        
                        //Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2}", playerRes.Player.DisplayName, playerRes.Player.Rating, playerRes.RecommendationPoints);
                        //Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2} \t\t\t\t--- {3}", playerRes.Player.DisplayName, playerRes.Player.Rating, playerRes.RecommendationPoints, playerRecPtsStr);
                        //Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2} \t\t\t\t--- P.Rec: {3}\t--- T.Rec: {4}", playerRes.Player.DisplayName, playerRes.Player.Rating, playerRes.RecommendationPoints, playerRecPtsStr, teamRecPtsStr);
                        var msg = $"{playerRes.Player.DisplayName} [{playerRes.Player.Rating}] \t\t-- rec.pts: {playerRes.RecommendationPoints} \t\t-- P.Rec: {playerRecPtsStr}\t-- T.Rec: {teamRecPtsStr}";
                        Console.WriteLine(msg);
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
