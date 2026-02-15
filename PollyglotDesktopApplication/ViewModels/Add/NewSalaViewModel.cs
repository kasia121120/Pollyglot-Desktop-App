using PollyglotDesktopApp.ViewModels.Abstract;
using PollyglotDesktopApp.Models;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewSalaViewModel : JedenViewModel<Sala>
    {
        #region Konstruktor
        public NewSalaViewModel()
            : base()
        {
            DisplayName = "Nowa sala";
            item = new Sala();
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

        public int? Pojemnosc
        {
            get => item.Pojemnosc;
            set
            {
                if (item.Pojemnosc != value)
                {
                    item.Pojemnosc = value;
                    OnPropertyChanged(nameof(Pojemnosc));
                }
            }
        }

        public string Lokalizacja
        {
            get => item.Lokalizacja;
            set
            {
                if (item.Lokalizacja != value)
                {
                    item.Lokalizacja = value;
                    OnPropertyChanged(nameof(Lokalizacja));
                }
            }
        }

        public string Wyposazenie
        {
            get => item.Wyposazenie;
            set
            {
                if (item.Wyposazenie != value)
                {
                    item.Wyposazenie = value;
                    OnPropertyChanged(nameof(Wyposazenie));
                }
            }
        }

        public string Typ
        {
            get => item.Typ;
            set
            {
                if (item.Typ != value)
                {
                    item.Typ = value;
                    OnPropertyChanged(nameof(Typ));
                }
            }
        }

        #endregion

        #region Komendy
        public override void Save()
        {
            db.Sala.Add(item);
            db.SaveChanges();
        }
        #endregion
    }
}
