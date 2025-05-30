
# EventBus System

This documentation covers the **EventBus** system and its supporting classes in the `EventBusSystem` namespace. The system provides a mechanism for broadcasting events to multiple subscribers in a decoupled manner.

---

## Namespace
```csharp
EventBusSystem
```

---

## Interfaces

### `IGlobalSubscriber`
```csharp
public interface IGlobalSubscriber
```
A marker interface that identifies classes as eligible for subscribing to events in the `EventBus` system.

---

## Classes

### `EventBus`
```csharp
public static class EventBus
```
A static class that manages event subscription, unsubscription, and broadcasting.

#### Methods
- `Subscribe(IGlobalSubscriber subscriber)`
    - Subscribes an object to events of the interfaces it implements.

- `Unsubscribe(IGlobalSubscriber subscriber)`
    - Unsubscribes an object from events of the interfaces it implements.

- `RaiseEvent<TSubscriber>(Action<TSubscriber> action) where TSubscriber : class, IGlobalSubscriber`
    - Broadcasts an event to all subscribers of type `TSubscriber`.
    - Executes the provided `action` on each subscriber.
    - Handles exceptions and logs them using `Debug.LogError`.

---

### `SubscribersList<TSubscriber>`
```csharp
internal class SubscribersList<TSubscriber> where TSubscriber : class
```
Maintains a list of subscribers with support for safe modifications during iteration.

#### Properties
- `List<TSubscriber> List`  
  The internal list of subscribers.

- `bool Executing`  
  Indicates whether the list is currently being iterated.

#### Methods
- `Add(TSubscriber subscriber)`  
  Adds a subscriber to the list.

- `Remove(TSubscriber subscriber)`  
  Removes a subscriber, or marks it for cleanup if iteration is in progress.

- `Cleanup()`  
  Removes all `null` entries from the list.

---

### `EventBusHelper`
```csharp
internal static class EventBusHelper
```
Provides helper methods for the `EventBus`.

#### Methods
- `GetSubscriberTypes(IGlobalSubscriber globalSubscriber) : List<Type>`  
  Returns a list of interfaces implemented by `globalSubscriber` that also implement `IGlobalSubscriber`.
  Uses caching for performance.

---

## Example Usage
```csharp
// Define a subscriber interface
public interface IMyEventSubscriber : IGlobalSubscriber
{
    void OnMyEvent();
}

// Implement the subscriber
public class MyComponent : MonoBehaviour, IMyEventSubscriber
{
    public void OnMyEvent()
    {
        Debug.Log("MyEvent received!");
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }
}

// Raise the event somewhere
EventBus.RaiseEvent<IMyEventSubscriber>(s => s.OnMyEvent());
```

---

## Notes
- The system supports multiple interfaces per subscriber.
- `EventBus` is thread-unsafe; use caution in multi-threaded contexts.
- `SubscribersList` ensures safe modifications to the list during event broadcasting.

---
