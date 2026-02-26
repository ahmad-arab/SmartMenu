namespace SmartMenu
{
    /// <summary>
    /// Shared fallback strings used when a localized text entry is missing.
    /// </summary>
    public static class FallbackText
    {
        public const string NoText = "(No text)";
        public const string NoTitle = "(No title)";

        /// <summary>
        /// Maximum number of characters shown in a summarized preview before truncating.
        /// </summary>
        public const int SummaryMaxLength = 100;

        /// <summary>
        /// Returns a truncated version of <paramref name="text"/> if it exceeds
        /// <see cref="SummaryMaxLength"/>, appending "…" when truncated.
        /// </summary>
        public static string Summarize(string text) =>
            text.Length > SummaryMaxLength ? text[..SummaryMaxLength] + "…" : text;
    }
}
