using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszyscyLektorzyViewModel : WszystkieViewModel<Lektor>
    {
        public WszyscyLektorzyViewModel() : base()
        {
            DisplayName = "Wszyscy lektorzy";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<Lektor>(db.Lektor);
        }

        public Lektor SelectedLektor
        {
            get => _selectedLektor;
            set
            {
                if (_selectedLektor == value)
                    return;

                _selectedLektor = value;
                OnPropertyChanged(nameof(SelectedLektor));

                if (_selectedLektor == null)
                    return;

                Messenger.Default.Send(_selectedLektor);
                OnRequestClose();
            }
        }

        private Lektor _selectedLektor;

        public override void Add()
        {
            Messenger.Default.Send("Wszyscy lektorzy Add");
        }
    }
}
