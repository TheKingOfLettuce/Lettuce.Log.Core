using System;

namespace Lettuce.Log.Core {
    /// <summary>
    /// Logger class that handles writing logs to a list of <see cref="ILogDestination"/> <br/>
    /// that are formatted by a list of <see cref="ILogFormatter"/>
    /// </summary>
    public sealed class Logger : IDisposable {
        /// <summary>
        /// The current <see cref="LogEventLevel"/> this log instance is set to
        /// </summary>
        public LogEventLevel CurrentLevel => _currentLevel;

        // copy-on-write arrays: writers swap in a new array under _writeLock,
        // readers (logging) grab the current array without locking and never see it change
        private volatile ILogDestination[] _destinations = Array.Empty<ILogDestination>();
        private volatile ILogFormatter[] _formatters = Array.Empty<ILogFormatter>();
        private readonly object _writeLock = new object();
        private readonly string _template;
        private volatile LogEventLevel _currentLevel;

        private volatile bool _disposed;

        /// <summary>
        /// Takes in a template for how log messages should be formatted
        /// </summary>
        /// <param name="template">the template to use for log formats</param>
        /// <param name="startingLevel">the starting <see cref="LogEventLevel"/> of this logger, defaults to <see cref="LogEventLevel.INFORMATION"/></param>
        public Logger(string template, LogEventLevel startingLevel = LogEventLevel.INFORMATION) {
            _template = template;
            _currentLevel = startingLevel;
        }

        /// <summary>
        /// Adds a <see cref="ILogDestination"/> to the list of places the log message goes
        /// </summary>
        /// <param name="destination">the place for logs to go</param>
        public void AddDestination(ILogDestination destination) {
            lock (_writeLock) {
                if (_disposed)
                    throw new ObjectDisposedException("Logger has been disposed");
                _destinations = Append(_destinations, destination);
            }
        }

        /// <summary>
        /// Adds a <see cref="ILogFormatter"/> to the list of formats
        /// </summary>
        /// <param name="formatter">the log formatter to add</param>
        public void AddFormatter(ILogFormatter formatter) {
            lock (_writeLock) {
                if (_disposed)
                    throw new ObjectDisposedException("Logger has been disposed");
                _formatters = Append(_formatters, formatter);
            }
        }

        private static T[] Append<T>(T[] source, T item) {
            T[] result = new T[source.Length + 1];
            Array.Copy(source, result, source.Length);
            result[source.Length] = item;
            return result;
        }

        /// <summary>
        /// Changes the <see cref="LogEventLevel"/> to the provided <paramref name="level"/>
        /// </summary>
        /// <param name="level">the new <see cref="LogEventLevel"/> to set</param>
        public void ChangeLevel(LogEventLevel level) => _currentLevel = level;

        /// <summary>
        /// Logs a <see cref="LogEventLevel.VERBOSE"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Verbose(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.VERBOSE, message, dynamicFormats);
        
        /// <summary>
        /// Logs a <see cref="LogEventLevel.DEBUG"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Debug(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.DEBUG, message, dynamicFormats);
        
        /// <summary>
        /// Logs a <see cref="LogEventLevel.INFORMATION"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Info(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.INFORMATION, message, dynamicFormats);
        
        /// <summary>
        /// Logs a <see cref="LogEventLevel.WARNING"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Warning(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.WARNING, message, dynamicFormats);
        
        /// <summary>
        /// Logs a <see cref="LogEventLevel.ERROR"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Error(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.ERROR, message, dynamicFormats);
        
        /// <summary>
        /// Logs a <see cref="LogEventLevel.FATAL"/> message with context info
        /// </summary>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void Fatal(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.FATAL, message, dynamicFormats);

        /// <summary>
        /// Logs a message with context info and a given <see cref="LogEventLevel"/>
        /// </summary>
        /// <param name="level">the logging level</param>
        /// <param name="message">the message to log</param>
        /// <param name="dynamicFormats">(optional) the list of <see cref="ILogFormatter"/> to use for log message formatting that are not a part of the logger</param>
        public void LogMessage(LogEventLevel level, string message, ILogFormatter[]? dynamicFormats = null) {
            if (_disposed)
                throw new ObjectDisposedException("Logger has been disposed");

            if (_currentLevel > level) {
                return;
            }

            string logMessage = FormatMessage(level, message, dynamicFormats);

            foreach(ILogDestination destination in _destinations) {
                destination.LogMessage(logMessage, level);
            }
        }

        private string FormatMessage(LogEventLevel level, string message, ILogFormatter[]? dynamicFormats = null) {
            const string MESSAGE_KEY = "{Message}";
            const string LEVEL_KEY = "{Level}";
            
            string toReturn = _template;

            toReturn = toReturn.Replace(MESSAGE_KEY, message);
            toReturn = toReturn.Replace(LEVEL_KEY, level.ToString().PadLeft(11));
            foreach (ILogFormatter formatter in _formatters) {
                toReturn = toReturn.Replace(formatter.FormatKey, formatter.GetFormat());
            }
            if (dynamicFormats != null) {
                foreach(ILogFormatter formatter in dynamicFormats) {
                    toReturn = toReturn.Replace(formatter.FormatKey, formatter.GetFormat());
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Disposes of the logger by disposing of any <see cref="ILogDestination"/>
        /// </summary>
        public void Dispose() {
            ILogDestination[] destinations;
            lock (_writeLock) {
                if (_disposed) {
                    return;
                }

                _disposed = true;
                destinations = _destinations;
            }

            foreach (ILogDestination destination in destinations) {
                if (destination is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }
    }
}