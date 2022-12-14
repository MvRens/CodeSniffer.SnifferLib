namespace CodeSniffer.SnifferLib.Matchers
{
    /// <summary>
    /// Creates an <see cref="IMatcher"/> from the specified pattern(s).
    /// <br/><br/>
    /// If the pattern starts and ends with a forward slash (/), the pattern is considered to be
    /// a regular expression. Otherwise, if a wildcard character is present (*, ?), it is treated as
    /// a pattern where an asterisk matches one or more characters and a question mark exactly one character.
    /// Finally, if neither are present it is considered to be an exact, case-insensitive, match.
    /// </summary>
    public class MatcherFactory
    {
        /// <inheritdoc cref="MatcherFactory"/>
        public static IMatcher Create(string pattern)
        {
            return RegexMatcher.CreateIfValid(pattern)
                   ?? WildcardMatcher.CreateIfValid(pattern)
                   ?? new ExactMatcher(pattern);
        }


        /// <inheritdoc cref="MatcherFactory"/>
        /// <remarks>
        /// The result of the <see cref="IMatcher.Matches"/> is true if any of the patterns match.
        /// </remarks>
        public static IMatcher Create(IEnumerable<string> patterns)
        {
            return new AggregateMatcher(patterns.Select(Create));
        }
    }
}