using System;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer
{
    public static class XmlHelpers
    {
        public static object InstantiateElement(this XElement element)
        {
            try
            {
                var propertyType = element.GetAttributeValue("type");
                var str = element.GetAttributeValue("value") ?? element.Value;
                object value;
                if (!string.IsNullOrWhiteSpace(propertyType))
                {
                    var type = Type.GetType(propertyType);
                    if (type == null)
                    {
                        throw new TypeAccessException($"Type '{propertyType}' not found");
                    }
                    else if (TypeWrapper.IsAssignableFrom(typeof(IXmlConfigurable), type))
                    {
                        var temp = (IXmlConfigurable)Activator.CreateInstance(type);
                        temp.Configure(element);
                        value = temp;
                    }
                    else
                    {
                        value = str.SafeConvertDynamic(type);
                    }
                }
                else
                    value = str;
                return value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
