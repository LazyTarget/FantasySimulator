using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Config
{
    public interface ISoccerSimulatorSettingsFactory
    {
        ISoccerSimulatorSettings GetSettings();
    }
}