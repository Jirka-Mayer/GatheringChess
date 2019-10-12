using RSG;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GatheringChess
{
    public class IapController : MonoBehaviour, IStoreListener
    {
        // Start is called before the first frame update
        void Start()
        {
            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            
            builder.AddProduct(
                "cloud.unisave.gathering_chess.gems_small",
                ProductType.Consumable
            );
            
            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            //
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            //
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            //
        }

        public IPromise<string> BuySmallGems()
        {
            // do stuff here
            
            return Promise<string>.Resolved("some-receipt-id");
        }
    }
}
