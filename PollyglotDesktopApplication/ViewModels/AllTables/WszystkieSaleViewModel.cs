using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieSaleViewModel : WszystkieViewModel<Sala>
    {
        public WszystkieSaleViewModel() : base()
        {
            DisplayName = "Wszystkie sale";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<Sala>(db.Sala);
        }

        public override void Add()
        {
            Messenger.Default.Send("Wszystkie sale Add");
        }
    }
}
