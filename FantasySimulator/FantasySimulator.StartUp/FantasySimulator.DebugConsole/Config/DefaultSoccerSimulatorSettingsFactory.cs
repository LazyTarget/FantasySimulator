using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Config
{
    public class DefaultSoccerSimulatorSettingsFactory : ISoccerSimulatorSettingsFactory
    {
        public ISoccerSimulatorSettings GetSettings()
        {
            var settings = SoccerSimulatorSettings.Default;
            return settings;
        }
    }
}