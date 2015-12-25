using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.DebugConsole.Data;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using FantasySimulator.Simulator.Soccer.Analysers;

namespace FantasySimulator.DebugConsole.Config
{
    public class SoccerSimulatorXmlConfigFactory : XmlConfigFactoryBase<SoccerSimulatorXmlConfigFactory.SoccerSimulatorXmlConfig>, ISoccerSimulatorSettingsFactory
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public SoccerSimulatorXmlConfigFactory()
        {
            RootElementName = "soccerSimulator";
        }


        public virtual SoccerSimulatorXmlConfig GetConfig()
        {
            var config = new SoccerSimulatorXmlConfig();
            using (var stream = GetConfigStream(ConfigUri))
            {
                ConfigureFromStream(stream, config);
            }
            return config;
        }

        public virtual ISoccerSimulatorSettings GetSettings()
        {
            var config = GetConfig();
            var settings = config?.Settings;
            return settings;
        }
        
        protected override void ConfigureFromXml(XElement rootElement, SoccerSimulatorXmlConfig result)
        {
            result.Configure(rootElement);
        }



        


        public class SoccerSimulatorXmlConfig : SoccerSimulatorConfig, IXmlConfigurable
        {
            private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);


            public SoccerSimulatorXmlConfig()
            {
                
            }

            

