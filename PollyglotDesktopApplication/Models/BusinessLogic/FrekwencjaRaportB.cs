using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    public class FrekwencjaRaportB : DatabaseClass
    {
        private const string PresentValue = "obecny";
        private const string AbsentValue = "nieobecny";

        public FrekwencjaRaportB(PollyglotDBEntities context)
            : base(context)
        {
        }

        public List<string> GetOkresy()
        {
            return db.Zajecia
                .Where(z => z.Data.HasValue)
                .Select(z => z.Data.Value)
                .ToList()
                .Select(d => d.ToString("yyyy-MM"))
                .Distinct()
                .OrderByDescending(o => o)
                .ToList();
        }

        public List<FrekwencjaRaportRow> GetRaport(string okres, int? grupaId = null)
        {
            if (!TryParseOkres(okres, out var year, out var month))
            {
                return new List<FrekwencjaRaportRow>();
            }

            var entries = db.Obecnosc
                .Include(o => o.Zajecia)
                .Include(o => o.Zajecia.Grupa)
                .Include(o => o.Uczen)
                .Where(o => o.Zajecia.Data.HasValue &&
                            o.Zajecia.Data.Value.Year == year &&
                            o.Zajecia.Data.Value.Month == month);

            if (grupaId.HasValue)
            {
                entries = entries.Where(o => o.Zajecia.GrupaId == grupaId.Value);
            }

            var attendanceList = entries.ToList();

            return attendanceList
                .GroupBy(o => o.UczenId)
                .Select(group =>
                {
                    var scheduled = group.Count();
                    var present = group.Count(o => IsPresentStatus(o.Status));
                    var first = group.FirstOrDefault();
                    var groupName = first?.Zajecia?.Grupa?.Nazwa ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(groupName))
                    {
                        groupName = "brak";
                    }
                    return new FrekwencjaRaportRow
                    {
                        UczenId = first?.UczenId ?? group.Key,
                        Imie = first?.Uczen?.Imie,
                        Nazwisko = first?.Uczen?.Nazwisko,
                        Grupa = groupName,
                        ZaplanowaneZajecia = scheduled,
                        Obecnosci = present,
                        Nieobecnosci = scheduled - present,
                        FrekwencjaProcent = scheduled == 0
                            ? 0m
                            : Math.Round((decimal)present / scheduled * 100m, 2),
                        Okres = okres
                    };
                })
                .OrderBy(r => r.FrekwencjaProcent)
                .ToList();
        }

        private static bool TryParseOkres(string okres, out int year, out int month)
        {
            year = 0;
            month = 0;

            if (string.IsNullOrWhiteSpace(okres))
                return false;

            var parts = okres.Split('-');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out year) || !int.TryParse(parts[1], out month))
                return false;

            return month >= 1 && month <= 12;
        }


        private static bool IsPresentStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            var normalized = status.Trim().ToLowerInvariant();
            if (normalized == AbsentValue)
                return false;
            return normalized == PresentValue;
        }
    }
}
