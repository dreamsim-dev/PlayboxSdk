# Purchase Validation

To validate in-game purchases, use the static method:

```csharp
Playbox.Analytics.LogPurchase(product, validateCallback);
```

### Parameters
- **`product`** — the purchase object (purchase data, for example `Product` from Unity IAP).
- **`validateCallback`** — a delegate (callback) triggered after the purchase validation.
  - The callback receives the validation result (success/failure).
  - On successful validation, the method will automatically send the event to analytics.

### Usage Example

```csharp
Playbox.Analytics.LogPurchase(product, (isValid) =>
{
    if (isValid)
    {
        Debug.Log("Purchase successfully validated and sent to analytics.");
    }
    else
    {
        Debug.LogWarning("Purchase validation failed.");
    }
});
```

Thus, the method both validates the purchase and, in case of success, logs it to analytics.
