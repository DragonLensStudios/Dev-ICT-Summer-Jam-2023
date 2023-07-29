namespace DLS.Utilities.Extensions.StringExtensions
{
    public static class RemoveQuotesExtension
    {
        public static string RemoveQuotations(this string input)
        {
            return input.Replace("\"", string.Empty);
        }
    }
}

