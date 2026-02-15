using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieOcenyViewModel : WszystkieViewModel<OcenaForAllView>
    {
        public WszystkieOcenyViewModel()
        {
            DisplayName = "Oceny";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<OcenaForAllView>(
                from o in db.Ocena
                select new OcenaForAllView
                {
                    OcenaId = o.OcenaId,
                    Uczen = o.Uczen.Imie + " " + o.Uczen.Nazwisko,
                    Zajecia = o.Zajecia.Temat,
                    Rodzaj = o.Rodzaj,
                    Wartosc = o.Wartosc,
                    TematOceny = o.TematOceny,
                    DataOceny = o.DataOceny
                }
            );
        }
    }
}
