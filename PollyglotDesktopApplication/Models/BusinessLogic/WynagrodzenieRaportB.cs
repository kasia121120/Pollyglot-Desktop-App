using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    public class WynagrodzenieRaportB : DatabaseClass
    {
        public WynagrodzenieRaportB(PollyglotDBEntities context)
            : base(context)
        {
        }

        public List<string> GetOkresy()
        {
            var okresyZajec = db.Zajecia
                .Where(z => z.Data.HasValue)
                .Select(z => z.Data.Value)
                .ToList()
                .Select(d => d.ToString("yyyy-MM"))
                .Distinct()
                .OrderByDescending(o => o)
                .ToList();

            if (!okresyZajec.Any())
            {
                return new List<string>();
            }

            return okresyZajec;
        }

        public List<WynagrodzenieRaportRow> GetRaport(string okres)
        {
            if (string.IsNullOrWhiteSpace(okres))
            {
                return new List<WynagrodzenieRaportRow>();
            }

            var wynagrodzenia = db.Wynagrodzenie
                .Where(w => w.Okres == okres)
                .OrderBy(w => w.LektorId)
                .ToList();

            return wynagrodzenia
                .GroupBy(w => w.LektorId)
                .Select(group => new WynagrodzenieRaportRow
                {
                    LektorId = group.Key,
                    Imie = group.FirstOrDefault()?.Lektor?.Imie,
                    Nazwisko = group.FirstOrDefault()?.Lektor?.Nazwisko,
                    Okres = okres,
                    LiczbaGodzin = group.Sum(w => w.LiczbaGodzin) ?? 0m,
                    StawkaGodzinowa = group.Average(w => w.StawkaGodzinowa) ?? 0m,
                    KwotaDoWyplaty = group.Sum(w => w.KwotaDoWyplaty) ?? 0m,
                    Status = group.FirstOrDefault()?.Status
                })
                .ToList();
        }

        public List<Wynagrodzenie> GetWynagrodzeniaLektoraZaRok(int lektorId, int rok)
        {
            if (lektorId <= 0 || rok <= 0)
            {
                return new List<Wynagrodzenie>();
            }

            var prefix = rok.ToString("D4") + "-";

            var wynagrodzenia = db.Wynagrodzenie
                .Where(w => w.LektorId == lektorId
                            && w.Okres != null
                            && w.Okres.StartsWith(prefix))
                .OrderBy(w => w.Okres)
                .ToList();

            return wynagrodzenia;
        }

        public void GenerujWynagrodzeniaZaOkres(string okres)
        {
            if (string.IsNullOrWhiteSpace(okres))
                throw new ArgumentException("Okres jest wymagany", nameof(okres));

            var connection = db.Database.Connection;
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                connection.Open();
            }

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.usp_GenerujWynagrodzeniaZaOkres";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Okres";
                    parameter.Value = okres;
                    command.Parameters.Add(parameter);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (shouldClose)
                {
                    connection.Close();
                }
            }
        }
    }
}
