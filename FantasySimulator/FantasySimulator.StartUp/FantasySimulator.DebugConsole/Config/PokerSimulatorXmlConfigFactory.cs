using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.DebugConsole.Data;
using FantasySimulator.Simulator.Poker;

namespace FantasySimulator.DebugConsole.Config
{
    public class PokerSimulatorXmlConfigFactory : XmlConfigFactoryBase<PokerSimulatorConfig>
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public PokerSimulatorXmlConfigFactory()
        {
            RootElementName = "pokerSimulator";
        }


        public virtual PokerSimulatorConfig GetConfig()
        {
            var config = new PokerSimulatorConfig();
            using (var stream = GetConfigStream(ConfigUri))
            {
                ConfigureFromStream(stream, config);
            }
            return config;
        }
        
        protected override void ConfigureFromXml(XElement rootElement, PokerSimulatorConfig result)
        {
            result.Configure(rootElement);
        }
        
    }
}
