using System;
using Photon.Pun;
using Unisave;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Bootstraps and controls a playground scene right after it loads
    /// </summary>
    public class PlaygroundController : MonoBehaviour
    {
        /// <summary>
        /// What match should be played
        /// Value is set by the calling scene
        /// </summary>
        public static MatchEntity matchToStart;

        /// <summary>
        /// Board reference
        /// </summary>
        public Board board;
        
        // time text references
        public Text myTimeText;
        public Text opponentTimeText;

        /// <summary>
        /// Represents the opponent
        /// </summary>
        private IOpponent opponent;

        /// <summary>
        /// Color of player on this computer
        /// </summary>
        private PieceColor playerColor;

        private Clock clock;

        /// <summary>
        /// Is currently running my turn?
        /// </summary>
        private bool myTurn;
        
        private void Start()
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));
            
            if (myTimeText == null)
                throw new ArgumentNullException(nameof(myTimeText));
            
            if (opponentTimeText == null)
                throw new ArgumentNullException(nameof(opponentTimeText));

            if (matchToStart == null)
            {
                StartDebug();
            }
            else
            {
                var match = matchToStart;
                matchToStart = null;
                StartRegular(match);
            }
        }

        private void Update()
        {
            if (clock != null)
            {
                myTimeText.text = Clock.FormatTime(
                    clock.MyDisplayTime
                );
                
                opponentTimeText.text = Clock.FormatTime(
                    clock.OpponentDisplayTime
                );

                if (myTurn && clock.IsTimeOverForMe())
                {
                    // TODO: handle match result
                    OnLeaveMatchButtonClick();
                }
            }
        }

        /// <summary>
        /// Start playground for debugging (when launched right
        /// away from unity editor)
        /// </summary>
        private void StartDebug()
        {
            Debug.LogWarning("No startup match provided, creating a " +
                             "debugging match against the computer.");
        
            var match = new MatchEntity() {
                WhitePlayer = Auth.Player,
                WhitePlayerSet = ChessHalfSet.CreateDefaultHalfSet(
                    PieceColor.White
                ),
                
                BlackPlayer = null,
                BlackPlayerSet = ChessHalfSet.CreateDefaultHalfSet(
                    PieceColor.Black
                )
            };
            
            StartRegular(match);
        }

        /// <summary>
        /// Start playground in a regular fashion when a match is known
        /// </summary>
        private async void StartRegular(MatchEntity match)
        {
            // which player are we?
            playerColor = match.WhitePlayer == Auth.Player
                ? PieceColor.White : PieceColor.Black;
            
            // who is the opponent
            UnisavePlayer opponentPlayer = playerColor.IsWhite()
                ? match.BlackPlayer : match.WhitePlayer;

            // connect to the opponent
            if (opponentPlayer == null)
            {
                // AI
                Debug.Log("Starting computer opponent...");
                opponent = new ComputerOpponent(
                    playerColor.Opposite(),
                    board,
                    match.Duration
                );
            }
            else
            {
                // Real person
                Debug.Log("Connecting to photon opponent...");
                opponent = PhotonOpponent.CreateInstance(match.EntityId);
            }
            
            // create and setup the board
            board.CreateBoard(
                playerColor.IsWhite(),
                match.WhitePlayerSet,
                match.BlackPlayerSet
            );
            
            // create the clock
            clock = new Clock(match.Duration);
            
            // register opponent events
            opponent.OnMoveFinish += OpponentsMoveWasFinished;
            opponent.OnGiveUp += OpponentGaveUp;

            // wait for the opponent
            Debug.Log("Waiting for the opponent...");
            await opponent.WaitForReady();
            
            // === start the game ===
            
            // if we are white, we are the one to start
            Debug.Log("Game has started.");
            if (playerColor.IsWhite())
            {
                PerformOurMove();
            }
            else
            {
                // it's the opponents turn so just wait
                clock.StartOpponent();
            }
        }

        /// <summary>
        /// Let us perform an action
        /// </summary>
        private async void PerformOurMove()
        {
            // === our turn begins ===
            
            myTurn = true;
            
            clock.StartMe();
            
            var move = await board.LetPlayerHaveAMove();
            
            move.duration = clock.StopMe();
            
            // === opponent turn begins ===
            
            myTurn = false;
            
            clock.StartOpponent();
            
            opponent.OurMoveWasFinished(move);
        }

        /// <summary>
        /// Called by the opponent when he finishes a move
        /// </summary>
        private void OpponentsMoveWasFinished(ChessMove move)
        {
            clock.StopOpponent(move.duration);
            
            board.PerformOpponentsMove(move.from, move.to);
            
            // === our turn begins ===
            
            PerformOurMove();
        }

        /// <summary>
        /// Called by the opponent when he gives up the match
        /// </summary>
        private void OpponentGaveUp()
        {
            Debug.Log("Opponent gave up.");
            
            SceneManager.LoadScene("MainMenu");
        }

        public void OnLeaveMatchButtonClick()
        {
            opponent.WeGiveUp();
            
            SceneManager.LoadScene("MainMenu");
        }
    }
}