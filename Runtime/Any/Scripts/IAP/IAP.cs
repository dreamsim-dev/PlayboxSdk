using System;
using System.Collections.Generic;
using CI.Utils.Extentions;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Playbox
{
    public class IAP : PlayboxBehaviour, IDetailedStoreListener
    {
        public static bool IsInitialized => storeController != null && storeExtensionProvider != null;
        
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;
        
        public static IAP Instance { get; private set; }
        
        
        private void Init(IStoreController storeController, IExtensionProvider extension)
        {
            IAP.storeController = storeController;
            IAP.storeExtensionProvider = extension;

            if (IsInitialized)
            {
                ApproveInitialization();
            }
        }

        public override void Initialization()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            UnityPurchasing.Initialize(this, builder);
        }
        
        public static void Purchase(Product product, Action<bool> callback, string payload)
        {
        }
        
        public void BuyProduct(string productId)
        {
            if (IsInitialized)
            {
                Product product = storeController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("Продукт не найден или недоступен для покупки.");
                }
            }
            else
            {
                Debug.Log("In-App Purchasing не инициализирован.");
            }
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
            string productId = purchaseEvent.purchasedProduct.definition.id;
            Debug.Log($"Покупка завершена: {productId}");
            
            

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            product.PlayboxInfo("Purchased Failed");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Init(controller, extensions);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
        }
        
        private void GrantProduct(string productId)
        {
            Debug.Log($"Начисляем продукт: {productId}");
            // Добавление контента пользователю
        }

        private void RevokeProduct(string productId)
        {
            Debug.Log($"Удаляем продукт: {productId}");
            // Убираем контент (например, снимаем монеты)
        }
    }

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