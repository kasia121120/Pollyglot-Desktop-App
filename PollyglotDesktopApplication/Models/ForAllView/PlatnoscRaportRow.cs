namespace PollyglotDesktopApp.Models.ForAllView
{
    public class PlatnoscRaportRow
    {
        public int UczenId { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string StatusRaportu { get; set; }
        public string Okres { get; set; }
        public string Uwagi { get; set; }
    }
}
