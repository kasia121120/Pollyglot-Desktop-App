using System;

namespace PollyglotDesktopApp.Models.Validatory
{
    public static class DateValidator
    {
        public static string SprawdzCzyDataZakonczeniaNieJestPrzedDolaczeniem(DateTime dolaczenia, DateTime? zakonczenia)
        {
            if (zakonczenia.HasValue && zakonczenia.Value.Date < dolaczenia.Date)
            {
                return "Data zakończenia nie może być wcześniejsza niż data dołączenia.";
            }

            return null;
        }
    }
}
