using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unisave.Examples.Local.Leaderboard;
using UnityEngine;
using UnityEngine.Serialization;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Contains current board state and manages piece movement
    /// </summary>
    public class Board : MonoBehaviour
    {
        /// <summary>
        /// Size of the chess board
        /// </summary>
        public const int BoardSize = 8;

        /// <summary>
        /// Reference to a tile prefab
        /// </summary>
        public GameObject tilePrefab;
        
        /// <summary>
        /// Reference to a piece prefab
        /// </summary>
        public GameObject piecePrefab;

        /// <summary>
        /// Reference to an active piece manager
        /// </summary>
        public ActivePieceManager activePieceManager;

        /// <summary>
        /// Individual tiles of the board that can be clicked
        /// </summary>
        private Tile[,] tiles; // X: (a, b, ..., h), Y: (1, 2, ..., 8)    [X, Y]
        
        /// <summary>
        /// All pieces that exist on the board
        /// </summary>
        private List<Piece> pieces;
        
        /// <summary>
        /// Chess color that is allowed to be moved by the user
        /// When null, no color can be moved
        /// </summary>
        private PieceColor? colorPlayerCanMove;

        private TaskCompletionSource<ChessMove> playerPerformsMovePromise;

        void Start()
        {
            if (tilePrefab == null)
                throw new ArgumentNullException(nameof(tilePrefab));
            
            if (piecePrefab == null)
                throw new ArgumentNullException(nameof(piecePrefab));
            
            if (activePieceManager == null)
                throw new ArgumentNullException(nameof(activePieceManager));
        }

        private void OnDestroy()
        {
            playerPerformsMovePromise?.SetCanceled();
            playerPerformsMovePromise = null;
        }

        /// <summary>
        /// Call this before using the chess board
        /// </summary>
        public void CreateBoard(bool isPlayerWhite, ChessHalfSet whiteSet, ChessHalfSet blackSet)
        {
            // rotate chessboard 180 deg
            if (!isPlayerWhite)
            {
                transform.Rotate(0, 0, 180, Space.Self);
            }

            // create tiles
            tiles = new Tile[BoardSize, BoardSize];

            for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++)
            {
                var go = InstantiatePrefabAt(tilePrefab, new Vector2Int(x, y));

                var tile = go.GetComponent<Tile>();
                tile.IsWhite = (x % 2 == 0) ^ (y % 2 == 0);
                tile.Position = new Vector2Int(x, y);
                tile.OnClick += TileClicked;

                tiles[x, y] = tile;
            }

            // place pieces
            pieces = new List<Piece>();

            // white
//            for (int i = 0; i < BoardSize; i++)
//                CreatePiece(i, 1, whiteSet.pawns[i]);
        
            CreatePiece(0, 0, whiteSet.leftRook);
            CreatePiece(1, 0, whiteSet.leftKnight);
            CreatePiece(2, 0, whiteSet.leftBishop);
            CreatePiece(3, 0, whiteSet.queen);
            CreatePiece(4, 0, whiteSet.king);
            CreatePiece(5, 0, whiteSet.rightBishop);
            CreatePiece(6, 0, whiteSet.rightKnight);
            CreatePiece(7, 0, whiteSet.rightRook);

            // black
            for (int i = 0; i < BoardSize; i++)
                CreatePiece(i, 6, blackSet.pawns[i]);
        
            CreatePiece(0, 7, blackSet.leftRook);
            CreatePiece(1, 7, blackSet.leftKnight);
            CreatePiece(2, 7, blackSet.leftBishop);
            CreatePiece(3, 7, blackSet.queen);
            CreatePiece(4, 7, blackSet.king);
            CreatePiece(5, 7, blackSet.rightBishop);
            CreatePiece(6, 7, blackSet.rightKnight);
            CreatePiece(7, 7, blackSet.rightRook);
        }

        /// <summary>
        /// Instantiates a prefab as a child
        /// of the board object at proper position
        /// </summary>
        public GameObject InstantiatePrefabAt(
            GameObject prefab,
            Vector2Int position
        )
        {
            GameObject instance = Instantiate(
                prefab,
                new Vector3(
                    position.x - BoardSize / 2 + 0.5f,
                    position.y - BoardSize / 2 + 0.5f,
                    0
                ),
                Quaternion.identity
            );
            instance.transform.SetParent(transform, false);
            instance.transform.rotation = Quaternion.identity;

            return instance;
        }

        /// <summary>
        /// Creates a piece of given id at a given position
        /// </summary>
        private void CreatePiece(int x, int y, PieceId pieceId)
        {
            var go = InstantiatePrefabAt(piecePrefab, new Vector2Int(x, y));

            var piece = go.GetComponent<Piece>();
            piece.pieceId = pieceId;
            piece.SetPosition(x, y);

            pieces.Add(piece);
        }

        /// <summary>
        /// Iterates over all board positions in unspecified order
        /// </summary>
        public static IEnumerable<Vector2Int> IteratePositions()
        {
            for (int y = 0; y < BoardSize; y++)
            for (int x = 0; x < BoardSize; x++)
            {
                yield return new Vector2Int(x, y);
            }
        }
        
        /// <summary>
        /// Returns piece instance at a given position or null
        /// </summary>
        private Piece GetPieceAt(Vector2Int position)
        {
            return pieces.FirstOrDefault(p => p.Position == position);
        }

        /// <summary>
        /// Returns ID of the piece at a given position or null
        /// </summary>
        public PieceId GetPieceIdAt(Vector2Int position)
        {
            return GetPieceAt(position)?.pieceId;
        }
        
        /// <summary>
        /// Called by the tile when it gets clicked
        /// </summary>
        private void TileClicked(Tile tile)
        {
            // clicking works only when pieces can be moved
            if (colorPlayerCanMove == null)
                return;

            // === activating a piece ===

            Piece clickedPiece = GetPieceAt(tile.Position);
            
            if (activePieceManager.ActivePiece == null
                && clickedPiece != null
                && clickedPiece.pieceId.color == colorPlayerCanMove)
            {
                activePieceManager.SetPieceAsActive(clickedPiece, this);
                return;
            }
            
            // continue only if some piece is active
            if (activePieceManager.ActivePiece == null)
                return;
            
            // === clicking on tile that is a possible move ===
            
            ChessMove selectedMove = activePieceManager.GetActivePieceMoveTo(
                tile.Position
            );
            
            if (selectedMove != null)
            {
                // deactivate piece
                activePieceManager.DeactivateActivePiece();
                
                // perform the move
                PerformMove(selectedMove);
                
                // disable further piece movement 
                colorPlayerCanMove = null;
                
                // resolve task
                playerPerformsMovePromise?.SetResult(selectedMove);
                playerPerformsMovePromise = null;
                
                return;
            }

            // === deactivating a piece (clicking where no target exists) ===
            
            activePieceManager.DeactivateActivePiece();
        }

        /// <summary>
        /// Lets the player move pieces of given color
        /// </summary>
        public Task<ChessMove> LetPlayerHaveAMove(PieceColor color)
        {
            if (playerPerformsMovePromise != null)
                throw new InvalidOperationException();
            
            colorPlayerCanMove = color;
            playerPerformsMovePromise = new TaskCompletionSource<ChessMove>();
            return playerPerformsMovePromise.Task;
        }

        /// <summary>
        /// Cancels player movement task
        /// </summary>
        public void CancelLetPlayerHaveAMove()
        {
            colorPlayerCanMove = null;
            activePieceManager.DeactivateActivePiece();
            
            playerPerformsMovePromise?.SetCanceled();
            playerPerformsMovePromise = null;
        }

        /// <summary>
        /// Perform a move
        /// </summary>
        public void PerformMove(ChessMove move)
        {
            Piece pieceToMove = GetPieceAt(move.origin);
            
            if (pieceToMove == null)
                throw new ArgumentException(
                    "There's no piece to be moved " +
                    $"from specified position {move.origin}"
                );

            Piece targetPiece = GetPieceAt(move.target);

            if (targetPiece != null)
            {
                if (!move.kill)
                    throw new ArgumentException(
                        "There's no piece to be killed, but the move " +
                        "suggests there should."
                    );
                
                // kill piece
                KillPieceAt(move.target);
            }

            pieceToMove.MovePieceTo(move.target);
        }

        /// <summary>
        /// Kills piece at given position
        /// </summary>
        public void KillPieceAt(Vector2Int position)
        {
            var piece = GetPieceAt(position);

            if (piece == null)
                throw new ArgumentException(
                    $"There's no piece to kill at {position}"
                );
            
            pieces.Remove(piece);
            
            piece.gameObject.SetActive(false);
            Destroy(piece);
        }
    }
}
