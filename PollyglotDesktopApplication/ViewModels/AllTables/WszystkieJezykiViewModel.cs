using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieJezykiViewModel : WszystkieViewModel<Jezyk>
    {
        public WszystkieJezykiViewModel() : base()
        {
            DisplayName = "Wszystkie języki";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<Jezyk>(db.Jezyk);
        }

        public Jezyk SelectedJezyk
        {
            get => _selectedJezyk;
            set
            {
                if (_selectedJezyk == value)
                    return;

                _selectedJezyk = value;
                OnPropertyChanged(nameof(SelectedJezyk));

                if (_selectedJezyk == null)
                    return;

                Messenger.Default.Send(_selectedJezyk);
                OnRequestClose();
            }
        }

        private Jezyk _selectedJezyk;

        public override void Add()
        {
            Messenger.Default.Send("Wszystkie języki Add");
        }
    }
}
