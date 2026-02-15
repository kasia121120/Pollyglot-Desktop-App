namespace PollyglotDesktopApp.Models.ForAllView
{
    public class FrekwencjaRaportRow
    {
        public int UczenId { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Grupa { get; set; }
        public int ZaplanowaneZajecia { get; set; }
        public int Obecnosci { get; set; }
        public int Nieobecnosci { get; set; }
        public decimal FrekwencjaProcent { get; set; }
        public string Okres { get; set; }
    }
}
