using System.Runtime.CompilerServices;

namespace Lettuce.Log.Core;

/// <summary>
/// Logger class that handles writing logs to a list of <see cref="ILogDestination"/> <br/>
/// that are formatted by a list of <see cref="ILogFormatter"/>
/// </summary>
public sealed class Logger : IDisposable {
    private List<ILogDestination> _destinations = new();
    private List<ILogFormatter> _formatters = new();
    private readonly string _template;

    private bool _disposed;

    /// <summary>
    /// Takes in a template for how log messages should be formatted
    /// </summary>
    /// <param name="temaplte">the template to use for log formats</param>
    public Logger(string temaplte) {
        _template = temaplte;
    }

    /// <summary>
    /// Adds a <see cref="ILogDestination"/> to the list of places the log message goes
    /// </summary>
    /// <param name="destination">the place for logs to go</param>
    public void AddDestination(ILogDestination destination) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _destinations.Add(destination);
    }

    /// <summary>
    /// Adds a <see cref="ILogFormatter"/> to the list of formats
    /// </summary>
    /// <param name="formatter">the log formatter to add</param>
    public void AddFormatter(ILogFormatter formatter) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _formatters.Add(formatter);
    }

    /// <summary>
    /// Logs a <see cref="LogEventLevel.VERBOSE"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Verbose(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.VERBOSE, message, callingFile, callingMethod, lineNumber);
    
    /// <summary>
    /// Logs a <see cref="LogEventLevel.DEBUG"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Debug(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.DEBUG, message, callingFile, callingMethod, lineNumber);
    
    /// <summary>
    /// Logs a <see cref="LogEventLevel.INFORMATION"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Info(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.INFORMATION, message, callingFile, callingMethod, lineNumber);
    
    /// <summary>
    /// Logs a <see cref="LogEventLevel.WARNING"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Warning(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.WARNING, message, callingFile, callingMethod, lineNumber);
    
    /// <summary>
    /// Logs a <see cref="LogEventLevel.ERROR"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Error(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.ERROR, message, callingFile, callingMethod, lineNumber);
    
    /// <summary>
    /// Logs a <see cref="LogEventLevel.FATAL"/> message with context info
    /// </summary>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void Fatal(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
        => LogMessage(LogEventLevel.FATAL, message, callingFile, callingMethod, lineNumber);

    /// <summary>
    /// Logs a message with context info and a given <see cref="LogEventLevel"/>
    /// </summary>
    /// <param name="level">the logging level</param>
    /// <param name="message">the message to log</param>
    /// <param name="callingFile">the calling file via <see cref="CallerFilePathAttribute"/></param>
    /// <param name="callingMethod">the calling method via <see cref="CallerMemberNameAttribute"/></param>
    /// <param name="lineNumber">the calling line number via <see cref="CallerLineNumberAttribute"/></param>
    public void LogMessage(LogEventLevel level, string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        string logMessage = FormatMessage(level, message, 
            Path.GetFileNameWithoutExtension(callingFile), callingMethod, lineNumber);

        foreach(ILogDestination destination in _destinations) {
            destination.LogMessage(logMessage, level);
        }
    }

    private string FormatMessage(LogEventLevel level, string message, string file, string method, int line) {
        const string MESSAGE_KEY = "{Message}";
        const string LEVEL_KEY = "{Level}";
        const string FILE_KEY = "{File}";
        const string METHOD_KEY = "{Method}";
        const string LINE_KEY = "{Line}";
        
        string toReturn = _template;

        toReturn = toReturn.Replace(MESSAGE_KEY, message);
        toReturn = toReturn.Replace(LEVEL_KEY, level.ToString().PadLeft(11));
        toReturn = toReturn.Replace(FILE_KEY, file);
        toReturn = toReturn.Replace(METHOD_KEY, method);
        toReturn = toReturn.Replace(LINE_KEY, line.ToString());
        foreach (ILogFormatter formatter in _formatters) {
            toReturn = toReturn.Replace(formatter.FormatKey, formatter.GetFormat());
        }

        return toReturn;
    }

    /// <summary>
    /// Disposes of the logger by disposing of any <see cref="ILogDestination"/>
    /// </summary>
    public void Dispose() {
        if (_disposed) {
            return;
        }

        foreach (ILogDestination destination in _destinations) {
            if (destination is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }
}