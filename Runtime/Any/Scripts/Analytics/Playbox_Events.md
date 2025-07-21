# Playbox.Analytics.Events Class Documentation

The `Events` static class provides a set of utility methods to track various analytics and gameplay events across multiple analytics providers such as **DevToDev (DTDAnalytics)**, **AppsFlyer**, and **Firebase (Crashlytics & FirebaseAnalytics)**.  
All methods automatically check if the respective SDK is initialized before sending events.

---

## Methods

### `LogLevelUp(int level)`
Logs a player level-up event.
- **Parameters**:  
  - `level` — The new player level.  
- **Behavior**:  
  - Sends the event to DevToDev if initialized (`DTDAnalytics.LevelUp`).  
  - Sends a corresponding AppsFlyer event (`af_level_achieved`).

---

### `LogContentView(string content)`
Tracks a content view event.
- **Parameters**:  
  - `content` — The name or identifier of the viewed content.  
- **Behavior**:  
  - Logs the event locally via `TrackEvent`.

---

### `LogTutorial(string tutorial, ETutorialState stateLevel = ETutorialState.Complete, string step = "none")`
Logs tutorial progression events (start, skipped, completed, step completed).
- **Parameters**:  
  - `tutorial` — The tutorial identifier.  
  - `stateLevel` — The tutorial state (`Start`, `Skipped`, `Complete`, `StepComplete`).  
  - `step` — Step name or identifier (default: `"none"`).  
- **Behavior**:  
  - Tracks the tutorial event with the given state and step.

---

### `TutorialCompleted()`
Sends a tutorial completion event.
- **Behavior**:  
  - Triggers the `"af_tutorial_completion"` event via AppsFlyer.

---

### `AdToCart(int count)`
Logs every 30 ad views as a conversion-like event.
- **Parameters**:  
  - `count` — The number of ad views.  
- **Behavior**:  
  - Sends `"af_add_to_cart"` event to AppsFlyer.

---

### `AdRewardCount(int count)`
Logs the number of rewarded video ad views.
- **Parameters**:  
  - `count` — The number of ad views.  
- **Behavior**:  
  - Sends `"ad_reward"` event to AppsFlyer.

---

### `CurrentBalance(Dictionary<string, long> balance)`
Logs the current player balance (virtual currencies).
- **Parameters**:  
  - `balance` — Dictionary of currency names and their amounts.  
- **Behavior**:  
  - Sends data to DevToDev.

---

### `CurrencyAccrual(string currencyName, int currencyAmount, string source, DTDAccrualType type)`
Logs currency accrual (earnings) events.
- **Parameters**:  
  - `currencyName`, `currencyAmount`, `source`, `type`.  
- **Behavior**:  
  - Sends accrual event to DevToDev.

---

### `RealCurrencyPayment(string orderId, double price, string productId, string currencyCode)`
Logs real-money transactions.
- **Behavior**:  
  - Reports to DevToDev (`RealCurrencyPayment`).

---

### `VirtualCurrencyPayment(string purchaseId, string purchaseType, int purchaseAmount, Dictionary<string, int> resources)`
Logs purchases made with virtual currency.
- **Behavior**:  
  - Sends the transaction to DevToDev.

---

### `AdImpression(string network, double revenue, string placement, string unit)`
Logs an ad impression with revenue data.
- **Behavior**:  
  - Sends the impression to DevToDev.

---

### `Tutorial(int step)`
Tracks a numeric tutorial progression step.
- **Behavior**:  
  - Sends to DevToDev.

---

### `SocialNetworkConnect(DTDSocialNetwork socialNetwork)`
Logs a social network connection.
- **Behavior**:  
  - Reports via DevToDev.

---

### `SocialNetworkPost(DTDSocialNetwork socialNetwork, string reason)`
Logs a social network post event.
- **Behavior**:  
  - Reports via DevToDev.

---

### `Referrer(Dictionary<DTDReferralProperty, string> referrer)`
Tracks referral source data.
- **Behavior**:  
  - Sends referrer information to DevToDev.

---

### `AppsFlyerPayment(Dictionary<string,string> appsFlyerPaymentValues)`
Sends a purchase event specifically for AppsFlyer.
- **Behavior**:  
  - Triggers `"af_purchase"` event.

---

### `StartProgressionEvent(string eventName)`
Starts a progression tracking event.
- **Behavior**:  
  - Sends to DevToDev.

---

### `StartProgressionEvent(string eventName, DTDStartProgressionEventParameters parameters)`
Starts a progression tracking event with custom parameters.
- **Behavior**:  
  - Sends to DevToDev.

---

### `FinishProgressionEvent(string eventName)`
Finishes a progression tracking event.
- **Behavior**:  
  - Sends to DevToDev.

---

### `FinishProgressionEvent(string eventName, DTDFinishProgressionEventParameters parameters)`
Finishes a progression tracking event with custom parameters.
- **Behavior**:  
  - Sends to DevToDev.

---

### `CrashlyticsLog(string eventName, string message)`
Logs custom messages to Firebase Crashlytics.
- **Behavior**:  
  - Sends formatted log if Firebase is initialized.

---

### `FirebaseEvent(string eventName, string message)`
Sends a custom event to Firebase Analytics.
- **Behavior**:  
  - Sends the event if Firebase is initialized.

---
