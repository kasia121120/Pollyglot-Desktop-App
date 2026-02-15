namespace PollyglotDesktopApp.Models.ForAllView
{
    public class KursForAllView
    {
        public int KursId { get; set; }
        public string NazwaKursu { get; set; }
        public string Jezyk { get; set; }
        public int JezykId { get; set; }
        public string RodzajKursu { get; set; }
        public int RodzajKursuId { get; set; }
        public string Poziom { get; set; }
        public decimal? CenaMiesieczna { get; set; }
        public string Tryb { get; set; }
    }
}
