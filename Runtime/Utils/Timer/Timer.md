
# Playbox Timer

> **PlayboxTimer** is designed to simplify working with timers by hiding low-level interactions with time variables.

## Quick Reference
**Class Documentation**: [PlayboxTimer Documentation](@ref Utils.Timer.PlayboxTimer)

---

## Creating and Configuring the Timer

Before using **PlayboxTimer**, create an instance:

```csharp
PlayboxTimer timer = new PlayboxTimer();
```

### Setting the Initial Time (Optional)

You can set the initial time of the timer in seconds:

```csharp
timer.initialTime = 5.0f;  // The timer will start with 5 seconds
```

---

## Working with Events

**PlayboxTimer** supports several callbacks for convenience.

- More about events:  
  [PlayboxTimer Events](@ref Utils.Timer.PlayboxTimer.OnTimerStart)

### Example Usage of OnTimeOut Event

The `OnTimeOut` event is triggered when the timer finishes counting down:

```csharp
timer.OnTimeOut += () => { Debug.Log("TimeOut"); };
```

---

## Starting the Timer

To start the timer, call the `Start()` method:

```csharp
timer.Start();
```

---

## Complete Example

```csharp
// Create a timer
PlayboxTimer timer = new PlayboxTimer();

// Set initial time (optional)
timer.initialTime = 5.0f;

// Subscribe to the timeout event
timer.OnTimeOut += () => { Debug.Log("TimeOut"); };

// Start the timer
timer.Start();
```

---

## Notes
- The `OnTimeOut` event fires once when the countdown reaches zero.
- To restart the timer after it finishes, you need to call `Start()` again.
- Other timer events are available for advanced usage (see the documentation linked above).

---
