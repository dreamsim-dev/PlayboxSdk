using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Playbox
{
    public class PBInAppManager: MonoBehaviour, IStoreListener
    {
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

    // Список ID продуктов из Unity IAP
    public static string PRODUCT_COINS_PACK = "coins_pack";
    public static string PRODUCT_SUBSCRIPTION = "premium_subscription";

    void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized()) return;
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

  
        builder.AddProduct(PRODUCT_COINS_PACK, ProductType.Consumable);
        builder.AddProduct(PRODUCT_SUBSCRIPTION, ProductType.Subscription);
        
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public void BuyProduct(string productId)
    {
        if (IsInitialized())
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

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
        Debug.Log("In-App Purchasing успешно инициализирован.");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Ошибка инициализации: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Ошибка инициализации: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"Покупка успешно завершена: {args.purchasedProduct.definition.id}");
        if (args.purchasedProduct.definition.id == PRODUCT_COINS_PACK)
        {
            // Добавляем монеты игроку
        }
        else if (args.purchasedProduct.definition.id == PRODUCT_SUBSCRIPTION)
        {
            // Активируем подписку
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Покупка не удалась: {product.definition.id}, Причина: {failureReason}");
    }

    public bool IsProductOwned(string productId)
    {
        if (IsInitialized())
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.hasReceipt)
            {
                return true;
            }
        }
        return false;
    }
    }
}