using System.Configuration;
using System.Xml;

namespace FantasySimulator.DebugConsole.Config
{
    public class SoccerSimulatorConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}
