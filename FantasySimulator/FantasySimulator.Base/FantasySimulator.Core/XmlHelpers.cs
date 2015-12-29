using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Core
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
                        // todo: use xml attributes use for CreateInstance with args

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
            var attr = configurable.GetType().GetCustomAttribute<AutoConfigurePropertiesAttribute>(inherit: true);

            bool autoConfigureProps;
            if (attr != null)
                autoConfigureProps = attr.AutoConfigure;
            else
            {
                autoConfigureProps = false;
                autoConfigureProps = true;      // todo: remove when AutoConfigurePropertiesAttribute added to all classes needed
            }

            //var autoConfigureProps = attr?.AutoConfigure;
            if (autoConfigureProps)
            {
                configurable.ConfigureProperties(element);
            }
            else
            {
                
            }

            configurable.Configure(element);
            return configurable;
        }


        public static void ConfigureProperties(this IXmlConfigurable configurable, XElement element)
        {
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
                        if (configurable is IHasProperties)
                        {
                            var hasProps = (IHasProperties) configurable;
                            hasProps.Properties[propertyName] = value;
                        }
                        else
                        {
                            var propertyInfo = configurable.GetType().GetProperty(propertyName);
                            if (propertyInfo != null)
                            {
                                value = value.SafeConvertDynamic(propertyInfo.PropertyType);
                                propertyInfo.SetValue(configurable, value);
                            }
                            else
                                throw new Exception($"Property not found {propertyName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Error instantiating property '{propertyName}'", ex);
                    }
                }
            }
            else
            {
                
            }
        }

    }
}
