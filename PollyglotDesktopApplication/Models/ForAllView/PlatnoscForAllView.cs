using System;

namespace PollyglotDesktopApp.Models.ForAllView
{
    public class PlatnoscForAllView
    {
        public int PlatnoscId { get; set; }
        public string Uczen { get; set; }
        public string Kurs { get; set; }
        public string Okres { get; set; }
        public decimal? Kwota { get; set; }
        public DateTime? DataPlatnosci { get; set; }
        public string Metoda { get; set; }
        public string Status { get; set; }

        public string DataPlatnosciDisplay => DataPlatnosci?.ToString("dd.MM.yyyy");
    }
}
