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
        public Vector2Int origin;
        
        /// <summary>
        /// Final piece position
        /// </summary>
        public Vector2Int target;

        /// <summary>
        /// Is it expected that there's a piece at the target to be killed?
        /// </summary>
        public bool kill;

        /// <summary>
        /// How many seconds did the move take
        /// (if it already happened, otherwise has no meaning)
        /// (used for clock synchronization)
        /// </summary>
        public float duration;
    }
}