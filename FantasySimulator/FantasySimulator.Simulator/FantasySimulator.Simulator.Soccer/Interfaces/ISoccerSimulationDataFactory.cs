using System.Threading.Tasks;

namespace FantasySimulator.Simulator.Soccer
{
    public interface ISoccerSimulationDataFactory
    {
        Task<SoccerSimulationData> Generate();
        Task<SoccerSimulationData> GenerateApplyTo(SoccerSimulationData data);
    }
}
