using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Collections.ObjectModel;
using System.Linq;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieKursyViewModel : WszystkieViewModel<KursForAllView>
    {
        private KursForAllView _wybranyKurs;

        public WszystkieKursyViewModel()
        {
            DisplayName = "Kursy";
            ShowSortFilter = false;
        }

        public override bool ShowAddButton => false;

        public override void load()
        {
            List = new ObservableCollection<KursForAllView>(
                from k in db.Kurs
                select new KursForAllView
                {
                    KursId = k.KursId,
                    NazwaKursu = k.NazwaKursu,
                    Jezyk = k.Jezyk.Nazwa,
                    JezykId = k.JezykId,
                    RodzajKursu = k.RodzajKursu.Nazwa,
                    RodzajKursuId = k.RodzajKursuId,
                    Poziom = k.Poziom,
                    CenaMiesieczna = k.CenaMiesieczna,
                    Tryb = k.Tryb
                }
            );
        }

        protected override void Sort()
        {
            throw new System.NotImplementedException();
        }

        protected override void Find()
        {
            throw new System.NotImplementedException();
        }

        public KursForAllView WybranyKurs
        {
            get => _wybranyKurs;
            set
            {
                if (_wybranyKurs == value)
                    return;

                _wybranyKurs = value;
                if (value == null)
                    return;

                Messenger.Default.Send(value);
                OnRequestClose();
            }
        }
    }
}