            public virtual void Configure(XElement element)
            {
                var dataFactoryElem = element.Element("dataFactory");
                if (dataFactoryElem != null)
                {
                    var type = dataFactoryElem.GetAttributeValue("type");
                    if (!string.IsNullOrWhiteSpace(type))
                    {
                        var obj = dataFactoryElem.InstantiateElement();
                        var dataFactory = (ISoccerSimulationDataFactory) obj;
                        DataFactory = dataFactory;
                    }
                    else
                    {
                        var obj = new SoccerSimulatorXmlDataFactory();
                        obj.InstantiateConfigurable(dataFactoryElem);
                        DataFactory = obj;
                    }
                }

                var settingsElem = element.Element("settings");
                if (settingsElem != null)
                {
                    var xmlSettings = Settings as SoccerSimulatorXmlSettings ?? new SoccerSimulatorXmlSettings();
                    xmlSettings.InstantiateConfigurable(settingsElem);
                    Settings = xmlSettings;
                }
            }
        }


        protected class SoccerSimulatorXmlDataFactory : IXmlConfigurable, IHasProperties, ISoccerSimulationDataFactory
        {
            private readonly IList<ISoccerSimulationDataFactory> _factories;

            public SoccerSimulatorXmlDataFactory()
            {
                Properties = new Dictionary<string, object>();
                _factories = new List<ISoccerSimulationDataFactory>();
            }

            public IDictionary<string, object> Properties { get; private set; }
            

            public void Configure(XElement element)
            {
                var dataFactoriesElems = element.Elements("factory").Where(x => x != null).ToList();
                if (dataFactoriesElems.Any())
                {
                    _factories.Clear();
                    foreach (var elem in dataFactoriesElems)
                    {
                        var obj = elem.InstantiateElement();
                        var dataFactory = (ISoccerSimulationDataFactory)obj;
                        var factory = dataFactory;
                        _factories.Add(factory);
                    }
                }
            }
            
            public async Task<SoccerSimulationData> Generate()
            {
                if (_factories != null)
                {
                    var data = new SoccerSimulationData();
                    foreach (var factory in _factories)
                    {
                        var data2 = await factory.Generate();
                        MergeData(data, data2);
                    }
                    return data;
                }
                return null;
            }

            protected virtual void MergeData(SoccerSimulationData data1, SoccerSimulationData data2)
            {
                data1.Leagues = data1.Leagues ?? new League[0];
                data1.Leagues = data1.Leagues.Concat(data2.Leagues ?? new League[0]).ToArray();
            }
        }


        protected class SoccerSimulatorXmlSettings : SoccerSimulatorSettings, IXmlConfigurable
        {
            private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);


            public SoccerSimulatorXmlSettings()
            {
                
            }

            

            public virtual void Configure(XElement element)
            {
                //var settingsElem = element.Element("settings");
                var settingsElem = element;
                if (settingsElem != null)
                {
                    foreach (var childNode in settingsElem.Elements())
                    {
                        if (childNode == null || string.IsNullOrWhiteSpace(childNode.Name.LocalName))
                            continue;
                        if (childNode.Name == "property")
                        {
                            ApplyProperty(childNode);
                        }
                        else
                        {
                            
                        }
                    }
                }

                
                var playerAnalysersNode = element.Element("playerAnalysers");
                if (playerAnalysersNode != null)
                {
                    foreach (var childNode in playerAnalysersNode.Elements())
                    {
                        if (childNode == null || string.IsNullOrWhiteSpace(childNode.Name.LocalName))
                            continue;
                        if (childNode.Name == "analyser")
                        {
                            ApplyPlayerAnalyser(childNode);
                        }
                        else
                        {
                            
                        }
                    }
                }


                var teamAnalysersNode = element.Element("teamAnalysers");
                if (teamAnalysersNode != null)
                {
                    foreach (var childNode in teamAnalysersNode.Elements())
                    {
                        if (childNode == null || string.IsNullOrWhiteSpace(childNode.Name.LocalName))
                            continue;
                        if (childNode.Name == "analyser")
                        {
                            ApplyTeamAnalyser(childNode);
                        }
                        else
                        {

                        }
                    }
                }
            }



            protected virtual void ApplyProperty(XElement element)
            {
                var propertyName = element.GetAttributeValue("name");
                try
                {
                    if (!string.IsNullOrWhiteSpace(propertyName))
                    {
                        var propertyInfo = GetType().GetProperty(propertyName);
                        if (propertyInfo != null)
                        {
                            //var str = element.GetAttributeValue("value") ?? element.Value;
                            //var value = str.SafeConvertDynamic(propertyInfo.PropertyType);

                            var value = element.InstantiateElement();
                            value = value.SafeConvertDynamic(propertyInfo.PropertyType);
                            propertyInfo.SetValue(this, value);
                        }
                        else
                            throw new Exception($"Property not found {propertyName}");
                    }
                    else
                        _log.Warn("Defined <property> has no name");
                }
                catch (Exception ex)
                {
                    _log.Error($"Error when applying property {propertyName}", ex);
                }
            }


            protected virtual void ApplyPlayerAnalyser(XElement element)
            {
                var typeName = element.GetAttributeValue("type");
                try
                {
                    if (string.IsNullOrWhiteSpace(typeName))
                        throw new FormatException("Invalid analyser type name");
                    var type = Type.GetType(typeName, true);
                    if (!(typeof (PlayerAnalyserBase).IsAssignableFrom(type)))
                        throw new Exception($"Specified type doesn't inherit '{nameof(PlayerAnalyserBase)}'");

                    //var analyser = (PlayerAnalyserBase) Activator.CreateInstance(type);
                    //analyser.Configure(element);
                    //PlayerAnalysers.Add(analyser);

                    var obj = element.InstantiateElement();
                    var analyser = (PlayerAnalyserBase) obj;
                    PlayerAnalysers.Add(analyser);
                }
                catch (Exception ex)
                {
                    _log.Error($"Error when applying player analyser", ex);
                }
            }

            protected virtual void ApplyTeamAnalyser(XElement element)
            {
                var typeName = element.GetAttributeValue("type");
                try
                {
                    if (string.IsNullOrWhiteSpace(typeName))
                        throw new FormatException("Invalid analyser type name");
                    var type = Type.GetType(typeName, true);
                    if (!(typeof(TeamAnalyserBase).IsAssignableFrom(type)))
                        throw new Exception($"Specified type doesn't inherit '{nameof(TeamAnalyserBase)}'");
                    
                    var obj = element.InstantiateElement();
                    var analyser = (TeamAnalyserBase)obj;
                    TeamAnalysers.Add(analyser);
                }
                catch (Exception ex)
                {
                    _log.Error($"Error when applying team analyser", ex);
                }
            }
        }
        
    }
}
