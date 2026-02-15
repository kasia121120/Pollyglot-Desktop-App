using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieUczenKursViewModel : WszystkieViewModel<UczenKursForAllView>
    {
        public WszystkieUczenKursViewModel()
        {
            DisplayName = "UczenKurs";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<UczenKursForAllView>(
                from uk in db.UczenKurs
                select new UczenKursForAllView
                {
                    Uczen = uk.Uczen.Imie + " " + uk.Uczen.Nazwisko,
                    Kurs = uk.Kurs.NazwaKursu,
                    Jezyk = uk.Jezyk.Nazwa,
                    RodzajKursu = uk.RodzajKursu.Nazwa,
                    Lektor = uk.Lektor.Imie + " " + uk.Lektor.Nazwisko,
                    Podrecznik = uk.Podrecznik.Tytul,
                    Status = uk.Status
                }
            );
        }

        public override void Add()
        {
            Messenger.Default.Send("UczenKurs Add");
        }
    }
}
