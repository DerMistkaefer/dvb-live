using System.Linq;

namespace vdo.trias
{
    /// <summary>
    /// Extensions for Trias Data Structure
    /// </summary>
    public static class TriasExtensions
    {
        /// <summary>
        /// Get the best possible display text of <see cref="InternationalTextStructure"/> data
        /// </summary>
        /// <param name="textStructures">possible options</param>
        /// <returns>best display text</returns>
        public static string GetBestText(this InternationalTextStructure[] textStructures)
            => textStructures?.FirstOrDefault(x => x.Language == "de")?.Text
               ?? textStructures?.FirstOrDefault()?.Text
               ?? "???";
    }
}