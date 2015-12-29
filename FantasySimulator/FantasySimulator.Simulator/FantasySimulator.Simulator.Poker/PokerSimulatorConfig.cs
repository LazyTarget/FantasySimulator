using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Poker.Interfaces;

namespace FantasySimulator.Simulator.Poker
{
    public class PokerSimulatorConfig : IXmlConfigurable
    {
        public IPokerSimulationDataFactory DataFactory { get; set; }
        //public IPokerSimulatorSettings Settings { get; set; }

        public void Configure(XElement element)
        {
            var dataFactoryElem = element.Element("dataFactory");
            if (dataFactoryElem != null)
            {
                var factoryElems = dataFactoryElem.Elements("factory").Where(x => x != null).ToList();
                foreach (var factoryElem in factoryElems)
                {
                    var obj = factoryElem.InstantiateElement();
                    DataFactory = (IPokerSimulationDataFactory)obj;
                }
            }

            //var settingsElem = element.Element("settings");
            //if (settingsElem != null)
            //{
            //    var settings = new PokerSimulatorSettings();
            //    settings.InstantiateConfigurable(settingsElem);
            //    Settings = settings;
            //}
        }
    }
}
