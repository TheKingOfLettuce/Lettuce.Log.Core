namespace Lettuce.Log.Core;

/// <summary>
/// Simple interface to represent a place where a log message can go
/// </summary>
public interface ILogDestination {
    /// <summary>
    /// the method to call to log a message to the destination
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="level">the logging level of the message</param>
    void LogMessage(string message, LogEventLevel level);
}