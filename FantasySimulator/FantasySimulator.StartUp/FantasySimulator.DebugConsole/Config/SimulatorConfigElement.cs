using System.Configuration;

namespace FantasySimulator.DebugConsole.Config
{
    public class SimulatorConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("Type", IsKey = true, IsRequired = true)]
        public string Type
        {
            get { return (string)this["Type"]; }
            set { this["Type"] = value; }
        }

        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = true)]
        public bool Enabled
        {
            get { return (bool)this["Enabled"]; }
            set { this["Enabled"] = value; }
        }

        [ConfigurationProperty("ConfigUri", IsRequired = true)]
        public string ConfigUri
        {
            get { return (string)this["ConfigUri"]; }
            set { this["ConfigUri"] = value; }
        }

        [ConfigurationProperty("RootElementName", IsRequired = false)]
        public string RootElementName
        {
            get { return (string)this["RootElementName"]; }
            set { this["RootElementName"] = value; }
        }
    }
}
