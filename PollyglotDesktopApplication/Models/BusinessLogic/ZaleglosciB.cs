using PollyglotDesktopApp.Models.ForAllView;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    public class ZaleglosciB : DatabaseClass
    {
        #region Konstruktor
        public ZaleglosciB(PollyglotDBEntities db)
            : base(db)
        {
        }
        #endregion

        #region Funkcje biznesowe

        // Raport zaległości dla okresu.
        // grupaId = null => wszyscy uczniowie (indywidualni + grupowi)
        public List<PlatnoscRaportRow> RaportZaleglosci(string okres, int? grupaId = null)
        {
            if (string.IsNullOrWhiteSpace(okres))
                return new List<PlatnoscRaportRow>();

            // 1) Uczniowie 
            var uczniowie = (
                from u in db.Uczen
                where grupaId == null || u.GrupaUczen.Any(gu => gu.GrupaId == grupaId.Value)
                select u
            ).ToList();

            // 2) Płatności w okresie
            var platnosciPerUczen = (
                from p in db.Platnosc
                where p.Okres == okres
                select p
            )
            .ToList()
            .GroupBy(p => p.UczenId)
            .ToDictionary(g => g.Key, g => g.ToList());

            // 3) Raport
            var raport = new List<PlatnoscRaportRow>();

            foreach (var uczen in uczniowie)
            {
                decimal expected = OczekiwanaKwotaMiesieczna(uczen.UczenId);
                decimal paid = SumaOplaconych(platnosciPerUczen, uczen.UczenId);
                var last = OstatniaPlatnosc(platnosciPerUczen, uczen.UczenId);

                decimal balance = expected - paid;

                raport.Add(new PlatnoscRaportRow
                {
                    Okres = okres,
                    UczenId = uczen.UczenId,
                    Imie = uczen.Imie,
                    Nazwisko = uczen.Nazwisko,
                    ExpectedAmount = expected,
                    PaidAmount = paid,
                    Balance = balance,
                    StatusRaportu = balance > 0 ? "ZALEGA" : "OPŁACONE",
                    Uwagi = last?.Uwagi
                });
            }

            return raport
                .OrderByDescending(r => r.Balance)
                .ToList();
        }

        // Oczekiwana kwota miesięczna:
        // - kursy indywidualne: UczenKurs -> Kurs.CenaMiesieczna
        // - kursy grupowe: GrupaUczen -> Grupa -> Kurs.CenaMiesieczna
        public decimal OczekiwanaKwotaMiesieczna(int uczenId)
        {
            var indywidualne = (
                from uk in db.UczenKurs
                where uk.UczenId == uczenId
                select new
                {
                    KursId = uk.KursId,
                    Cena = (decimal?)(uk.Kurs.CenaMiesieczna)
                }
            ).ToList();

            var grupowe = (
                from gu in db.GrupaUczen
                where gu.UczenId == uczenId
                select new
                {
                    KursId = gu.Grupa.KursId,
                    Cena = (decimal?)(gu.Grupa.Kurs.CenaMiesieczna)
                }
            ).ToList();

            var wszystkie = indywidualne
                .Concat(grupowe)
                .GroupBy(x => x.KursId)
                .Select(g => g.FirstOrDefault())
                .ToList();

            return wszystkie.Sum(x => x?.Cena ?? 0m);
        }

        #endregion

        #region Helpers

        private decimal SumaOplaconych(Dictionary<int, List<Platnosc>> platnosciPerUczen, int uczenId)
        {
            if (!platnosciPerUczen.TryGetValue(uczenId, out var lista))
                return 0m;

            return lista
                .Where(p => CzyOplacona(p.Status))
                .Sum(p => p.Kwota ?? 0m);
        }

        private Platnosc OstatniaPlatnosc(Dictionary<int, List<Platnosc>> platnosciPerUczen, int uczenId)
        {
            if (!platnosciPerUczen.TryGetValue(uczenId, out var lista))
                return null;

            return lista
                .OrderByDescending(p => p.DataPlatnosci ?? DateTime.MinValue)
                .FirstOrDefault();
        }

        private bool CzyOplacona(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            var s = status.Trim().ToLowerInvariant();

            return s.Contains("opłac") || s.Contains("oplac")
                || s.Contains("zapłac") || s.Contains("zaplac");
        }

        #endregion
    }
}
