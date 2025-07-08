using System;
using System.Collections.Generic;
using CI.Utils.Extentions;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Playbox
{
    public class IAP : PlayboxBehaviour, IDetailedStoreListener
    {
        private string environment = "production";
        public static bool IsInitialized => storeController != null && storeExtensionProvider != null;
        
        public static event Action<Product> OnGrantProduct = delegate { };
        /// <summary>
        /// The product does not exist or is not available for purchase
        /// </summary>
        public static event Action<Product, PurchaseFailureDescription> OnProductFailed = delegate { };
        public static event Action<Product> OnRevokeProduct = delegate { };
        
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;
        
        public static IAP Instance { get; private set; }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Init(controller, extensions);
        }
        
        private void Init(IStoreController storeController, IExtensionProvider extension)
        {
            IAP.storeController = storeController;
            IAP.storeExtensionProvider = extension;
            
            ApproveInitialization();
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
            
            try {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);
 
                UnityServices.InitializeAsync(options);
            }
            catch (Exception exception) {
               
            }
            
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            ProductCatalog catalog = ProductCatalog.LoadDefaultCatalog();

            foreach (var product in catalog.allProducts)
            {
                builder.AddProduct(product.id, product.type);
                
                $"Add Product : {product.id} ; type {product.type}".PlayboxInfo("IAP");
            }
            
            UnityPurchasing.Initialize(this, builder);
        }
        
        public static void BuyProduct(string productId)
        {
            if (!IsInitialized)
                "In-App Purchasing is not initialized.".PlayboxInfo("IAP");
            
            Product product = storeController.products.WithID(productId);
                
            if (product is { availableToPurchase: true })
            {
                $"Initiate purchase : {product}.".PlayboxInfo("IAP");
                storeController.InitiatePurchase(product);
                Analytics.LogPurshaseInitiation(product);
            }
            else
            {
                "Product not found or not available for purchase.".PlayboxInfo("IAP");
            }
            
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Product product = purchaseEvent.purchasedProduct;
            
            if (purchaseEvent.purchasedProduct.availableToPurchase)
            {
                $"The purchase is complete: {product.definition.id}".PlayboxInfo("IAP");    
                
                Analytics.LogPurchase(product, (b) =>
                {
                    if (b)
                    {
                        $"Validate Purchase {product.definition.id}".PlayboxInfo("IAP");
                    }
                    else
                    {
                        $"Unvalidate Purchase {product.definition.id}".PlayboxInfo("IAP");
                    }
                });
                
                GrantProduct(product);
            }
            else
            {
                OnProductFailed?.Invoke(product, null);
            }
            
            return PurchaseProcessingResult.Complete;
        }
        
        private void GrantProduct(Product product)
        {
            Debug.Log($"Grant product: {product.definition.id}");
            
            OnGrantProduct?.Invoke(product);
        }

        private void RevokeProduct(Product product)
        {
            Debug.Log($"Revoke product: {product}");
            
            OnRevokeProduct?.Invoke(product);
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            $"Purchase failed: {error}".PlayboxError("IAP");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            $"Purchase failed: {error} | {message}".PlayboxError("IAP");
        }
        
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            OnProductFailed?.Invoke(product, failureDescription);
        }
        
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            product.PlayboxInfo("Purchased Failed");
        }
    }
}