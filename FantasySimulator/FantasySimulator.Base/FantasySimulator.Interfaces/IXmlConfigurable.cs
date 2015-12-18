using System.Xml.Linq;

namespace FantasySimulator.Interfaces
{
    public interface IXmlConfigurable
    {
        void Configure(XElement element);
    }
}
