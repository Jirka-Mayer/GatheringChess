using System.Collections.Generic;
using Unisave;
using UnityEngine;

namespace GatheringChess.Entities
{
    /// <summary>
    /// Stores data about chess pieces and decks the player has
    /// </summary>
    public class PlayerCollectionEntity : Entity
    {
        /// <summary>
        /// Pieces owned by the player
        /// (number of instances for each owned piece id)
        /// </summary>
        [X] public Dictionary<PieceId, int> OwnedPieces { get; set; }
            = new Dictionary<PieceId, int>();
    
        /// <summary>
        /// Creates the default collection for a new player
        /// </summary>
        public static PlayerCollectionEntity CreateDefaultCollection()
        {
            var collection = new PlayerCollectionEntity {
                OwnedPieces = {
                    [new PieceId(PieceType.Pawn, PieceEdition.Default)] = 8,
                    [new PieceId(PieceType.Rook, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Knight, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Bishop, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Queen, PieceEdition.Default)] = 1,
                    [new PieceId(PieceType.King, PieceEdition.Default)] = 1
                }
            };

            return collection;
        }
    }
}
