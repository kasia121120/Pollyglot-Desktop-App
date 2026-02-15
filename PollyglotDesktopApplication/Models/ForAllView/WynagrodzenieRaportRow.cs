namespace PollyglotDesktopApp.Models.ForAllView
{
    public class WynagrodzenieRaportRow
    {
        public int LektorId { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Okres { get; set; }
        public decimal? LiczbaGodzin { get; set; }
        public decimal? StawkaGodzinowa { get; set; }
        public decimal KwotaDoWyplaty { get; set; }
        public string Status { get; set; }
    }
}
