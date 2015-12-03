using System.Xml.Linq;

namespace FantasySimulator.Core
{
    public static class XmlExtensions
    {
        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            var attr = element.Attribute(attributeName);
            return attr != null ? attr.Value : null;
        }

    }
}
