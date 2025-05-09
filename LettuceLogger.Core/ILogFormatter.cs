namespace LettuceLogger.Core;

public interface ILogFormatter {
    string FormatKey {get;}
    string GetFormat();
}