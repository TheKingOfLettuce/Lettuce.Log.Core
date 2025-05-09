using System.Runtime.CompilerServices;

namespace LettuceLogger.Core {
    public class Logger {
        private List<ILogDestination> _destinations = new();
        private List<ILogFormatter> _formatters = new();
        private readonly string _template;

        public Logger(string temaplte) {
            _template = temaplte;
        }

        public void AddDestination(ILogDestination destination) {
            _destinations.Add(destination);
        }

        public void AddFormatter(ILogFormatter formatter) {
            _formatters.Add(formatter);
        }

        public void Verbose(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.VERBOSE, message, callingFile, callingMethod, lineNumber);
        
        public void Debug(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.DEBUG, message, callingFile, callingMethod, lineNumber);
        
        public void Info(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.INFORMATION, message, callingFile, callingMethod, lineNumber);
        
        public void Warning(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.WARNING, message, callingFile, callingMethod, lineNumber);
        
        public void Error(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.ERROR, message, callingFile, callingMethod, lineNumber);
        
        public void Fatal(string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1)
            => LogMessage(LogEventLevel.FATAL, message, callingFile, callingMethod, lineNumber);

        public void LogMessage(LogEventLevel level, string message, [CallerFilePath]string callingFile = "", [CallerMemberName]string callingMethod = "", [CallerLineNumber]int lineNumber = -1) {
            string logMessage = FormatMessage(level, message, 
                Path.GetFileNameWithoutExtension(callingFile), callingMethod, lineNumber);

            foreach(ILogDestination destination in _destinations) {
                destination.LogMessage(logMessage);
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
    }
}