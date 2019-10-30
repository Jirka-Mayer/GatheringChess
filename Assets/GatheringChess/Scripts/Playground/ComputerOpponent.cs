using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GatheringChess.Playground
{
    public class ComputerOpponent : IOpponent
    {
        private PieceColor color;
        
        public ComputerOpponent(PieceColor color)
        {
            this.color = color;
        }
        
        public async Task<ChessMove> PerformMove(Board board)
        {
            for (int x = 0; x < Board.BoardSize; x++)
            for (int y = 0; y < Board.BoardSize; y++)
            {
                Piece piece = board[x, y];
                
                if (piece == null)
                    continue;
                
                if (piece.Id.color != color)
                    continue;

                await Task.Delay(500);
                
                return new ChessMove {
                    from = new Vector2Int(x, y),
                    to = FindFreeSpot(board)
                };
            }

            throw new Exception();
        }

        public Vector2Int FindFreeSpot(Board board)
        {
            while (true)
            {
                int x = Random.Range(0, Board.BoardSize - 1);
                int y = Random.Range(0, Board.BoardSize - 1);
                
                if (board[x, y] == null)
                    return new Vector2Int(x, y);
            }
        }
    }
}