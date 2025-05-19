# Lettuce.Log.Core

The core of the logging system to allow for ease of formatting logs and sending them to various destinations

[![Build](https://github.com/TheKingOfLettuce/Lettuce.Log.Core/actions/workflows/build.yml/badge.svg)](https://github.com/TheKingOfLettuce/Lettuce.Log.Core/actions/workflows/build.yml)


## Quick Start

`Lettuce.Log.Core` is avaliable on [nuget](https://www.nuget.org/packages/Lettuce.Log.Core/)

```csharp
Logger logger = new Logger(Log.DEFAULT_TEMPLATE);
// logger.AddDestination(...);
// logger.AddFormatter(...);

logger.Info("This is an information message");
logger.Error("This is an error message");
```


### Logger

`Logger` objects are where you log messages to certain `LogEventLevel`. Every log message is context aware, making using of Microsoft's Caller Attributes, where the Calling file, method and line number are all passed with each log call.

`Logger` objects are also the collection `ILogFormatter` and `ILogDestination` objects.

There is also a statically wrapped `Logger` called `Log` to allow for global logging around your systems without having to pass a `Logger` instance around


### ILogFormatter

These objects format the log messages that come in. Each logger when constructing takes in a template for how the log message should be formatted before being fully logged. These templates are essentially keys to look in a string to insert the formatted string you want.

By default, `Logger` has the following formatter that get run for each log:
`{Message}` | The log message
`{Level}`   | The logging level of the message
`{File}`    | The calling file the log originated from
`{Method}`  | The calling method the log originated from
`{Line}`    | The line number the log originated from


### ILogDestination

These object take formatted log messages and logs them somewhere. How they get logged depends on the implementation, check out these commonly used desinations here:

- [Lettuce.Log.Console](https://github.com/TheKingOfLettuce/Lettuce.Log.Console)
- [Lettuce.Log.File](https://github.com/TheKingOfLettuce/Lettuce.Log.File)