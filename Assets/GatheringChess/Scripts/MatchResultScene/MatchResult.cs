namespace GatheringChess.MatchResultScene
{
    /// <summary>
    /// Contains result of a match
    /// </summary>
    public class MatchResult
    {
        private readonly bool weWon;
        private readonly string message;

        public MatchResult(bool weWon, string message)
        {
            this.weWon = weWon;
            this.message = message;
        }

        public string FormatMessage()
        {
            return (weWon ? "VICTORY!" : "You looser...") + "\n" + message;
        }
    }
}