using System.Threading.Tasks;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IFixtureOddsProvider
    {
        Task<FixtureOdds> GetFixtureOdds(Fixture fixture);
    }
}
