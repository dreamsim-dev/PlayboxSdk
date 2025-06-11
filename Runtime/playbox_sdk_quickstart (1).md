# Playbox SDK — Quickstart Guide

Welcome to Playbox SDK for Unity! This guide covers the minimal steps for correct installation and startup of the SDK.

---

## 📦 Installation

### 1️⃣ Connect SDK via Unity Package Manager (UPM)

#### Add package:

Go to `Window` → `Package Manager`, git clone [Playbox SDK](https://github.com/dreamsim-dev/PlayboxSdk.git) and install it.

---

### 2️⃣ Install dependencies

Before using the SDK, you need to install:

- `Newtonsoft.Json`
- `UnityWebRequest`
- `Addressables` (for RemoteConfig)
- `Unity IAP` (for In-App Purchase)

*UPM will pull dependencies automatically, but it's recommended to verify their presence.*

---

## ⚙️ SDK Configuration

### 1️⃣ Create configuration

Create a configuration class:

```csharp
using Playbox;

public class MyPlayboxConfiguration : PlayboxConfiguration
{
    public override string GameId => "YOUR_GAME_ID";
    public override string AppVersion => Application.version;
    public override bool IsSandbox => true;
}
```

---

## 🚀 SDK Initialization

### 1️⃣ Add MainInitialization to the scene

1. In Unity Editor select: `Playbox → Initialization → Create`

2. An object with `MainInitialization` component will be created.

3. In the Inspector, drag the created `MyPlayboxConfiguration` to the configuration field.

> Manual initialization in code is not required.

---

## 🕰️ Working with timer

```csharp
PlayboxTimer timer = new PlayboxTimer();
timer.initialTime = 10.0f;
timer.OnTimeOut += () => Debug.Log("Timer finished!");
timer.Start();
```

---

## 💳 Purchase verification

```csharp
InAppVerification.Validate(
  productID: "your_product_id",
  receipt: receiptString,
  saveId: "purchase123",
  callback: result => {
    if (result) Debug.Log("✅ Purchase verified");
    else Debug.Log("❌ Verification failed");
  }
);
```

---

## ⚙️ CI/CD SmartCma

| Argument                | Description              |
| ----------------------- | ------------------------ |
| `-build-location`       | Build output path        |
| `-build-version`        | Application version      |
| `-build-number`         | Build number             |
| `-keystore-path`        | Android keystore path    |
| `-keystorepass`         | Keystore password        |
| `-keyaliasname`         | Alias name               |
| `-keyaliaspass`         | Alias password           |
| `-code-sign-manual`     | Manual iOS signing       |
| `-provision-profile`    | iOS provision profile    |
| `-code-sign-identity`   | iOS Code Sign Identity   |
| `-profile-development`  | iOS Development Profile  |
| `-profile-distribution` | iOS Distribution Profile |

Example:

```bash
Unity -quit -batchmode -executeMethod BuildCommand \
  -build-location "./Builds/Android" \
  -build-version "1.0.0" \
  -build-number "456"
```

---

## 🔧 SDK Modules

- EventBus
- Analytics
- RemoteConfig
- A/B Testing
- In-App Purchases
- PlayboxTimer
- Server-Side Validation

---

## ❓ FAQ

**Is manual initialization required?** → No. Just create MainInitialization via `Playbox → Initialization → Create`.

**How to change settings?** → Through the created `PlayboxConfiguration` script.

**Do I need to manually add dependencies?** → Only if UPM didn't pull them automatically.

**Is it required to connect all modules?** → No, only use what you need.

---

## 📢 Technical support

- 📧 [support@playbox.network](mailto\:support@playbox.network)
- 📃 Documentation: [https://playbox.network/docs](https://playbox.network/docs)
- 🗨️ Slack / Discord

---

🔜 After completing these steps, your Playbox SDK is ready to use.

