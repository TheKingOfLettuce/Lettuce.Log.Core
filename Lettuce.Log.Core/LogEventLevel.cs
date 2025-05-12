namespace Lettuce.Log.Core;

/// <summary>
/// Enum for log levels
/// </summary>
public enum LogEventLevel {

    /// <summary>
    /// Used for logs that need exact trace information
    /// </summary>
    VERBOSE,

    /// <summary>
    /// Used for the logs that describe low level behavior
    /// </summary>
    DEBUG,

    /// <summary>
    /// Used for logs that describe high level behavior
    /// </summary>
    INFORMATION,

    /// <summary>
    /// Used for logs that encounted an issue that were able to recover/move on from
    /// </summary>
    WARNING,

    /// <summary>
    /// Used for logs that encounted an issue that requires interrutiping behavior
    /// </summary>
    ERROR,

    /// <summary>
    /// Used for logs that are dentreimental to the application as a whole
    /// </summary>
    FATAL
}