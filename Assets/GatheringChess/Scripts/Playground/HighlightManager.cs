using System;
using System.Collections.Generic;
using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Manages tile and piece highlights
    /// - active piece
    /// - possible moves
    /// - last move
    /// </summary>
    public class HighlightManager : MonoBehaviour
    {
        public GameObject moveHighlightPrefab;
        public GameObject killHighlightPrefab;

        /// <summary>
        /// Board reference
        /// </summary>
        public Board board;
        
        /// <summary>
        /// Are active piece highlights displayed?
        /// </summary>
        private bool activePieceHighlighted;

        /// <summary>
        /// List of highlights related to the active piece
        /// </summary>
        private List<GameObject> activePieceHighlights;

        void Start()
        {
            if (moveHighlightPrefab == null)
                throw new ArgumentNullException(nameof(moveHighlightPrefab));
            
            if (killHighlightPrefab == null)
                throw new ArgumentNullException(nameof(killHighlightPrefab));
            
            if (board == null)
                throw new ArgumentNullException(nameof(board));
        }
        
        /// <summary>
        /// Display highlights related to an active piece
        /// </summary>
        public void HighlightActivePiece(
            Vector2Int activePiecePosition,
            List<ChessMove> possibleMoves
        )
        {
            if (activePieceHighlighted)
                ClearActivePieceHighlight();
            
            activePieceHighlights = new List<GameObject>();
            InstantiateActivePieceHighlightAt(
                moveHighlightPrefab,
                activePiecePosition
            );
            foreach (var move in possibleMoves)
            {
                InstantiateActivePieceHighlightAt(
                    move.kill ? killHighlightPrefab : moveHighlightPrefab,
                    move.target
                );
            }
            
            activePieceHighlighted = true;
        }

        /// <summary>
        /// Clear highlights related to the active piece
        /// </summary>
        public void ClearActivePieceHighlight()
        {
            if (!activePieceHighlighted)
                return;
            
            foreach (var go in activePieceHighlights)
                Destroy(go);
            activePieceHighlights = null;
            
            activePieceHighlighted = false;
        }

        /// <summary>
        /// Creates instance of an active piece highlight
        /// and remembers the instance for later deletion
        /// </summary>
        private void InstantiateActivePieceHighlightAt(
            GameObject prefab,
            Vector2Int position
        )
        {
            var go = board.InstantiatePrefabAt(prefab, position);
            activePieceHighlights.Add(go);
        }
    }
}