using System.Threading.Tasks;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole
{
    public interface ISoccerSimulationDataFactory
    {
        Task<SoccerSimulationData> Generate();
    }
}
