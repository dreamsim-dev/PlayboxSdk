# 📦 PlayboxSDK Integration Guide
*Document in development*

---

## 🚀 Install

### 🔹 1. Prerequisites

#### 📦 AppLovin
- **Download the AppLovin package**  
  [AppLovin Integration Guide](https://developers.applovin.com/en/max/unity/overview/integration/)
- **Import AppLovin into Unity**
- AppLovin will automatically install the External Dependency Manager.

---

#### 📦 AppsFlyer
- **Download AppsFlyer and import it into the Unity project**  
  [AppsFlyerSDK Download Link](https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/releases)
- **Or add via Unity Package Manager**:
  ```
  https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm
  ```
  [AppsFlyer UPM link](https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm)

---

#### 📦 DevToDev
- **Add DevToDev packages via Unity Package Manager**:
    - [DevToDev Analytics](https://github.com/devtodev-analytics/package_Analytics.git)
      ```
      https://github.com/devtodev-analytics/package_Analytics.git
      ```
    - [DevToDev Google](https://github.com/devtodev-analytics/package_Google.git)
      ```
      https://github.com/devtodev-analytics/package_Google.git
      ```

---

#### 📦 Facebook SDK
- [Download Facebook SDK](https://lookaside.facebook.com/developers/resources/?id=FacebookSDK-current.zip)
- **Note:** Adding Facebook SDK may cause conflicts with Unity UI components.  
  To resolve:
    1. Delete the **Facebook Samples** folder.
    2. Use `Reimport All` in Unity.

---

#### 📦 Unity In-App Purchasing
- [Unity Purchasing Documentation](https://docs.unity3d.com/Packages/com.unity.purchasing@4.12/manual/index.html)

---

#### 📦 Firebase
- [Firebase Crashlytics & Firebase Analytics Download](https://firebase.google.com/download/unity)

---

#### 📦 Google Consent 
- [google ads mobile](https://github.com/googleads/googleads-mobile-unity.git?path=packages/com.google.ads.mobile)

---

---
- And lastly just install the package via google upm :
- > https://github.com/dreamsim-dev/PlayboxSdk.git#main
---

## 📝 About PlayboxSDK

**PlayboxSDK consists of the following components:**
1. **CI/CD builder** for Unity.
2. **Unified analytics** (integration of multiple analytics SDKs).
3. **Unified ad management** (multiple ad providers in one abstraction).
4. **Purchase validation** system.
5. **Abstraction over SDKs for purchases**.
6. **Custom log system** for streamlined logging and debugging.

---
