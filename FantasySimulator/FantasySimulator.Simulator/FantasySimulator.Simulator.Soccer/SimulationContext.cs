namespace FantasySimulator.Simulator.Soccer
{
    internal class SimulationContext
    {
        private readonly SoccerSimulationData _data;

        public SimulationContext(SoccerSimulationData data)
        {
            _data = data;
        }

        public Gameweek LastPlayedGameweek { get; set; }
    }
}