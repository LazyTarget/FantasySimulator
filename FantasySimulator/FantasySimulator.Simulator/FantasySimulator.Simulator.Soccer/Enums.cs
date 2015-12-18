namespace FantasySimulator.Simulator.Soccer
{
    public enum RecommendationType
    {
        None,
        TeamQuality,
        TeamForm,
        PlayerQuality,
        PlayerCost,
        PlayerForm,
        PlayerPlaytime,
        FixtureRatingDiff,
        MultipleFixturesInGameweek,
        HomeTeamAdvantage,
        PlayerUnavailable,
        ChanceOfPlaying,
    }

    public enum PlayerPosition
    {
        None,
        Goalkeeper,
        Defender,
        Midfielder,
        Forward,
    }

    public enum FixtureWinner
    {
        None,
        Undetermined,
        Draw,
        HomeTeam,
        AwayTeam,
    }

    public enum GameWinState
    {
        None,
        Undetermined,
        Win,
        Draw,
        Loss,
    }
}