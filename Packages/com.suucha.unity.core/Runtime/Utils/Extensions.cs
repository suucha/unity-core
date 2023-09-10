using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Utils
{
    public static class Extensions
    {

        /// <summary>Matches the underscore and return the first or default.</summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static string MatchUnderscoreFirstOrDefault(this IEnumerable<string> source, string target)
        {
            var matched = source.FirstOrDefault(s =>
                s.EndsWith("_") && (target.StartsWith(s) ||
                target == s.Substring(0, s.Length - 1)) ||
                (!s.EndsWith("_") && target == s));
            return matched;
        }
        /// <summary>Matches the underscore.</summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static bool MatchUnderscore(this IEnumerable<string> source, string target)
        {
            var matched = MatchUnderscoreFirstOrDefault(source, target);
            return !string.IsNullOrEmpty(matched);
        }
    }
}
