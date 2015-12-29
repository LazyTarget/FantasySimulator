using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Simulator.Poker;
using FantasySimulator.Simulator.Poker.Interfaces;

namespace FantasySimulator.DebugConsole.Data
{
    public class XmlPokerDataFactory : XmlConfigFactoryBase<PokerSimulationData>, IPokerSimulationDataFactory
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public XmlPokerDataFactory()
        {
            RootElementName = "pokerSimulatorData";
        }

        
        public async Task<PokerSimulationData> Generate()
        {
            var data = new PokerSimulationData();
            data = await GenerateApplyTo(data);
            return data;
        }

        public virtual async Task<PokerSimulationData> GenerateApplyTo(PokerSimulationData data)
        {
            data = await _GenerateApplyTo((PokerSimulationData) data);
            return data;
        }

        private async Task<PokerSimulationData> _GenerateApplyTo(PokerSimulationData data)
        {
            using (var stream = GetConfigStream(ConfigUri))
            {
                ConfigureFromStream(stream, data);
            }
            return data;
        }


        protected override void ConfigureFromXml(XElement rootElement, PokerSimulationData result)
        {
            result.Configure(rootElement);
        }
        
    }
}
