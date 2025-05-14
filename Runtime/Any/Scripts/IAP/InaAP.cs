using System;
using CI.Utils.Extentions;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Playbox
{
    public class InaAP : PlayboxBehaviour, IDetailedStoreListener
    {
        public static bool IsInitialized => storeController != null && extension != null;
        
        private static IStoreController storeController;
        private static IExtensionProvider extension;
        
        public static InaAP Instance { get; private set; }
        
        
        private static void Init(IStoreController storeController, IExtensionProvider extension)
        {
            InaAP.storeController = storeController;
            InaAP.extension = extension;
        }

        public override void Initialization()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            //UnityPurchasing.Initialize(this, builder);
        }
        
        public static void Purchase(Product product, Action<bool> callback, string payload)
        {
            PlayerPrefs.SetString("purshase_payload", payload);
            PlayerPrefs.Save();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            $"Purchase failed: {error}".PlayboxError("IAP");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            $"Purchase failed: {error} | {message}".PlayboxError("IAP");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            purchaseEvent.purchasedProduct.PlayboxInfo("Purchased");
            
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            product.PlayboxInfo("Purchased Failed");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            InaAP.Init(controller, extensions);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
        }
    }

    
}