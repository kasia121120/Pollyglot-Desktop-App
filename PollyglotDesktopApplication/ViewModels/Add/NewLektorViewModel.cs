using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewLektorViewModel : JedenViewModel<Lektor>
    {
        #region Konstruktor
        public NewLektorViewModel()
            : base()
        {
            DisplayName = "Nowy lektor";

            item = new Lektor()
            {
                Aktywny = true,
                DataZatrudnienia = DateTime.Now
            };



        }
        #endregion

        #region Właściwości

        public string Imie
        {
            get => item.Imie;
            set
            {
                if (item.Imie != value)
                {
                    item.Imie = value;
                    OnPropertyChanged(nameof(Imie));
                }
            }
        }

        public string Nazwisko
        {
            get => item.Nazwisko;
            set
            {
                if (item.Nazwisko != value)
                {
                    item.Nazwisko = value;
                    OnPropertyChanged(nameof(Nazwisko));
                }
            }
        }

        public string Email
        {
            get => item.Email;
            set
            {
                if (item.Email != value)
                {
                    item.Email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Telefon
        {
            get => item.Telefon;
            set
            {
                if (item.Telefon != value)
                {
                    item.Telefon = value;
                    OnPropertyChanged(nameof(Telefon));
                }
            }
        }

        public decimal StawkaGodzinowa
        {
            get => item.StawkaGodzinowa;
            set
            {
                if (item.StawkaGodzinowa != value)
                {
                    item.StawkaGodzinowa = value;
                    OnPropertyChanged(nameof(StawkaGodzinowa));
                }
            }
        }

        public string FormaZatrudnienia
        {
            get => item.FormaZatrudnienia;
            set
            {
                if (item.FormaZatrudnienia != value)
                {
                    item.FormaZatrudnienia = value;
                    OnPropertyChanged(nameof(FormaZatrudnienia));
                }
            }
        }

        public DateTime? DataZatrudnienia
        {
            get => item.DataZatrudnienia;
            set
            {
                if (item.DataZatrudnienia != value)
                {
                    item.DataZatrudnienia = value;
                    OnPropertyChanged(nameof(DataZatrudnienia));
                }
            }
        }


        public bool? Aktywny
        {
            get => item.Aktywny;
            set
            {
                if (item.Aktywny != value)
                {
                    item.Aktywny = value;
                    OnPropertyChanged(nameof(Aktywny));
                }
            }
        }

        #endregion

        #region Komendy
        public override void Save()
        {
            db.Lektor.Add(item);
            db.SaveChanges();
        }
        #endregion
    }
}
