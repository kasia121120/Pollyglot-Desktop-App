using System;

namespace PollyglotDesktopApp.Models.ForAllView
{
    public class ZajeciaForAllView
    {
        public int ZajeciaId { get; set; }
        public DateTime? Data { get; set; }
        public TimeSpan? GodzinaStart { get; set; }
        public TimeSpan? GodzinaKoniec { get; set; }
        public string Lektor { get; set; }
        public string Sala { get; set; }
        public string Grupa { get; set; }
        public int? GrupaId { get; set; }
        public string Temat { get; set; }
        public string Uczen { get; set; }
        public string Tryb { get; set; }
        public string DzienTygodnia { get; set; }
        public string RodzajZajec { get; set; }
        public string DataDisplay => Data?.ToString("dd.MM.yyyy");
    }
}
