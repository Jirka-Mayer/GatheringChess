using System;

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

        public static PieceColor Opposite(this PieceColor color)
            => color == PieceColor.White ? PieceColor.Black : PieceColor.White;

        public static char GetLetter(this PieceColor color)
        {
            switch (color)
            {
                case PieceColor.White: return 'W';
                case PieceColor.Black: return 'B';
            }
            
            throw new Exception($"Unknown piece color {color}");
        }
    }
}