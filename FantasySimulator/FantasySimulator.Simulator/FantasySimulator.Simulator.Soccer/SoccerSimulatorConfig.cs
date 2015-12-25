namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulatorConfig
    {
        public ISoccerSimulationDataFactory DataFactory { get; set; }
        public ISoccerSimulatorSettings Settings { get; set; }
    }
}