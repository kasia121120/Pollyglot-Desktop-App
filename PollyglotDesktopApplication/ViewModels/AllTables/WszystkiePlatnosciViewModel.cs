using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkiePlatnosciViewModel : WszystkieViewModel<PlatnoscForAllView>
    {
        public WszystkiePlatnosciViewModel()
        {
            DisplayName = "Płatności";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<PlatnoscForAllView>(
                from p in db.Platnosc
                select new PlatnoscForAllView
                {
                    PlatnoscId = p.PlatnoscId,
                    Uczen = p.Uczen.Imie + " " + p.Uczen.Nazwisko,
                    Kurs = p.Kurs.NazwaKursu,
                    Okres = p.Okres,
                    Kwota = p.Kwota,
                    DataPlatnosci = p.DataPlatnosci,
                    Metoda = p.Metoda,
                    Status = p.Status
                }
            );
        }
    }
}
