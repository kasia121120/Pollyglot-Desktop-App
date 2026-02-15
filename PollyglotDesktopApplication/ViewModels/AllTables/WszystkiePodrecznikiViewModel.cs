using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;
using System.Linq;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkiePodrecznikiViewModel : WszystkieViewModel<PodrecznikForAllView>
    {
        private PodrecznikForAllView _wybranyPodrecznik;

        public WszystkiePodrecznikiViewModel()
        {
            DisplayName = "Podręczniki";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<PodrecznikForAllView>(
                from p in db.Podrecznik
                select new PodrecznikForAllView
                {
                    PodrecznikId = p.PodrecznikId,
                    Tytul = p.Tytul,
                    Autor = p.Autor,
                    Wydawnictwo = p.Wydawnictwo,
                    Jezyk = p.Jezyk.Nazwa,
                    JezykId = p.JezykId,
                    Poziom = p.Poziom,
                    RokWydania = p.RokWydania,
                    Opis = p.Opis
                }
            );
        }

        public PodrecznikForAllView WybranyPodrecznik
        {
            get => _wybranyPodrecznik;
            set
            {
                if (_wybranyPodrecznik == value)
                    return;

                _wybranyPodrecznik = value;
                if (value == null)
                    return;

                Messenger.Default.Send(value);
                OnRequestClose();
            }
        }
    }
}

