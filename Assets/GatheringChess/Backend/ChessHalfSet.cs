using System.Linq;

namespace GatheringChess
{
    /// <summary>
    /// Half of a chess set (only one color)
    /// </summary>
    public class ChessHalfSet
    {
        /*
         * FOR WHITE:
         * 
         * [ pawn ] | [ pawn ] | [ pawn ] | [P] | [P] | [ P ] | [ P ] | [ P ]
         * ---------+----------+----------+-----+-----+-------+-------+------
         * [left R] | [left N] | [left B] | [Q] | [K] | [r B] | [r N] | [r R]
         */

        /// <summary>
        /// Color of this half set
        /// </summary>
        public PieceColor color;

        public PieceId[] pawns = new PieceId[8];

        public PieceId leftRook;
        public PieceId leftKnight;
        public PieceId leftBishop;
        public PieceId queen;
        public PieceId king;
        public PieceId rightBishop;
        public PieceId rightKnight;
        public PieceId rightRook;

        public bool IsComplete() =>
            pawns != null && pawns.All(p => p != null)
            && leftRook != null
            && leftKnight != null
            && leftBishop != null
            && queen != null
            && king != null
            && rightBishop != null
            && rightKnight != null
            && rightRook != null;

        public bool IsValid() =>
            (pawns == null || pawns.All(p => p.type == PieceType.Pawn && p.color == color))
            && leftRook == null || (leftRook.type == PieceType.Rook && leftRook.color == color)
            && leftKnight == null || (leftKnight.type == PieceType.Knight && leftKnight.color == color)
            && leftBishop == null || (leftBishop.type == PieceType.Bishop && leftBishop.color == color)
            && queen == null || (queen.type == PieceType.Queen && queen.color == color)
            && king == null || (king.type == PieceType.King && king.color == color)
            && rightBishop == null || (rightBishop.type == PieceType.Bishop && rightBishop.color == color)
            && rightKnight == null || (rightKnight.type == PieceType.Knight && rightKnight.color == color)
            && rightRook == null || (rightRook.type == PieceType.Rook && rightRook.color == color);
        
        public ChessHalfSet(PieceColor color)
        {
            this.color = color;
        }

        public static ChessHalfSet CreateDefaultHalfSet(PieceColor color)
        {
            var edition = PieceEdition.Default;
            
            var set = new ChessHalfSet(color);
            
            for (int i = 0; i < 8; i++)
                set.pawns[i] = new PieceId(PieceType.Pawn, color, edition);
            
            set.king = new PieceId(PieceType.King, color, edition);
            set.queen = new PieceId(PieceType.Queen, color, edition);
            set.leftRook = new PieceId(PieceType.Rook, color, edition);
            set.rightRook = new PieceId(PieceType.Rook, color, edition);
            set.leftKnight = new PieceId(PieceType.Knight, color, edition);
            set.rightKnight = new PieceId(PieceType.Knight, color, edition);
            set.leftBishop = new PieceId(PieceType.Bishop, color, edition);
            set.rightBishop = new PieceId(PieceType.Bishop, color, edition);
            
            return set;
        }
    }
}