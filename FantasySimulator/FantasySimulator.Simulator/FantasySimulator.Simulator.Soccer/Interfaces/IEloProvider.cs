using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IEloProvider
    {
        Task<EloRating> GetRating(DateTime time, Team team);

        Task<IEnumerable<EloRating>> GetRatings(DateTime time, IEnumerable<Team> teams);
    }
}
