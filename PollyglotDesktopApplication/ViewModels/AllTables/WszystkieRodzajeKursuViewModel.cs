using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieRodzajeKursuViewModel : WszystkieViewModel<RodzajKursu>
    {
        public WszystkieRodzajeKursuViewModel() : base()
        {
            DisplayName = "Wszystkie rodzaje kursu";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<RodzajKursu>(db.RodzajKursu);
        }

        public RodzajKursu SelectedRodzaj
        {
            get => _selectedRodzaj;
            set
            {
                if (_selectedRodzaj == value)
                    return;

                _selectedRodzaj = value;
                OnPropertyChanged(nameof(SelectedRodzaj));

                if (_selectedRodzaj == null)
                    return;

                Messenger.Default.Send(_selectedRodzaj);
                OnRequestClose();
            }
        }

        private RodzajKursu _selectedRodzaj;

        public void SelectItem(RodzajKursu rodzaj)
        {
            if (rodzaj == null)
                return;

            SelectedRodzaj = rodzaj;
        }

        public override void Add()
        {
            Messenger.Default.Send("Wszystkie rodzaje kursu Add");
        }
    }
}
