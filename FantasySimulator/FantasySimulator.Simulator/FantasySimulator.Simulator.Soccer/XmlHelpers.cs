using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer
{
    public static class XmlHelpers
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

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
                        temp.InstantiateConfigurable(element);
                        value = temp;
                    }
                    else if (!type.IsPrimitive)
                    {
                        var temp = Activator.CreateInstance(type);
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
                //_log.Error($"Error when instantiating element", ex);
                throw;
            }
        }

        public static IXmlConfigurable InstantiateConfigurable(this IXmlConfigurable configurable, XElement element)
        {
            configurable.ConfigureExtra(element);
            configurable.Configure(element);
            return configurable;
        }


        public static void ConfigureExtra(this IXmlConfigurable configurable, XElement element)
        {
            if (configurable is IHasProperties)
            {
                var hasProps = (IHasProperties) configurable;
                var propertyElems = element.Elements("property").Where(x => x != null).ToList();
                if (propertyElems.Any())
                {
                    foreach (var elem in propertyElems)
                    {
                        var propertyName = elem.GetAttributeValue("name");
                        if (string.IsNullOrWhiteSpace(propertyName))
                            continue;
                        try
                        {
                            object value = elem.InstantiateElement();
                            hasProps.Properties[propertyName] = value;
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Error instantiating property '{propertyName}'", ex);
                        }
                    }
                }
            }

        }

    }
}
