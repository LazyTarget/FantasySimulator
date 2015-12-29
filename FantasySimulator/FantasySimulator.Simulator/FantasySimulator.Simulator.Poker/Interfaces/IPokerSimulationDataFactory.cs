using System.Threading.Tasks;

namespace FantasySimulator.Simulator.Poker.Interfaces
{
    public interface IPokerSimulationDataFactory
    {
        Task<PokerSimulationData> Generate();
    }
}
