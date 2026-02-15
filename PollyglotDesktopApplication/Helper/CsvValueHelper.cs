using System.Globalization;

namespace PollyglotDesktopApp.Helper
{
    public static class CsvValueHelper
    {
        private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

        public static string FormatDecimal(decimal value)
        {
            return value.ToString("N2", PolishCulture);
        }

        public static string Sanitize(string value)
        {
            return value?.Replace(";", ",") ?? string.Empty;
        }
    }
}
