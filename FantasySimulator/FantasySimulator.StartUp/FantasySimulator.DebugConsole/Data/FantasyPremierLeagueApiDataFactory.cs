using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Data
{
    //[AutoConfigureProperties] todo:
    public class FantasyPremierLeagueApiDataFactory : ISoccerSimulationDataFactory, IXmlConfigurable, IHasProperties
    {
        public FantasyPremierLeagueApiDataFactory()
        {
            Properties = new Dictionary<string, object>();
            //Logger = new FantasyPremierLeagueApi.Helpers.Logger.ConsoleLogger();
            Logger = new NullLogger();
        }

        public IDictionary<string, object> Properties { get; private set; }

        public string Username
        {
            get { return Properties["Username"].SafeConvert<string>(); }
            set { Properties["Username"] = value; }
        }

        public string Password
        {
            get { return Properties["Password"].SafeConvert<string>(); }
            set { Properties["Password"] = value; }
        }

        public FantasyPremierLeagueApi.Helpers.Logger.ILogger Logger
        {
            get { return Properties.GetOrDefault("Logger").SafeConvert<FantasyPremierLeagueApi.Helpers.Logger.ILogger>(); }
            set { Properties["Logger"] = value; }
        }


        public void Configure(XElement element)
        {

        }

        public async Task<SoccerSimulationData> Generate()
        {
            var data = new SoccerSimulationData();
            data = await GenerateApplyTo(data);
            return data;
        }

        public async Task<SoccerSimulationData> GenerateApplyTo(SoccerSimulationData data)
        {
            var api = new FantasyPremierLeagueApi.Api.FantasyPremierLeagueApi(Username, Password, Logger);
            var mySquad = api.GetMySquad();
            var clubSeasonPerformances = api.GetClubSeasonPerformances();
            var remainingBudget = api.GetRemainingBudget();
            var players = api.GetAllPlayers().ToList();

            // todo: Convert classes, and append to data
            throw new NotImplementedException();

            return data;
        }

        private class NullLogger : FantasyPremierLeagueApi.Helpers.Logger.ILogger
        {
            public void WriteDebugMessage(string message)
            {
                
            }

            public void WriteErrorMessage(string message)
            {
                
            }

            public void WriteErrorMessage(string message, Exception e)
            {
                
            }

            public void WriteInfoMessage(string message)
            {
                
            }
        }
    }
}
