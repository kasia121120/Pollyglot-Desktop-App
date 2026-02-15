using System;

namespace PollyglotDesktopApp.Models.ForAllView
{
    public class GrupaUczenForAllView
    {
        public int GrupaId { get; set; }
        public string Grupa { get; set; }
        public int UczenId { get; set; }
        public string Uczen { get; set; }
        public DateTime? DataDolaczenia { get; set; }
        public DateTime? DataZakonczenia { get; set; }
        public string Status { get; set; }

        public string DataDolaczeniaDisplay => DataDolaczenia?.ToString("dd.MM.yyyy");
        public string DataZakonczeniaDisplay => DataZakonczenia?.ToString("dd.MM.yyyy");
    }
}
