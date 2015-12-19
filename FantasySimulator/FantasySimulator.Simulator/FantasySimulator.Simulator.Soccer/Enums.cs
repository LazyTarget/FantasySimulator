namespace FantasySimulator.Simulator.Soccer
{
    public enum PlayerRecommendationType
    {
        None,
        PlayerQuality,
        PlayerCost,
        PlayerForm,
        PlayerPlaytime,
        PlayerUnavailable,
        ChanceOfPlaying,
    }

    public enum TeamRecommendationType
    {
        None,
        TeamQuality,
        TeamForm,
        FixtureRatingDiff,
        MultipleFixturesInGameweek,
        HomeTeamAdvantage,
        FixtureOdds,
    }


    // a bit focused on Fantasy Premier League...
    public enum PlayerPosition
    {
        None,
        Goalkeeper,
        Defender,
        Midfielder,
        Forward,
    }

    public enum FixtureOutcome
    {
        None,
        Draw,
        HomeTeam,
        AwayTeam,
        Undetermined,
        Postponed,
    }

    public enum TeamFixtureOutcome
    {
        None,
        Undetermined,
        Win,
        Draw,
        Loss,
    }
}