using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewUczenViewModel : JedenViewModel<Uczen>
    {
        public NewUczenViewModel() : base()
        {
            DisplayName = "Nowy uczeń";
            item = new Uczen
            {
                DataZapisu = DateTime.Now,
                Status = "aktywny"
            };
        }

        public string Imie
        {
            get => item.Imie;
            set { item.Imie = value; OnPropertyChanged(nameof(Imie)); }
        }

        public string Nazwisko
        {
            get => item.Nazwisko;
            set { item.Nazwisko = value; OnPropertyChanged(nameof(Nazwisko)); }
        }

        public string PESEL
        {
            get => item.PESEL;
            set { item.PESEL = value; OnPropertyChanged(nameof(PESEL)); }
        }

        public string Email
        {
            get => item.Email;
            set { item.Email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Telefon
        {
            get => item.Telefon;
            set { item.Telefon = value; OnPropertyChanged(nameof(Telefon)); }
        }

        public DateTime? DataZapisu
        {
            get => item.DataZapisu;
            set { item.DataZapisu = value; OnPropertyChanged(nameof(DataZapisu)); }
        }

        public string Status
        {
            get => item.Status;
            set { item.Status = value; OnPropertyChanged(nameof(Status)); }
        }

        public override void Save()
        {
            db.Uczen.Add(item);
            db.SaveChanges();
        }
    }
}
