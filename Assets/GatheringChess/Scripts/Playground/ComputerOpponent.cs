using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GatheringChess.Playground
{
    public class ComputerOpponent : IOpponent
    {
        public event Action<ChessMove> OnMoveFinish;
        public event Action OnGiveUp;
        
        private PieceColor color;
        private Board board;
        
        public ComputerOpponent(PieceColor color, Board board)
        {
            this.color = color;
            this.board = board;
        }
        
        public Task WaitForReady()
        {
            return Task.CompletedTask;
        }

        public async void OurMoveWasFinished(ChessMove ourMove)
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
                
                OnMoveFinish?.Invoke(new ChessMove {
                    from = new Vector2Int(x, y),
                    to = FindFreeSpot()
                });
                return;
            }

            throw new Exception();
        }

        public void WeGiveUp()
        {
            throw new NotImplementedException();
        }

        private Vector2Int FindFreeSpot()
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