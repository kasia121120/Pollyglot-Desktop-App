using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieLektorJezykViewModel : WszystkieViewModel<LektorJezykForAllView>
    {
        public WszystkieLektorJezykViewModel()
        {
            DisplayName = "Kompetencje lektorów";
            ShowSortFilter = false;
        }

        public override void load()
        {
            List = new ObservableCollection<LektorJezykForAllView>(
                from lj in db.LektorJezyk
                select new LektorJezykForAllView
                {
                    Lektor = lj.Lektor.Imie + " " + lj.Lektor.Nazwisko,
                    Jezyk = lj.Jezyk.Nazwa,
                    PoziomKompetencji = lj.PoziomKompetencji,
                    DoswiadczenieLat = lj.DoswiadczenieLat,
                    Certyfikat = lj.Certyfikat,
                    Specjalizacja = lj.Specjalizacja
                }
            );
        }

        public override void Add()
        {
            Messenger.Default.Send("Kompetencje lektorów Add");
        }
    }
}
