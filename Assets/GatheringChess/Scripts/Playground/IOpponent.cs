using System;
using System.Threading.Tasks;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Interface we have with the opponent
    /// </summary>
    public interface IOpponent
    {
        /// <summary>
        /// Fired when the opponent finishes a move and gives control to us
        /// </summary>
        event Action<ChessMove> OnMoveFinish;
        
        /// <summary>
        /// Fired when the opponent gives up the match
        /// </summary>
        event Action OnGiveUp;
        
        /// <summary>
        /// Fired when the opponent runs out of time
        /// </summary>
        event Action OnOutOfTime;

        /// <summary>
        /// Waits for the opponent to get ready
        /// and also signals that we are ready
        /// </summary>
        Task WaitForReady();

        /// <summary>
        /// We've finished our move and we give control to the opponent
        /// </summary>
        void OurMoveWasFinished(ChessMove ourMove);

        /// <summary>
        /// Call to inform the opponent that we give up the match
        /// </summary>
        void WeGiveUp();
        
        /// <summary>
        /// Call to inform the opponent that we ran out of time
        /// </summary>
        void WeRanOutOfTime();
    }
}