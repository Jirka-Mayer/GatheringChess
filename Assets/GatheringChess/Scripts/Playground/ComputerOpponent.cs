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
        public event Action OnOutOfTime;
        
        private PieceColor color;
        private Board board;

        private Clock clock;
        
        public ComputerOpponent(PieceColor color, Board board, float duration)
        {
            this.color = color;
            this.board = board;
            
            clock = new Clock(duration);
        }
        
        public Task WaitForReady()
        {
            return Task.CompletedTask;
        }

        public async void OurMoveWasFinished(ChessMove ourMove)
        {
            clock.StartMe();
            
            for (int x = 0; x < Board.BoardSize; x++)
            for (int y = 0; y < Board.BoardSize; y++)
            {
                Piece piece = board[x, y];
                
                if (piece == null)
                    continue;
                
                if (piece.Id.color != color)
                    continue;

                await Task.Delay(Random.Range(500, 5_000));

                float duration = clock.StopMe();

                if (clock.IsTimeOverForMe())
                {
                    OnOutOfTime?.Invoke();
                    return;
                }
                
                OnMoveFinish?.Invoke(new ChessMove {
                    from = new Vector2Int(x, y),
                    to = FindFreeSpot(),
                    duration = duration
                });
                return;
            }

            throw new Exception();
        }

        public void WeGiveUp()
        {
            //
        }

        public void WeRanOutOfTime()
        {
            //
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