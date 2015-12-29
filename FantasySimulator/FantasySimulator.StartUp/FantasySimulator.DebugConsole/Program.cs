using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.DebugConsole.Config;
using FantasySimulator.DebugConsole.Data;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Poker;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole
{
    class Program
    {
        private static ISoccerSimulationDataFactory DataFactory;
        //private static ISoccerSimulatorSettingsFactory SettingsFactory;
        private static SoccerSimulatorXmlConfigFactory SettingsFactory;


        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog _unhandledLog = Log.GetLog("Logger.UnhandledExceptions");
        private static readonly ILog _firstChanceLog = Log.GetLog("Logger.FirstChanceExceptions");


        static Program()
        {
            //DataFactory = new SampleDataFactory();
            DataFactory = new FantasyPremierLeagueDataFactory();
            //SettingsFactory = new DefaultSoccerSimulatorSettingsFactory();
            SettingsFactory = new SoccerSimulatorXmlConfigFactory();
        }

        static void Main(string[] args)
        {
            _log.Info("Program started...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_OnFirstChanceException;

            
            var configSection = SimulatorRunnerConfigSection.LoadFromConfig();
            foreach (SimulatorConfigElement configElement in configSection.Runners)
            {
                if (!configElement.Enabled)
                {
                    Console.WriteLine("Ignoring Simulator '{0}' as it has Enabled set to false", configElement.Type);
                    continue;
                }
                var type = Type.GetType(configElement.Type);
                if (type == null)
                {
                    Console.WriteLine("Ignoring Simulator '{0}' as it could not find the Type", configElement.Type);
                    continue;
                }
                var inst = Activator.CreateInstance(type);
                var simulator = (ISimulator) inst;


                Console.WriteLine("Starting Simulator '{0}'", simulator.GetType().Name);

                var soccerSimulator = simulator as SoccerSimulator;
                var pokerSimulator = simulator as PokerSimulator;
                if (soccerSimulator != null)
                    RunSoccerSimulator(soccerSimulator, configElement);
                else if (pokerSimulator != null)
                    RunPokerSimulator(pokerSimulator, configElement);
                else
                {
                    // todo: Implement
                    Console.WriteLine("Simulator '{0}', not implemented", simulator.GetType().Name);
                }

                Console.WriteLine("Simulator '{0}' complete", simulator.GetType().Name);

            }

#if DEBUG
            Console.ReadLine();
            _log.Info("Program exited...");
#endif
        }


        private static void RunSoccerSimulator(SoccerSimulator simulator, SimulatorConfigElement simulatorConfigElement)
        {
            if (!string.IsNullOrWhiteSpace(simulatorConfigElement.ConfigUri))
                SettingsFactory.SetConfigUri(simulatorConfigElement.ConfigUri);
            if (!string.IsNullOrWhiteSpace(simulatorConfigElement.RootElementName))
                SettingsFactory.RootElementName = simulatorConfigElement.RootElementName;

            var config = SettingsFactory.GetConfig();

            //simulator.Settings = SettingsFactory.GetSettings();
            simulator.Settings = config.Settings;

            if (config.DataFactory != null)
            {
                DataFactory = config.DataFactory;
            }

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

        }


        private static void RunPokerSimulator(PokerSimulator simulator, SimulatorConfigElement simulatorConfigElement)
        {
            var configFactory = new PokerSimulatorXmlConfigFactory();
            if (!string.IsNullOrWhiteSpace(simulatorConfigElement.ConfigUri))
                configFactory.SetConfigUri(simulatorConfigElement.ConfigUri);
            if (!string.IsNullOrWhiteSpace(simulatorConfigElement.RootElementName))
                configFactory.RootElementName = simulatorConfigElement.RootElementName;

            var config = configFactory.GetConfig();
            var data = config.DataFactory.Generate().WaitForResult();

            var result = simulator.Simulate(data);
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
