# Logging
A lightweight, configurable logging package.

# Usage
## Logging
```csharp
Log.Combat.Info?.Log($"{amount} items were picked up.");
Log.Combat.Warning?.Log("A suspicious exception happened.", someException);
Log.Combat.Error?.Log(someException);
```
- `Log.Combat` is an example `LogCategory`. These categories are defined per project (see *Setup*).
- Always use the `?.` operator before a `Log` call, because the properties before it might return `null`.
  - This allows us to do costly operations for the `Log` call parameters.
  - If there are no `LogAction`s defined (see below) to respond to a specific category/level combination,
the `?.` operator will short-circuit and prevent the Log method from being called.

### Log Levels
There are five available logging lvels, which are the same as [Rust's](https://docs.rs/log/latest/log/#usage): `Error`, `Warning`, `Info`, `Debug` and `Trace`.

By default, `Error`, `Warning` and `Info` forward their logs to their Unity counterparts (`Debug.LogError`, `LogWarning` and `Log`).
Apart from that, the levels come with no implemented semantics, so a project may freely define their exact usage and implement logging setup accordingly (see below).

# Setup
## Creating Categories
Create one or more `LogCategory` instances in order to log through them.
```csharp
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public static class Log
{
    public static readonly LogCategory Combat = new LogCategory("Combat");
```

The `[InitializeOnLoad]` attribute makes sure that the log level window (`TeaSpoons/Logging/Log Levels`) works correctly.

## Custom Logging Behavior
To define custom logging behavior, run code like this during game startup:
```csharp
using TeaSpoons.Logging.Setup;
```

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
private static void InitializeLogging()
{
    // Preprocessor directives can be used to define different logging behavior per platform.
#if !UNITY_EDITOR
    // Specify a default LogAction that will be used regardless of LogCategory.
    // This will disable default Unity console logging.
    LoggingSetup.SetDefaultLogAction(LogLevel.Error, (in LogEntry entry) =>
    {
        // TODO Do something with ALL error log entries.
        
        // The default Behavior would be:
        Debug.LogError(entry);
    });
    
    // Remove the above default LogAction again.
    LoggingSetup.SetDefaultLogAction(LogLevel.Error, null);
#endif
    
    // You can add additional custom LogActions for a category/level combination.
    LoggingSetup.AddCustomLogAction(Log.Combat, LogLevel.Info, MyLogAction);
    LoggingSetup.RemoveCustomLogAction(Log.Combat, LogLevel.Info, MyLogAction);
}
```

Custom `LogAction`s are added to a list, so logs can be handled in multiple ways at once.

Every method in `LoggingSetup` can be called at any time during runtime, so changing logging behavior for debugging purposes is possible even in builds.

# Advanced Setup
## Setting Max Log Levels/Muting Categories
The `LoggingSetup` class allows you to change the max log level of a `LogCategory`:
```csharp
LoggingSetup.SetMaxLogLevel(Log.Combat, LogLevel.Info); // Error, Warning and Info will be logged
LoggingSetup.MuteLogCategory(Log.Combat); // Nothing will be logged
```

or globally:
```csharp
LoggingSetup.SetGlobalMaxLogLevel(LogLevel.Highest); // Everything will be logged
LoggingSetup.MuteLogging(Log.Combat); // Nothing will be logged
```

## Rerouting Unity Logs
Use the `UnityLogHandler` class to route Unity logs into a LogCategory:
```csharp
UnityLogHandler.CreateAndAssign(Log.System); // Reroute Unity logs into this LogCategory
UnityLogHandler.CreateAndAssign(null); // Reroute Unity logs into oblivion
UnityLogHandler.ResetToDefault(); // Restore default Unity logging behavior
```

# Advanced Logging
## Additional Data
You can implement the `ILogData` interface to add additional data to your logs, which can be parsed by your custom `LogAction` like a dictionary.

Here is an example implementation:
```csharp
public readonly struct MyLogData : ILogData
{
    private readonly string foo;
    
    public MyLogData(string foo)
    {
        this.foo = foo;
    }

    IEnumerator<(string Key, string Value)> IEnumerable<(string Key, string Value)>.GetEnumerator()
    {
        yield return ("foo", foo);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<(string Key, string Value)>)this).GetEnumerator();
    }
}
```

You can attach an `ILogData` to your log calls:
```csharp
Log.Combat.Info?.Log("Message", new MyLogData("bar"));
```

Your custom log action can iterate over the the data points accessed via `LogEntry.AdditionalData`:
```csharp
foreach (var item in entry.AdditionalData)
{
    DoSomethingWith(item.Key, item.Value);
}
```

## Flags
You can add semantic flags to your logs. Use POT integer constants to define them.

Declaration:
```csharp
public static class Log
{
    public static class Flag
    {
        public const int SkipFormatting = 1;
        public const int Foo = 2;
        public const int Bar = 4;
```

Usage:
```csharp
Log.Combat.Info?.Log("Message", Log.Flag.SkipFormatting | Log.Flag.Foo);
```

Interpretation within your `LogAction`:
```csharp
if (entry.HasFlag(Log.Flag.SkipFormatting))
```

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/logging.git
```

Pin a release by appending a tag, for example `#v1.4.0`.

### Dependencies

None. Logging works on its own and installs from the git URL without adding anything else.

It uses the optional package below when your project has it (Unity detects it automatically) and falls back to plain behaviour when it does not.

| Package | Used for |
|---|---|
| Package Core (`com.tea-spoons.package-core` 1.0.0+) | The shared `TeaSpoons/` menu root and the `GUIColor` helper of the **Log Levels** editor window. Without it the window looks and works the same, at `TeaSpoons/Logging/Log Levels`, using small built-in equivalents. |

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
