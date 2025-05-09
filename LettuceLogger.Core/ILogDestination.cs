namespace LettuceLogger.Core;

/// <summary>
/// Simple interface to represent a place where a log message can go
/// </summary>
public interface ILogDestination {
    /// <summary>
    /// the method to call to log a message to the destination
    /// </summary>
    /// <param name="message"></param>
    void LogMessage(string message);
}