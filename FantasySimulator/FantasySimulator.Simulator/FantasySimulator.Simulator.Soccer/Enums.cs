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
        FixtureOdds,
    }

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