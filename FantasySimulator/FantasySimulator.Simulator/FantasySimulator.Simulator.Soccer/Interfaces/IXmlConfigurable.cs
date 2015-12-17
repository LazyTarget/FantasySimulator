using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IXmlConfigurable
    {
        void Configure(XElement element);
    }
}
