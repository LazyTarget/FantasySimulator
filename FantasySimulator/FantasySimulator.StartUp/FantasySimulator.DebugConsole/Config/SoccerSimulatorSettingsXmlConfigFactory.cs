using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using FantasySimulator.Simulator.Soccer.Analysers;

namespace FantasySimulator.DebugConsole.Config
{
    public class SoccerSimulatorSettingsXmlConfigFactory : ISoccerSimulatorSettingsFactory
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public SoccerSimulatorSettingsXmlConfigFactory()
        {
            var configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

            ConfigUri = new Uri(configPath);
            RootElementName = "soccerSimulator";
        }


        public string RootElementName { get; set; }

        public Uri ConfigUri { get; set; }


        public virtual ISoccerSimulatorSettings GetSettings()
        {
            var settings = new SoccerSimulatorXmlSettings();
            using (var stream = GetConfigStream(ConfigUri))
            {
                ConfigureFromStream(stream, settings);
            }
            return settings;
        }


        protected virtual Stream GetConfigStream(Uri configUri)
        {
            if (configUri.IsFile)
            {
                var fileInfo = new FileInfo(configUri.LocalPath);
                if (fileInfo.Exists)
                {
                    var fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return fileStream;
                }
                else
                {
                    throw new FileNotFoundException("Config file not found", fileInfo.FullName);
                }
            }
            else
            {
                var client = new HttpClient();
                try
                {
                    var task = client.GetAsync(configUri);
                    var response = task.WaitForResult();
                    response.EnsureSuccessStatusCode();

                    var task2 = response.Content.ReadAsStreamAsync();
                    var stream = task2.WaitForResult();
                    return stream;
                }
                catch (Exception ex)
                {
                    throw;
                }
                throw new NotImplementedException("Network paths not implemented");
            }
        }


        private void ConfigureFromStream(Stream stream, SoccerSimulatorXmlSettings settings)
        {
            XDocument xdoc;
            try
            {
                var xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;

                using (var xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                {
                    xdoc = XDocument.Load(xmlReader);
                    var elements = xdoc.Elements(RootElementName);
                    var configElem = xdoc.Element("configuration");
                    if (configElem != null)
                        elements = elements.Concat(configElem.Elements(RootElementName));
                    foreach (var elem in elements)
                    {
                        ConfigureFromXml(elem, settings);
                    }
                }
            }
            catch (Exception ex)
            {
                xdoc = null;
                throw;
            }
        }


        protected virtual void ConfigureFromXml(XElement rootElement, SoccerSimulatorXmlSettings settings)
        {
            settings.Configure(rootElement);
        }





        protected class SoccerSimulatorXmlSettings : SoccerSimulatorSettings, IXmlConfigurable
        {
            private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);


            public SoccerSimulatorXmlSettings()
            {
                //PlayerAnalysers = new PlayerAnalyserBase[0];
            }
            

            public virtual void Configure(XElement element)
            {
                var settingsNode = element.Element("settings");
                if (settingsNode != null)
                {
                    foreach (var childNode in settingsNode.Elements())
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
                            ApplyAnalyser(childNode);
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


            protected virtual void ApplyAnalyser(XElement element)
            {
                try
                {
                    var typeName = element.GetAttributeValue("type");
                    if (string.IsNullOrWhiteSpace(typeName))
                        throw new FormatException("Invalid analyser type name");
                    var type = Type.GetType(typeName, true);
                    if (!(typeof (PlayerAnalyserBase).IsAssignableFrom(type)))
                        throw new Exception("Specified type doesn't inherit PlayerAnalyserBase");

                    var analyser = (PlayerAnalyserBase) Activator.CreateInstance(type);
                    analyser.Configure(element);
                    //PlayerAnalysers = PlayerAnalysers.Where(x => x.Name != analyser.Name)
                    //                                 .Concat(new[] {analyser})
                    //                                 .ToArray();
                    PlayerAnalysers.Add(analyser);
                }
                catch (Exception ex)
                {
                    _log.Error($"Error when applying analyser", ex);
                }
            }
        }
        
    }
}
