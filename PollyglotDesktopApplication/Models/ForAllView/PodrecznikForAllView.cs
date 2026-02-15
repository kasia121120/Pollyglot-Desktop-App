namespace PollyglotDesktopApp.Models.ForAllView
{
    public class PodrecznikForAllView
    {
        public int PodrecznikId { get; set; }
        public string Tytul { get; set; }
        public string Autor { get; set; }
        public string Wydawnictwo { get; set; }
        public string Jezyk { get; set; }
        public int JezykId { get; set; }
        public string Poziom { get; set; }
        public int? RokWydania { get; set; }
        public string Opis { get; set; }
    }
}
