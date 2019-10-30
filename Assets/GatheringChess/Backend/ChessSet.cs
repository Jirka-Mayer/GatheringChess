namespace GatheringChess
{
    /// <summary>
    /// Represents a set of chess pieces that a game can be played with
    /// (like deck in gathering card games)
    /// </summary>
    public class ChessSet // = "card deck"
    {
        /// <summary>
        /// Name of this set
        /// </summary>
        public string name;
        
        // two halves of the set
        public ChessHalfSet whiteHalf = new ChessHalfSet(PieceColor.White);
        public ChessHalfSet blackHalf = new ChessHalfSet(PieceColor.Black);

        /// <summary>
        /// Does this set have all the necessary pieces to play?
        /// </summary>
        public bool IsComplete() => (whiteHalf?.IsComplete() ?? false)
                                    && (blackHalf?.IsComplete() ?? false);
        
        /// <summary>
        /// Are both halves valid?
        /// </summary>
        public bool IsValid() => (whiteHalf?.IsValid() ?? false)
                                 && (blackHalf?.IsValid() ?? false);

        public ChessHalfSet GetHalf(PieceColor color)
        {
            if (color.IsWhite())
                return whiteHalf;

            return blackHalf;
        }

        public static ChessSet CreateDefaultSet()
        {
            var set = new ChessSet();
            set.whiteHalf = ChessHalfSet.CreateDefaultHalfSet(PieceColor.White);
            set.blackHalf = ChessHalfSet.CreateDefaultHalfSet(PieceColor.Black);
            return set;
        }
    }
}