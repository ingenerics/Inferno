## Logging

#### Introduction

Where I got the milk from: [Logger wrapper best practice](https://stackoverflow.com/questions/5646820/logger-wrapper-best-practice).



#### ILogger

The core of the logging infrastructure resides at the `Inferno.Core.Logging` namespace. Including a `DebugLogger` and a slew of extension methods on `ILogger`, for example:

```c#
public static void LogInformation(this ILogger logger, object classifier, string message)
{
    logger.Log(new LogEntry(LoggingEventType.Information, message, classifier.GetType().Name));
}
```
Feel free to visit the namespace, or IntelliSense a logger (`using Inferno.Core.Logging;`), it's all pretty self-explanatory.



#### Reactive

`Inferno.Reactive` builds upon the core abstractions. All extensions are implemented in the root `Inferno` namespace, allowing them to be picked up more easily.

There's a `LoggerObserver`, and a `SubscribeLogger` extension method that makes use of it, as well as a couple of `Log` extension methods that can be used from anywhere inside the observable pipeline.

Ref `ScoreBoard` sample.

```c#
ActiveItem.CloseCommand
    .Log("Requested close application")
    .SelectMany(_ => AskConfirmation())
    .SubscribeLogger("Confirmed close application")
    .DisposeWith(disposables);
```
When the user cancels the close, `SubscribeLogger` will add the following output.

> [Information] Confirmed close application: OnNext(False)

There are, of course, overloads to pass the `LoggingEventType`. Though, keep in mind Steven van Deursen's answer from the post in the introduction. The `ILogger` functional interface (with a Single Abstract Method) keeps everything open for extension.

The `WorldCup` sample implements its own `ItemsControlLogger`, which is decorated with:

```c#
public interface IItemsControlLogger : ILogger
{
    IList<string> ItemsSource { get; }
}
```
Just be sure to register both `IItemsControlLogger` and `ILogger` with the `DependencyResolver` as the framework depends on the latter. I registered `IItemsControlLogger` in order to resolve `ShellViewModel`, which takes it as a constructor argument. This is without a doubt (ctor arg) the cleanest (read: academic) way to pass on your logger.

However, to avoid the situation where users of the framework would expose public statics and/or singletons to be able to log from the outer realms of the application, the approach of ReactiveUI is followed. Any class decorated with the `IEnableLogger` marker interface, will expose similar extension methods as those found on the `ILogger` interface. 

```c#
public static void LogInformation(this IEnableLogger _, object classifier, string message)
    => _logger.LogInformation(classifier, message);
```



#### Framework

Component lifecycle events are logged (type `Information`). The `WorldCup` sample highlights this by showing these in the *Logs* `ItemsControl` at the bottom.



#### Next

[Final thoughts](../MainSellingPoints/MainSellingPoints.md)

