using PollyglotDesktopApp.Models.ForAllView;
using System.Collections.Generic;
using System.Linq;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    public class LektorB : DatabaseClass
    {
        #region Konstruktor
        public LektorB(PollyglotDBEntities db) : base(db)
        {
        }
        #endregion

        #region Funkcje pomocnicze (ComboBox)
        public IQueryable<KeyAndValue<int>> GetLektorzyKeyAndValueItems()
        {
            //select z bazy danych
            var dane = db.Lektor
                .OrderBy(l => l.Nazwisko)
                .ThenBy(l => l.Imie)
                .Select(l => new
                {
                    l.LektorId,
                    l.Imie,
                    l.Nazwisko
                })
                .ToList();

            // formatowanie nazwy
            var wynik = dane
                .Select(l => new KeyAndValue<int>
                {
                    Key = l.LektorId,
                    Value = ((l.Imie ?? "") + " " + (l.Nazwisko ?? "")).Trim()
                })
                .ToList();

            return wynik.AsQueryable();
        }
        #endregion
    }
}
