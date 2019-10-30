using System.Threading.Tasks;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Represents an opponent that makes moves
    /// </summary>
    public interface IOpponent
    {
        Task<ChessMove> PerformMove(Board board);
    }
}