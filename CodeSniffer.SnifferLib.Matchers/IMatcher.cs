namespace CodeSniffer.SnifferLib.Matchers
{
    /// <summary>
    /// Matches a value against a predetermined pattern.
    /// </summary>
    public interface IMatcher
    {
        /// <inheritdoc cref="IMatcher"/>
        bool Matches(string value);
    }
}
