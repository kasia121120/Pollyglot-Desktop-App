using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieGrupaUczenViewModel : WszystkieViewModel<GrupaUczenForAllView>
    {
        public WszystkieGrupaUczenViewModel()
        {
            DisplayName = "Grupy - lista uczniów";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<GrupaUczenForAllView>(
                from gu in db.GrupaUczen
                select new GrupaUczenForAllView
                {
                    GrupaId = gu.GrupaId,
                    Grupa = gu.Grupa.Nazwa,
                    UczenId = gu.UczenId,
                    Uczen = gu.Uczen.Imie + " " + gu.Uczen.Nazwisko,
                    DataDolaczenia = gu.DataDolaczenia,
                    DataZakonczenia = gu.DataZakonczenia,
                    Status = gu.Status
                }
            );
        }

        public override void Add()
        {
            Messenger.Default.Send("GrupaUczen Add");
        }
    }
}
