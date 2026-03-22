using System.Runtime.CompilerServices;
using System;

namespace Lettuce.Log.Core {
    /// <summary>
    /// Static wrapped <see cref="Logger"/> to allow for easy global logging
    /// </summary>
    public static class Log {
        /// <summary>
        /// Default template used for the lazy initialized logger
        /// </summary>
        public const string DEFAULT_TEMPLATE = "{Level} {Message}";

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
        public static void Verbose(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.VERBOSE, message, dynamicFormats);
        
        /// <inheritdoc cref="Logger.Debug"/>
        public static void Debug(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.DEBUG, message, dynamicFormats);
        
        /// <inheritdoc cref="Logger.Info"/>
        public static void Info(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.INFORMATION, message, dynamicFormats);
        
        /// <inheritdoc cref="Logger.Warning"/>
        public static void Warning(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.WARNING, message, dynamicFormats);
        
        /// <inheritdoc cref="Logger.Error"/>
        public static void Error(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.ERROR, message, dynamicFormats);
        
        /// <inheritdoc cref="Logger.Fatal"/>
        public static void Fatal(string message, ILogFormatter[]? dynamicFormats = null)
            => LogMessage(LogEventLevel.FATAL, message, dynamicFormats);


        /// <inheritdoc cref="Logger.LogMessage"/>
        public static void LogMessage(LogEventLevel logLevel, string message, ILogFormatter[]? dynamicFormats = null)
        {
            Logger.LogMessage(logLevel, message, dynamicFormats);
        }
    }
}