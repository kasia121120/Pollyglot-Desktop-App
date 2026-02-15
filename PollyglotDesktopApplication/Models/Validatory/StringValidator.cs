using System;

namespace PollyglotDesktopApp.Models.Validatory
{
    public class StringValidator : Validator
    {
        public static string SprawdzCzyZaczynaSieOdDuzej(string wartosc)
        {
            var trimmed = wartosc?.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return null;
            }

            return char.IsUpper(trimmed[0])
                ? null
                : "Rozpocznij dużą literą.";
        }

        public static string SprawdzMinimalnaDlugosc(string wartosc, int minDlugosc, string message)
        {
            var trimmed = wartosc?.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.Length < minDlugosc)
            {
                return message;
            }

            return null;
        }
    }
}
