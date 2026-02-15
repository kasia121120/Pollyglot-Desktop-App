using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieGrupyViewModel : WszystkieViewModel<GrupaForAllView>
    {
        private GrupaForAllView _selectedGroup;

        public WszystkieGrupyViewModel() : base()
        {
            DisplayName = "Grupy";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<GrupaForAllView>(
                from g in db.Grupa
                select new GrupaForAllView
                {
                    GrupaId = g.GrupaId,
                    Nazwa = g.Nazwa,
                    Kurs = g.Kurs.NazwaKursu,
                    Jezyk = g.Kurs.Jezyk.Nazwa,
                    RodzajKursu = g.Kurs.RodzajKursu.Nazwa,
                    Lektor = g.Lektor.Imie + " " + g.Lektor.Nazwisko,
                    Sala = g.Sala.Nazwa,
                    Podrecznik = g.Podrecznik.Tytul,
                    Dzien = g.DzienTygodnia,
                    Tryb = g.Tryb,
                    Status = g.Status
                }
            );
        }

        public GrupaForAllView SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup == value)
                    return;

                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }

        public void SelectGroup(GrupaForAllView grupa)
        {
            if (grupa == null)
                return;

            Messenger.Default.Send(grupa);
            OnRequestClose();
        }
    }
}
