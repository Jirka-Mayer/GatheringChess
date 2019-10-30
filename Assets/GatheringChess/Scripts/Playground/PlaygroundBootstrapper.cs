using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Bootstraps a playground scene right after it loads
    /// </summary>
    public class PlaygroundBootstrapper : MonoBehaviour
    {
        /// <summary>
        /// Start the playground in debug mode
        /// </summary>
        public bool debug;

        /// <summary>
        /// Board reference
        /// </summary>
        public Board board;

        /// <summary>
        /// Represents the opponent
        /// </summary>
        private IOpponent opponent;

        /// <summary>
        /// Color of player on this computer
        /// </summary>
        private PieceColor playerColor;
        
        private void Start()
        {
            if (board == null)
            {
                Debug.LogError(
                    "Board reference for playground bootstrapper is missing."
                );
                return;
            }
            
            if (debug)
                StartDebug();
        }

        private void StartDebug()
        {
            // let us have a chess half sets for both players obtained from match entity
            var whiteSet = ChessHalfSet.CreateDefaultHalfSet(PieceColor.White);
            var blackSet = ChessHalfSet.CreateDefaultHalfSet(PieceColor.Black);
            
            // white set has different king and queen
            whiteSet.king = new PieceId(PieceType.King, PieceColor.White, PieceEdition.ManRay);
            whiteSet.queen = new PieceId(PieceType.Queen, PieceColor.White, PieceEdition.ManRay);

            playerColor = PieceColor.White;

            opponent = new ComputerOpponent(playerColor.Opposite());
            board.CreateBoard(playerColor.IsWhite(), whiteSet, blackSet);
            
            RunGame();
        }

        /// <summary>
        /// Performs the match
        /// </summary>
        private async void RunGame()
        {
            if (playerColor.IsWhite())
            {
                await board.LetPlayerHaveAMove();
            }
            
            while (true) // while not game over
            {
                ChessMove move = await opponent.PerformMove(board);
                board.PerformOpponentsMove(move.from, move.to);
                
                await board.LetPlayerHaveAMove();
            }
        }
    }
}