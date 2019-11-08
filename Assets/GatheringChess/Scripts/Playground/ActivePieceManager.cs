using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Tracks the active piece and handles the surrounding logic
    /// </summary>
    public class ActivePieceManager : MonoBehaviour
    {
        /// <summary>
        /// The active piece
        /// </summary>
        public Piece ActivePiece { get; private set; }

        /// <summary>
        /// Possible moves of the active piece
        /// </summary>
        private List<ChessMove> possibleMoves;

        /// <summary>
        /// Reference to a highlight manager
        /// </summary>
        public HighlightManager highlightManager;

        void Start()
        {
            if (highlightManager == null)
                throw new ArgumentNullException(nameof(highlightManager));
        }
        
        /// <summary>
        /// Sets given piece as active, null acts like deselection
        /// </summary>
        public void SetPieceAsActive(Piece piece, Board board)
        {
            if (piece == null)
            {
                DeactivateActivePiece();
                return;
            }
            
            // === activate ===

            ActivePiece = piece;
            
            // === list possible moves ===

            possibleMoves = PieceMovement.GetPossibleMoves(
                piece.Position, piece.pieceId, board
            );
            
            // === update board highlights ===
            
            highlightManager.HighlightActivePiece(
                ActivePiece.Position,
                possibleMoves
            );
        }

        /// <summary>
        /// Deactivates current active piece
        /// </summary>
        public void DeactivateActivePiece()
        {
            // === deactivate ===
            
            ActivePiece = null;
            
            // === dump possible moves ===
            
            possibleMoves = null;

            // === update board highlights ===

            highlightManager.ClearActivePieceHighlight();
        }

        /// <summary>
        /// Returns a move that the active piece
        /// could make to end up at a given position.
        /// Returns null if no such move exist.
        /// </summary>
        public ChessMove GetActivePieceMoveTo(Vector2Int position)
        {
            return possibleMoves.FirstOrDefault(m => m.target == position);
        }
    }
}