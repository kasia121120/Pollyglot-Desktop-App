using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollyglotDesktopApp.Models.Validatory
{
    public class ZajeciaValidator : Validator
    {
        private static readonly HashSet<string> AllowedDays = new HashSet<string>
        {
            "poniedzialek",
            "wtorek",
            "sroda",
            "czwartek",
            "piatek"
        };

        private static readonly Dictionary<char, char> DiacriticMap = new Dictionary<char, char>
        {
            {'ą', 'a'},
            {'ć', 'c'},
            {'ę', 'e'},
            {'ł', 'l'},
            {'ń', 'n'},
            {'ó', 'o'},
            {'ś', 's'},
            {'ż', 'z'},
            {'ź', 'z'}
        };

        public static string ValidateGroupOrStudent(int? grupaId, int? uczenId)
        {
            var hasGroup = grupaId.HasValue;
            var hasStudent = uczenId.HasValue;
            if (hasGroup == hasStudent)
            {
                return "Wybierz dokładnie jedną opcję: grupę albo ucznia.";
            }

            return null;
        }

        public static string ValidateRodzajZajec(string rodzajZajec, int? grupaId, int? uczenId)
        {
            var normalized = NormalizeText(rodzajZajec);
            if (string.IsNullOrEmpty(normalized))
            {
                return "Rodzaj zajęć jest wymagany.";
            }

            if (normalized != "grupowe" && normalized != "indywidualne")
            {
                return "Rodzaj zajęć musi być 'grupowe' lub 'indywidualne'.";
            }

            if (grupaId.HasValue && normalized != "grupowe")
            {
                return "Dla grupy wybierz rodzaj 'grupowe'.";
            }

            if (uczenId.HasValue && normalized != "indywidualne")
            {
                return "Dla ucznia wybierz rodzaj 'indywidualne'.";
            }

            return null;
        }

        public static string ValidateData(DateTime? data)
        {
            if (!data.HasValue)
            {
                return "Data zajęć jest wymagana.";
            }

            return null;
        }

        public static string ValidateDzienTygodnia(string dzienTygodnia)
        {
            var normalized = NormalizeDayName(dzienTygodnia);
            if (string.IsNullOrEmpty(normalized))
            {
                return "Dzień tygodnia jest wymagany.";
            }

            if (!AllowedDays.Contains(normalized))
            {
                return "Dzień tygodnia musi być poniedziałek, wtorek, środa, czwartek lub piątek.";
            }

            return null;
        }

        public static string ValidateTryb(string tryb)
        {
            var normalized = NormalizeText(tryb);
            if (string.IsNullOrEmpty(normalized))
            {
                return "Tryb zajęć jest wymagany.";
            }

            if (normalized != "online" && normalized != "stacjonarne")
            {
                return "Tryb musi być 'online' lub 'stacjonarne'.";
            }

            return null;
        }

        public static string ValidateSala(int? salaId, string tryb)
        {
            if (!salaId.HasValue)
            {
                return "Sala jest wymagana.";
            }

            var normalizedTryb = NormalizeText(tryb);
            if (normalizedTryb == "online" && salaId.Value != 11)
            {
                return "Dla trybu online wybierz salę o numerze 11.";
            }

            if (normalizedTryb == "stacjonarne" && salaId.Value == 11)
            {
                return "Dla trybu stacjonarnego wybierz salę inną niż 11.";
            }

            return null;
        }

        public static string ValidateLektor(int? lektorId)
        {
            return lektorId.HasValue ? null : "Lektor jest wymagany.";
        }

        public static string ValidateTemat(string temat)
        {
            if (string.IsNullOrWhiteSpace(temat))
            {
                return "Temat jest wymagany.";
            }

            var trimmed = temat.Trim();
            var lengthError = StringValidator.SprawdzMinimalnaDlugosc(trimmed, 3, "Temat musi mieć przynajmniej 3 znaki.");
            if (!string.IsNullOrEmpty(lengthError))
            {
                return lengthError;
            }

            return StringValidator.SprawdzCzyZaczynaSieOdDuzej(trimmed);
        }

        public static string ValidateUwagi(string uwagi)
        {
            if (!string.IsNullOrEmpty(uwagi) && uwagi.Trim().Length > 300)
            {
                return "Uwagi nie mogą przekraczać 300 znaków.";
            }

            return null;
        }

        private static string NormalizeText(string value)
        {
            var trimmed = value?.Trim().ToLowerInvariant();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        private static string NormalizeDayName(string value)
        {
            var normalized = NormalizeText(value);
            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var ch in normalized)
            {
                builder.Append(DiacriticMap.TryGetValue(ch, out var replacement) ? replacement : ch);
            }

            return builder.ToString();
        }
    }
}
