using GatheringChess.Entities;
using GatheringChess.Facets;
using Unisave;
using Unisave.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatheringChess
{
    public class CollectionController : MonoBehaviour
    {
        public Text piecesText;

        public IapController iapController;
        
        private PlayerCollectionEntity collection;
        
        async void Start()
        {
            collection = await OnFacet<PlayerFacet>
                .CallAsync<PlayerCollectionEntity>("GetPlayerCollectionEntity");

            piecesText.text = Serializer
                .ToJson(collection.OwnedPieces)
                .ToString();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void BuyBoosterPack()
        {
            Debug.Log("Buying booster pack...");
            
            // verify state
            
            // call facet that does the verification again and does the exchange
        }

        // buy gems for real money
        public void BuyGems()
        {
            // this is just a sketch,
            // the actual implementation (verification) will most likely differ
            
            iapController.BuySmallGems()
                .Then(receiptId => {
                    
                    // call server facet to
                    // - verify receipt
                    // - increment gem count
                    OnFacet<PlayerFacet>
                        .Call<PlayerEntity>(
                            "PlayerHasBoughtSmallGems",
                            receiptId
                        )
                        .Then((entity) => {
                            Debug.Log("Exchange finished.");
                        })
                        .Done();
                    
                })
                .Done();
        }
    }
}
