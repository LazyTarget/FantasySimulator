namespace FantasySimulator.Simulator.Soccer
{
    public class SimulationContext
    {
        private readonly SoccerSimulationData _data;

        internal SimulationContext(SoccerSimulationData data, ISoccerSimulatorSettings settings)
        {
            Settings = settings;
            _data = data;
        }

        public Gameweek LastPlayedGameweek { get; set; }
        public ISoccerSimulatorSettings Settings { get; set; }
    }
}