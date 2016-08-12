using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.DebugConsole.Config;
using FantasySimulator.DebugConsole.Data;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using FantasySimulator.Core;
using Fclp;

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
            DataFactory = new FantasyPremierLeague2016DataFactory();
            //SettingsFactory = new DefaultSoccerSimulatorSettingsFactory();
            SettingsFactory = new SoccerSimulatorXmlConfigFactory();
        }

        static void Main(string[] args)
        {
            _log.Info("Program started...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_OnFirstChanceException;


            
            
            if (args?.Length > 0)
            {
                //System.Threading.Thread.Sleep(10 * 1200);

                var macros = new Dictionary<string, Func<string, string, object>>();
                macros["now"]           = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-utc"]       = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-local"]     = (macro, format) => DateTime.Now.ToString(format);
                var macroResolver = new MacroResolver();

                var parts = args.ToArray();
                var switchIndex = parts.ToList().FindIndex(x => x.StartsWith("/"));
                if (switchIndex < 0)
                    switchIndex = parts.Length;
                var verb = switchIndex > 0
                    ? String.Join(" ", parts.Take(switchIndex))
                    : parts.Length > 0 ? parts[0] : null;
                parts = switchIndex > 0
                    ? parts.Skip(switchIndex).ToArray()
                    : parts.Length > 1 ? parts.Skip(1).ToArray() : parts;
                
                if (verb == "fpl2016-dl")
                {
                    var parser = new FluentCommandLineParser<DownloadJsonArguments>();
                    parser.Setup(x => x.Uri)
                        .As("uri")
                        .Required()
                        .WithDescription("The uri to download from");
                    parser.Setup(x => x.FileName)
                        .As('f', "file")
                        .Required()
                        .WithDescription("The target filename");
                    parser.Setup(x => x.Username)
                        .As("user")
                        .WithDescription("The auth user");
                    parser.Setup(x => x.Password)
                        .As("pass")
                        .WithDescription("The auth password");

                    var r = parser.Parse(parts);
                    if (r.HasErrors)
                    {
                        Console.WriteLine("Error: " + r.ErrorText);
                    }
                    else if (r.HelpCalled)
                    {
                        Console.WriteLine($"Help for {verb} is not implemented...");
                    }
                    else
                    {
                        var arguments = parser.Object;
                        
                        var fileName = arguments.FileName;
                        fileName = fileName.StartsWith("\"") && fileName.EndsWith("\"")
                            ? fileName.Substring(1, fileName.Length - 2)
                            : fileName;
                        fileName = macroResolver.Resolve(fileName, macros);

                        try
                        {
                            var dataFactory = new FantasyPremierLeague2016DataFactory();
                            dataFactory.Username = arguments.Username;
                            dataFactory.Password = arguments.Password;

                            var data = dataFactory.GetJTokenFromAPI(arguments.Uri).WaitForResult();
                            var saved = dataFactory.SaveJTokenToTextFile(data, fileName).WaitForResult();
                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine("Error saving data. Error: " + ex?.InnerException?.GetBaseException()?.Message);
                            Console.WriteLine(ex);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error saving data. Error: " + ex?.GetBaseException()?.Message);
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            else
            {
                RunSimulator(args);
                
#if DEBUG
                if (Environment.UserInteractive)
                    Console.ReadLine();
                _log.Info("Program exited...");
#endif
            }
        }

        private static void RunSimulator(string[] args)
        {
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
                if (soccerSimulator != null)
                    RunSoccerSimulator(soccerSimulator, configElement);
                else
                {
                    // todo: Implement
                    Console.WriteLine("Simulator '{0}', not implemented", simulator.GetType().Name);
                }

                Console.WriteLine("Simulator '{0}' complete", simulator.GetType().Name);
            }
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
