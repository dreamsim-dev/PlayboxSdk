using System;
using UnityEngine.Purchasing;

namespace Playbox
{
    public class IAPRevoker
    {
        private IExtensionProvider storeExtensionProvider;
        
        public IAPRevoker(IExtensionProvider provider)
        {
            storeExtensionProvider = provider;
        }

        public bool IsRevoked(PurchaseEventArgs purchaseEvent)
        {
            if(storeExtensionProvider == null)
                throw new Exception("Extension provider not initialized.");
#if UNITY_IOS
            return IsIOSRevoked(purchaseEvent);
#endif
            
#if UNITY_ANDROID
            return IsAndroidRevoked(purchaseEvent);
#endif

            throw new Exception("IAP Revoke is not supported.");
        }

        private bool IsAndroidRevoked(PurchaseEventArgs purchaseEvent)
        {
#if UNITY_ANDROID
        
            if (purchaseEvent == null)
                throw new Exception("purchaseEvent is null.");
            
            var receipt = purchaseEvent.purchasedProduct.receipt;
            
#endif
            return false;
        }

        private bool IsIOSRevoked(PurchaseEventArgs purchaseEvent)
        {
#if UNITY_IOS
            var appleExtensions = storeExtensionProvider.GetExtension<IAppleExtensions>();
            if (appleExtensions.GetTransactionReceiptForProduct(purchaseEvent.purchasedProduct) == null)
            {
                return true;
            }
#endif      
            return false;
        }
    }
}