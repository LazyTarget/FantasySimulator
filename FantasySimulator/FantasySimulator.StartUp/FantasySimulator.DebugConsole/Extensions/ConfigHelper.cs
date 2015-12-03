using System.Configuration;
using System.Linq;

namespace FantasySimulator.DebugConsole
{
    public static class ConfigHelper
    {
        public static ConfigurationSection GetSectionByType(this Configuration configuration, string type)
        {
            foreach (var section in configuration.Sections.OfType<ConfigurationSection>())
            {
                if (section != null && section.SectionInformation.Type == type)
                {
                    return section;
                }
            }
            return null;
        }

    }
}
