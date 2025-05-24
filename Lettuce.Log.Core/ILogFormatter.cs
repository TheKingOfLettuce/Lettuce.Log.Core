namespace Lettuce.Log.Core {
    /// <summary>
    /// Formatter which formats a template by searching for the <see cref="FormatKey"/> and replaces it with <see cref="GetFormat"/>
    /// </summary>
    public interface ILogFormatter {
        /// <summary>
        /// The key to look for in the template
        /// </summary>
        string FormatKey {get;}

        /// <summary>
        /// The method that gets called to replace <see cref="FormatKey"/> with in the log template
        /// </summary>
        /// <returns></returns>
        string GetFormat();
    }
}