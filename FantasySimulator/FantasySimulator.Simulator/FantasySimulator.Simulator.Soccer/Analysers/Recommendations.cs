namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PlayerRecommendation : IAnalyserRecommendation
    {
        public PlayerRecommendationType Type { get; set; }
        public double Points { get; set; }

        string IAnalyserRecommendation.Type => Type.ToString();
    }

    public class TeamRecommendation : IAnalyserRecommendation
    {
        public TeamRecommendationType Type { get; set; }
        public double Points { get; set; }

        string IAnalyserRecommendation.Type => Type.ToString();
    }
}