namespace BinanceImporter.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveNonNumeric(this string input)
        {
            string output = string.Empty;

            foreach (char c in input)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    output += c;
                }
            }

            return output;
        }
    }

}