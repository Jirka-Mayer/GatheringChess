using GatheringChess.Entities;
using Unisave;

namespace GatheringChess.Facets
{
    public class PlayerFacet : Facet
    {
        public PlayerEntity GetPlayerEntity()
        {
            return GetEntity<PlayerEntity>.OfPlayer(Caller).First();
        }

        public PlayerCollectionEntity GetPlayerCollectionEntity()
        {
            return GetEntity<PlayerCollectionEntity>.OfPlayer(Caller).First();
        }

        public PlayerEntity PlayerHasBoughtSmallGems(string receiptId)
        {
            var entity = GetPlayerEntity();
            
            // do some verification here

            DB.Transaction(() => {
                entity.RefreshAndLockForUpdate();
                entity.Gems += 100;
                entity.Save();
            });

            return entity;
        }
    }
}
