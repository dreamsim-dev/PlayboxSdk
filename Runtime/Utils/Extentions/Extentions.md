
# DebugExtensions

This documentation describes the `DebugExtensions` class within the `CI.Utils.Extensions` namespace. The class provides utility methods for logging and managing log prefixes, tailored for use with the Playbox framework and Unity.

## Namespace
```csharp
CI.Utils.Extensions
```

---

## Methods

### Prefix Management

#### `BeginPrefixZone(string prefix)`
Starts a prefix zone for logging, pushing the current prefix onto a stack.

#### `EndPrefixZone()`
Ends the current prefix zone by popping the previous prefix from the stack.

#### `ClearPrefixes()`
Clears all prefixes and resets the current prefix.

---

### Playbox Logging

#### `PlayboxSplashLogUGUI(this object obj)`
Logs a message to the Playbox UGUI logger.

#### `PlayboxSplashLog(this object obj)`
Logs a message to the Playbox logger.

---

### Detailed Logging Methods

Each logging method formats the log message with color, prefix, description, and a category (predicate).

| Method                          | Description                                      | Log Type |
|---------------------------------|--------------------------------------------------|----------|
| `PlayboxLog(this object, string)` | Logs a message with default color (white).      | Log      |
| `PlayboxError(this object, string)` | Logs an error message in red.                  | Error    |
| `PlayboxException(this object, string)` | Logs an exception with details in red.        | Exception |
| `PlayboxWarning(this object, string)` | Logs a warning message in yellow.             | Warning  |
| `PlayboxInfo(this object, string)` | Logs an informational message in gray.         | Info     |
| `PlayboxInitialized(this object, string)` | Logs an "initialized" message in green.       | Initialized |

---

### "D" Variants (Return Original Text)

These methods perform the same logging as above but return the original text (`text.ToString()`), which can be useful for inline chaining.

| Method                                  | Description                                      | Log Type |
|-----------------------------------------|--------------------------------------------------|----------|
| `PlayboxLogD(this object, string)`      | Logs a message with default color (white).      | Log      |
| `PlayboxErrorD(this object, string)`    | Logs an error message in red.                  | Error    |
| `PlayboxExceptionD(this object, string)`| Logs an exception with details in red.         | Exception |
| `PlayboxWarningD(this object, string)`  | Logs a warning message in yellow.              | Warning  |
| `PlayboxInfoD(this object, string)`     | Logs an informational message in gray.         | Info     |
| `PlayboxInitializedD(this object, string)` | Logs an "initialized" message in green.     | Initialized |

---

### Private Method

#### `PlayboxLogger(Color color, object text, Action<string> action, string predicate, string description, bool isException)`
Formats the log message with a category (predicate), description, prefix, and color. Used internally by public logging methods.

---

## Usage Example

```csharp
// Start a prefix zone
DebugExtensions.BeginPrefixZone("GameStart");

// Log different levels
"Game Initialized".PlayboxInitialized("Starting up");
"An error occurred".PlayboxError("Error detail");

// End the prefix zone
DebugExtensions.EndPrefixZone();
```

---

## Notes
- Prefixes allow for contextual grouping of logs, useful during debugging of complex systems.
- Logging methods are intended for use in **debug builds** (`Debug.isDebugBuild`).
- The methods leverage Unity's `Debug` class and Playbox-specific loggers for display.

---
