using System.Runtime.CompilerServices;

namespace Lettuce.Log.Core;

/// <summary>
/// Static wrapped <see cref="Logger"/> to allow for easy global logging
/// </summary>
public static class Log {
    /// <summary>
    /// Default template used for the lazy initialized logger
    /// </summary>
    public const string DEFAULT_TEMPLATE = "{Level} ({File}/{Method} at {Line}) {Message}";

    /// <summary>
    /// The wrapped <see cref="Logger"/>
    /// </summary>
    public static Logger Logger => _wrappedLogger;

    private static Logger _wrappedLogger;

    static Log() {
        _wrappedLogger = new Logger(DEFAULT_TEMPLATE);
    }

    /// <summary>
    /// Sets the statically wrapped logger
    /// </summary>
    /// <param name="logger">the new logger to wrap</param>
    /// <exception cref="ArgumentException">throws if the provided logger is null</exception>
    public static void ReassignLogger(Logger logger) {
        if (logger == null) {
            throw new ArgumentException("Cannot assign null logger to static wrapped logger", nameof(logger));
        }

        _wrappedLogger.Dispose();
        _wrappedLogger = logger;
    }

    /// <inheritdoc cref="Logger.Verbose"/>
    public static void Verbose(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.VERBOSE, message, methodName, filePath, lineNumber);
    
    /// <inheritdoc cref="Logger.Debug"/>
    public static void Debug(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.DEBUG, message, methodName, filePath, lineNumber);
    
    /// <inheritdoc cref="Logger.Info"/>
    public static void Info(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.INFORMATION, message, methodName, filePath, lineNumber);
    
    /// <inheritdoc cref="Logger.Warning"/>
    public static void Warning(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.WARNING, message, methodName, filePath, lineNumber);
    
    /// <inheritdoc cref="Logger.Error"/>
    public static void Error(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.ERROR, message, methodName, filePath, lineNumber);
    
    /// <inheritdoc cref="Logger.Fatal"/>
    public static void Fatal(string message, 
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.FATAL, message, methodName, filePath, lineNumber);


    /// <inheritdoc cref="Logger.LogMessage"/>
    public static void LogMessage(LogEventLevel logLevel, string message,
        [CallerMemberName]string methodName = "", 
        [CallerFilePath]string filePath = "",
        [CallerLineNumber]int lineNumber = -1)
    {
        Logger.LogMessage(logLevel, message, filePath, methodName, lineNumber);
    }
}