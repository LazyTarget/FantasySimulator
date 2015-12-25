using System;
using System.Collections.Generic;
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

namespace FantasySimulator.DebugConsole.Data
{
    public abstract class XmlConfigFactoryBase<TResult> : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public XmlConfigFactoryBase()
        {
            Properties = new Dictionary<string, object>();

            var configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

            ConfigUri = new Uri(configPath);
            //RootElementName = "soccerSimulatorData";
        }

        public IDictionary<string, object> Properties { get; }


        public string RootElementName
        {
            get { return Properties["RootElementName"].SafeConvert<string>(); }
            set { Properties["RootElementName"] = value; }
        }

        public Uri ConfigUri
        {
            get
            {
                var temp = Properties["ConfigUri"];
                if (temp is Uri)
                {
                    return (Uri) temp;
                }
                var str = temp.SafeConvert<string>();
                var uri = new Uri(str);
                return uri;
            }
            set { Properties["ConfigUri"] = value; }
        }


        public void Configure(XElement element)
        {
            
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


        protected virtual void ConfigureFromStream(Stream stream, TResult result)
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
                        ConfigureFromXml(elem, result);
                    }
                }
            }
            catch (Exception ex)
            {
                xdoc = null;
                throw;
            }
        }


        protected abstract void ConfigureFromXml(XElement rootElement, TResult result);

    }
}
