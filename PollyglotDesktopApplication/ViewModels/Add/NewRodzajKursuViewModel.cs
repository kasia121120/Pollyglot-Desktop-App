using PollyglotDesktopApp.ViewModels.Abstract;
using PollyglotDesktopApp.Models;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewRodzajKursuViewModel : JedenViewModel<RodzajKursu>
    {
        #region Konstruktor
        public NewRodzajKursuViewModel()
            : base()
        {
            DisplayName = "Nowy rodzaj kursu";
            item = new RodzajKursu();  
        }
        #endregion

        #region Właściwości

        public string Nazwa
        {
            get => item.Nazwa;
            set
            {
                if (item.Nazwa != value)
                {
                    item.Nazwa = value;
                    OnPropertyChanged(nameof(Nazwa));
                }
            }
        }

        public string Opis
        {
            get => item.Opis;
            set
            {
                if (item.Opis != value)
                {
                    item.Opis = value;
                    OnPropertyChanged(nameof(Opis));
                }
            }
        }

        #endregion

        #region Komendy
        public override void Save()
        {
            db.RodzajKursu.Add(item);
            db.SaveChanges();
        }
        #endregion
    }
}
