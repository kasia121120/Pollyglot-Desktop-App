using System;

namespace PollyglotDesktopApp.Models.ForAllView
{
    public class OcenaForAllView
    {
        public int OcenaId { get; set; }
        public string Uczen { get; set; }
        public string Zajecia { get; set; }
        public string Rodzaj { get; set; }
        public string Wartosc { get; set; }
        public string TematOceny { get; set; }
        public DateTime? DataOceny { get; set; }
        public string DataOcenyDisplay => DataOceny?.ToString("dd.MM.yyyy");

    }
}
