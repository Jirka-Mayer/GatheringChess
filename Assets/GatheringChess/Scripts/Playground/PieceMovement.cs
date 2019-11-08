using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Computes possible moves
    /// </summary>
    public static class PieceMovement
    {
        /// <summary>
        /// Position of the subject piece
        /// </summary>
        private static Vector2Int subjectPosition;

        /// <summary>
        /// Id of the subject piece
        /// </summary>
        private static PieceId subjectId;
        
        /// <summary>
        /// Board reference
        /// </summary>
        private static Board board;

        /// <summary>
        /// Move buffer
        /// </summary>
        private static List<ChessMove> moves;
        
        public static List<ChessMove> GetPossibleMoves(
            Vector2Int subjectPosition,
            PieceId subjectId,
            Board board
        )
        {
            PieceMovement.subjectPosition = subjectPosition;
            PieceMovement.subjectId = subjectId;
            PieceMovement.board = board;

            moves = new List<ChessMove>();
            
            switch (subjectId.type)
            {
                case PieceType.Pawn:
                    PawnMovement();
                    break;
                
                case PieceType.Rook:
                    RookMovement();
                    break;
                
                case PieceType.Bishop:
                    BishopMovement();
                    break;
                
                case PieceType.Knight:
                    KnightMovement();
                    break;
                
                case PieceType.Queen:
                    QueenMovement();
                    break;
                
                case PieceType.King:
                    KingMovement();
                    break;
                
                default:
                    FakeMovement();
                    break;
            }

            PieceMovement.subjectPosition = default(Vector2Int);
            PieceMovement.subjectId = null;
            PieceMovement.board = null;

            var ret = moves;
            moves = null;
            return ret;
        }

        private static void PawnMovement()
        {
            // is in starting position
            var starting = subjectId.color.IsWhite()
                ? subjectPosition.y == 1
                : subjectPosition.y == Board.BoardSize - 2;

            var forward = subjectId.color.IsWhite()
                ? Vector2Int.up : Vector2Int.down;

            // movement
            if (TryMove(subjectPosition + forward))
            {
                if (starting)
                {
                    TryMove(subjectPosition + forward * 2);
                }
            }
            
            // killing
            TryKill(subjectPosition + forward + Vector2Int.left);
            TryKill(subjectPosition + forward + Vector2Int.right);
            
            // TODO: em passant
        }
        
        private static void RookMovement(int limit = Board.BoardSize)
        {
            SlideMovement(Vector2Int.up, limit);
            SlideMovement(Vector2Int.down, limit);
            SlideMovement(Vector2Int.left, limit);
            SlideMovement(Vector2Int.right, limit);
        }

        private static void KnightMovement()
        {
            void TryJump(Vector2Int delta)
            {
                TryMove(subjectPosition + delta);
                TryKill(subjectPosition + delta);
            }
            
            TryJump(Vector2Int.up * 2 + Vector2Int.left);
            TryJump(Vector2Int.up * 2 + Vector2Int.right);
            
            TryJump(Vector2Int.down * 2 + Vector2Int.left);
            TryJump(Vector2Int.down * 2 + Vector2Int.right);
            
            TryJump(Vector2Int.left * 2 + Vector2Int.up);
            TryJump(Vector2Int.left * 2 + Vector2Int.down);
            
            TryJump(Vector2Int.right * 2 + Vector2Int.up);
            TryJump(Vector2Int.right * 2 + Vector2Int.down);
        }
        
        private static void BishopMovement(int limit = Board.BoardSize)
        {
            SlideMovement(Vector2Int.up + Vector2Int.left, limit);
            SlideMovement(Vector2Int.down + Vector2Int.left, limit);
            SlideMovement(Vector2Int.up + Vector2Int.right, limit);
            SlideMovement(Vector2Int.down + Vector2Int.right, limit);
        }

        private static void QueenMovement(int limit = Board.BoardSize)
        {
            RookMovement(limit);
            BishopMovement(limit);
        }
        
        private static void KingMovement()
        {
            QueenMovement(1);
        }

        /// <summary>
        /// Linearly slide in a direction, try to move and try to kill the
        /// piece that we hit at the end
        /// </summary>
        private static void SlideMovement(
            Vector2Int direction,
            int limit = Board.BoardSize
        )
        {
            for (int i = 1; i <= limit; i++)
            {
                if (!TryMove(subjectPosition + direction * i))
                {
                    TryKill(subjectPosition + direction * i);
                    break;
                }
            }
        }

        /// <summary>
        /// Try to add a move that simply moves to this position
        /// </summary>
        private static bool TryMove(Vector2Int targetPosition)
        {
            // board boundaries
            if (!InsideBoard(targetPosition))
                return false;
            
            // taken by some piece
            if (IsOccupied(targetPosition))
                return false;
            
            moves.Add(new ChessMove {
                origin = subjectPosition,
                target = targetPosition,
                kill = false
            });

            return true;
        }
        
        /// <summary>
        /// Try to add a move that kills enemy piece at given position
        /// </summary>
        private static bool TryKill(Vector2Int targetPosition)
        {
            // board boundaries
            if (!InsideBoard(targetPosition))
                return false;
            
            // not taken by enemy piece
            if (!IsEnemy(targetPosition))
                return false;
            
            moves.Add(new ChessMove {
                origin = subjectPosition,
                target = targetPosition,
                kill = true
            });

            return true;
        }
        
        /// <summary>
        /// Returns true if there's any kind of piece piece at the position
        /// </summary>
        private static bool IsOccupied(Vector2Int position)
        {
            PieceId pieceIdAtPos = board.GetPieceIdAt(position);

            if (pieceIdAtPos == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if there's an enemy piece at the position
        /// </summary>
        private static bool IsEnemy(Vector2Int position)
        {
            PieceId pieceIdAtPos = board.GetPieceIdAt(position);

            if (pieceIdAtPos == null)
                return false;
            
            return pieceIdAtPos.color != subjectId.color;
        }
        
        /// <summary>
        /// Returns true if there's a friendly piece at the position
        /// </summary>
        private static bool IsFriend(Vector2Int position)
        {
            PieceId pieceIdAtPos = board.GetPieceIdAt(position);

            if (pieceIdAtPos == null)
                return false;
            
            return pieceIdAtPos.color == subjectId.color;
        }

        /// <summary>
        /// Returns true if the position is inside board borders
        /// </summary>
        private static bool InsideBoard(Vector2Int position)
        {
            if (position.x < 0 || position.x >= Board.BoardSize)
                return false;
            
            if (position.y < 0 || position.y >= Board.BoardSize)
                return false;

            return true;
        }

        private static void FakeMovement()
        {
            foreach (Vector2Int pos in Board.IteratePositions())
            {
                if (pos == subjectPosition)
                    continue;

                if ((pos - subjectPosition).magnitude > 2f)
                    continue;

                TryMove(pos);
                TryKill(pos);
            }
        }
    }
}