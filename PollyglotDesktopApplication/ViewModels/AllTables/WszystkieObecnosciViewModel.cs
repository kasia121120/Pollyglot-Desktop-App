using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieObecnosciViewModel : WszystkieViewModel<ObecnoscForAllView>
    {
        public WszystkieObecnosciViewModel()
        {
            DisplayName = "Obecności";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<ObecnoscForAllView>(
                from o in db.Obecnosc
                select new ObecnoscForAllView
                {
                    ObecnoscId = o.ObecnoscId,
                    Zajecia = o.Zajecia != null ? o.Zajecia.Temat : string.Empty,
                    Uczen = o.Uczen.Imie + " " + o.Uczen.Nazwisko,
                    Status = o.Status,
                    Uwagi = o.Uwagi,
                    RecordStatus = o.RecordStatus
                }
            );
        }
    }
}
