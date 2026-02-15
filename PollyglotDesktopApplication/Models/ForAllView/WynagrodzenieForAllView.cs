using System;

namespace PollyglotDesktopApp.Models.ForAllView
{
    public class WynagrodzenieForAllView
    {
        public int WynagrodzenieId { get; set; }
        public string Lektor { get; set; }
        public string Okres { get; set; }
        public decimal? LiczbaGodzin { get; set; }
        public decimal? StawkaGodzinowa { get; set; }
        public decimal? KwotaDoWyplaty { get; set; }
        public DateTime? DataWyplaty { get; set; }
        public string Status { get; set; }

        public string DataWyplatyDisplay => DataWyplaty?.ToString("dd.MM.yyyy");
    }
}
