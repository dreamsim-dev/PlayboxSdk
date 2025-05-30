
# PlayboxSDK Configurations

This document describes the configuration management classes for PlayboxSDK integrations.

---

## 📦 Classes

### 🔹 `GlobalPlayboxConfig`
Manages the global configuration for the Playbox SDK, including loading, saving, and handling subconfigurations.

### 🔹 `AppLovinConfiguration`
Provides configuration management for AppLovin SDK integration with Playbox, including saving and loading JSON configurations.

### 🔹 `AppsFlyerConfiguration`
Provides configuration management for AppsFlyer SDK integration with Playbox, including saving and loading JSON configurations.

### 🔹 `DevToDevConfiguration`
Provides configuration management for DevToDev analytics integration with Playbox, including log levels, saving, and loading configurations.

### 🔹 `FacebookSdkConfiguration`
Provides configuration management for Facebook SDK integration with Playbox, including app details and tokens.

---

## 📝 Notes
- Each configuration class manages its own set of parameters (e.g., keys, tokens, app IDs) relevant to the specific SDK.
- Configuration data is saved as JSON using `GlobalPlayboxConfig` as the storage handler.
- Each class supports saving and loading configurations from JSON for persistent storage.

---
