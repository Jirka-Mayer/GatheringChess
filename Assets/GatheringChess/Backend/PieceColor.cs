namespace GatheringChess
{
    public enum PieceColor
    {
        White = 0,
        Black = 1,
    }

    public static class PieceColorMethods
    {
        public static bool IsWhite(this PieceColor color)
            => color == PieceColor.White;
        
        public static bool IsBlack(this PieceColor color)
            => color == PieceColor.Black;
    }
}