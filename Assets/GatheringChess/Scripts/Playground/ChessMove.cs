using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Represents a move of a chess piece
    /// </summary>
    public class ChessMove
    {
        /// <summary>
        /// Initial piece position
        /// </summary>
        public Vector2Int from;
        
        /// <summary>
        /// Final piece position
        /// </summary>
        public Vector2Int to;
    }
}