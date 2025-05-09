namespace LettuceLogger.Core;

public interface ILogDestination {
    void LogMessage(string message);
}