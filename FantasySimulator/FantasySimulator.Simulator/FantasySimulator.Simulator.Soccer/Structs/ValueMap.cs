using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class ValueMap : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, Val> _data = new Dictionary<string, Val>();


        public object this[string key]
        {
            get
            {
                return _data[key];
            }
            set
            {
                Val val;
                if (!TryGetValue(key, out val))
                    val = new Val();
                val.Unit = key;
                val.Value = value;
                _data[key] = val;
            }
        }

        public int Count { get { return _data.Count; } }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public bool TryGetValue(string key, out Val value)
        {
            return _data.TryGetValue(key, out value);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public void Clear()
        {
            _data.Clear();
        }


        #region IEnumerable

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }


    public class Val
    {
        public string Unit { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Unit))
                return string.Format("[{0}]: '{1}'", Unit, Value);
            if (Value != null)
                return Value.ToString();
            return base.ToString();
        }
    }


    public class ValueMapKeys
    {
        // League / Gameweek

        public const string GameweekStarted                                 = "gameweek-started";
        public const string GameweekEnded                                   = "gameweek-ended";
        

        // Fixture
        
        public const string FixtureStarted                                  = "fixture-started";
        public const string FixtureEnded                                    = "fixture-ended";
        public const string EloRatingPlayerTeam                             = "elo-player-team";
        public const string EloRatingOpposingTeam                           = "elo-opposing-team";
        public const string EloRatingTeamDifferance                         = "elo-team-differance";
        public const string EloRatingTeamDifferancePercentage               = "elo-team-differance-percentage";


        // Team

        public const string TotalTeamMinutes                                = "total-minutes-played";
        public const string TotalGamesPlayed                                = "total-games-played";
        public const string TotalGamesWon                                   = "total-games-won";
        public const string TotalGamesDraw                                  = "total-games-draw";
        public const string TotalGamesLoss                                  = "total-games-loss";
        public const string TotalPoints                                     = "total-games-points";
        public const string TotalPointsPerGame                              = "total-points-per-game";

        public const string RecentGamesTeamPlayed                           = "team-recent-games-played";
        public const string RecentGamesTeamWins                             = "team-recent-games-won";
        public const string RecentGamesTeamDraw                             = "team-recent-games-draw";
        public const string RecentGamesTeamLoss                             = "team-recent-games-loss";
        public const string RecentGamesTeamPoints                           = "team-recent-games-points";
        public const string RecentPointsPerGame                             = "team-recent-points-per-game";


        // Player
        public const string PlayerPosition                                  = "player-position";

        public const string PlayerTotalGamesPlayed                          = "player-total-games-played";
        public const string PlayerTotalMinutesPlayed                        = "player-total-minutes-played";
        public const string PlayerTotalMinutesPercentageOfTeam              = "player-total-minute-percentage-of-team";
        public const string PlayerTotalCleansheets                          = "player-total-cleansheets";
        public const string PlayerTotalCleansheetsPercentage                = "player-total-cleansheets-percentage";
        public const string PlayerTotalSaves                                = "player-total-saves";
        public const string PlayerTotalSavesPercentage                      = "player-total-saves-percentage";
        public const string PlayerTotalSavesPerGame                         = "player-total-saves-per-game";

        public const string PlayerRecentGamesPlayed                         = "player-recent-games-played";
        public const string PlayerRecentMinutesPlayed                       = "player-recent-minutes-played";
        public const string PlayerRecentMinutesPercentageOfTeam             = "player-recent-minute-percentage-of-team";
        public const string PlayerRecentCleansheets                         = "player-recent-cleansheets";
        public const string PlayerRecentCleansheetsPercentage               = "player-recent-cleansheets-percentage";
        public const string PlayerRecentSaves                               = "player-recent-saves";
        public const string PlayerRecentSavesPercentage                     = "player-recent-saves-percentage";
        public const string PlayerRecentSavesPerGame                        = "player-recent-saves-per-game";


        // Player Fantasy

        public const string FantasyPlayerUnavailable                        = "fantasyplayer-unavailable";
        public const string FantasyPlayerOriginalPrice                      = "fantasyplayer-original-price";
        public const string FantasyPlayerPurchasePrice                      = "fantasyplayer-purchased-price";
        public const string FantasyPlayerCurrentPrice                       = "fantasyplayer-current-price";
        public const string FantasyPlayerRecentPriceChange                  = "fantasyplayer-recent-price-change";
        public const string FantasyPlayerTotalPriceChange                   = "fantasyplayer-total-price-change";
        public const string FantasyPlayerOwnagePercent                      = "fantasyplayer-ownage-percent";
        public const string FantasyPlayerChanceOfPlayingNextFixture         = "fantasyplayer-chance-of-playing-next-fixture";
        public const string FantasyPlayerTotalBonusPoints                   = "fantasyplayer-total-bonus";
        public const string FantasyPlayerRecentBonusPoints                  = "fantasyplayer-recent-bonus";
        public const string FantasyPlayerRecentForm                         = "fantasyplayer-recent-form";
    }
}
