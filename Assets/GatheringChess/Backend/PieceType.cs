using System;

namespace GatheringChess
{
    /// <summary>
    /// Type of a chess piece
    /// </summary>
    public enum PieceType
    {
        Pawn = 0,
        Rook = 1,
        Knight = 2,
        Bishop = 3,
        Queen = 4,
        King = 5
    }

    public static class PieceTypeMethods
    {
        public static char GetLetter(this PieceType type)
        {
            switch (type)
            {
                case PieceType.Pawn: return 'P';
                case PieceType.Rook: return 'R';
                case PieceType.Knight: return 'N';
                case PieceType.Bishop: return 'B';
                case PieceType.Queen: return 'Q';
                case PieceType.King: return 'K';
            }

            throw new Exception($"Unknown piece type {type}");
        }
    }
}