using System.Configuration;

namespace FantasySimulator.DebugConsole.Config
{
    public class SimulatorRunnerConfigSection : ConfigurationSection
    {
        public static SimulatorRunnerConfigSection LoadFromConfig()
        {
            var settings = ConfigurationManager.GetSection("simulationRunner") as SimulatorRunnerConfigSection;
            settings = settings ?? new SimulatorRunnerConfigSection();
            return settings;
        }


        [ConfigurationProperty("runners", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(SimulatorConfigElementCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public SimulatorConfigElementCollection Runners
        {
            get
            {
                var o = base["runners"];
                var res = (SimulatorConfigElementCollection)o;
                return res;
            }
        }

    }
}
