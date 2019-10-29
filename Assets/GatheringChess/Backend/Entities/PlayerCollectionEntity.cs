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
                    [new PieceId(PieceType.Pawn, PieceColor.Black, PieceEdition.Default)] = 8,
                    [new PieceId(PieceType.Rook, PieceColor.Black, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Knight, PieceColor.Black, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Bishop, PieceColor.Black, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Queen, PieceColor.Black, PieceEdition.Default)] = 1,
                    [new PieceId(PieceType.King, PieceColor.Black, PieceEdition.Default)] = 1,
                    
                    [new PieceId(PieceType.Pawn, PieceColor.White, PieceEdition.Default)] = 8,
                    [new PieceId(PieceType.Rook, PieceColor.White, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Knight, PieceColor.White, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Bishop, PieceColor.White, PieceEdition.Default)] = 2,
                    [new PieceId(PieceType.Queen, PieceColor.White, PieceEdition.Default)] = 1,
                    [new PieceId(PieceType.King, PieceColor.White, PieceEdition.Default)] = 1
                }
            };

            return collection;
        }
    }
}
