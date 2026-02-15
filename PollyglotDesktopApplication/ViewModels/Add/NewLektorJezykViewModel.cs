using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewLektorJezykViewModel : JedenViewModel<LektorJezyk>, IDataErrorInfo
    {
        private ICommand _showLektorzyCommand;
        private ICommand _showJezykiCommand;
        private string _lektorDane = "Brak wybranego lektora";
        private string _jezykDane = "Brak wybranego języka";

        public NewLektorJezykViewModel()
        {
            DisplayName = "Nowa kompetencja";
            item = new LektorJezyk();

            Messenger.Default.Register<Lektor>(this, OnLektorSelected);
            Messenger.Default.Register<Jezyk>(this, OnJezykSelected);
        }

        public ICommand ShowLektorzyCommand
        {
            get
            {
                if (_showLektorzyCommand == null)
                    _showLektorzyCommand = new BaseCommand(() => Messenger.Default.Send("Lektorzy Show"));
                return _showLektorzyCommand;
            }
        }

        public ICommand ShowJezykiCommand
        {
            get
            {
                if (_showJezykiCommand == null)
                    _showJezykiCommand = new BaseCommand(() => Messenger.Default.Send("Jezyki Show"));
                return _showJezykiCommand;
            }
        }

        public int? LektorId
        {
            get => item.LektorId != 0 ? (int?)item.LektorId : null;
            private set
            {
                var newValue = value ?? 0;
                if (item.LektorId == newValue)
                    return;
                item.LektorId = newValue;
                OnPropertyChanged(nameof(LektorId));
                OnPropertyChanged(nameof(LektorError));
                OnPropertyChanged(nameof(HasLektorError));
                OnPropertyChanged(nameof(JezykError));
                OnPropertyChanged(nameof(HasJezykError));
            }
        }

        public int? JezykId
        {
            get => item.JezykId != 0 ? (int?)item.JezykId : null;
            private set
            {
                var newValue = value ?? 0;
                if (item.JezykId == newValue)
                    return;
                item.JezykId = newValue;
                OnPropertyChanged(nameof(JezykId));
                OnPropertyChanged(nameof(JezykError));
                OnPropertyChanged(nameof(HasJezykError));
                OnPropertyChanged(nameof(LektorError));
                OnPropertyChanged(nameof(HasLektorError));
            }
        }

        public string PoziomKompetencji
        {
            get => item.PoziomKompetencji;
            set
            {
                if (item.PoziomKompetencji == value)
                    return;
                item.PoziomKompetencji = value;
                OnPropertyChanged(nameof(PoziomKompetencji));
                OnPropertyChanged(nameof(PoziomError));
            }
        }

        public int? DoswiadczenieLat
        {
            get => item.DoswiadczenieLat;
            set
            {
                if (item.DoswiadczenieLat == value)
                    return;
                item.DoswiadczenieLat = value;
                OnPropertyChanged(nameof(DoswiadczenieLat));
                OnPropertyChanged(nameof(DoswiadczenieError));
            }
        }

        public string Certyfikat
        {
            get => item.Certyfikat;
            set
            {
                if (item.Certyfikat == value)
                    return;
                item.Certyfikat = value;
                OnPropertyChanged(nameof(Certyfikat));
            }
        }

        public string Specjalizacja
        {
            get => item.Specjalizacja;
            set
            {
                if (item.Specjalizacja == value)
                    return;
                item.Specjalizacja = value;
                OnPropertyChanged(nameof(Specjalizacja));
            }
        }

        public string Uwagi
        {
            get => item.Uwagi;
            set
            {
                if (item.Uwagi == value)
                    return;
                item.Uwagi = value;
                OnPropertyChanged(nameof(Uwagi));
            }
        }

        public string LektorDane
        {
            get => _lektorDane;
            private set
            {
                if (_lektorDane == value)
                    return;
                _lektorDane = value;
                OnPropertyChanged(nameof(LektorDane));
            }
        }

        public string JezykDane
        {
            get => _jezykDane;
            private set
            {
                if (_jezykDane == value)
                    return;
                _jezykDane = value;
                OnPropertyChanged(nameof(JezykDane));
            }
        }

        public string LektorError => this[nameof(LektorId)];
        public string JezykError => this[nameof(JezykId)];
        public string PoziomError => this[nameof(PoziomKompetencji)];
        public string DoswiadczenieError => this[nameof(DoswiadczenieLat)];
        public bool HasLektorError => !string.IsNullOrEmpty(LektorError);
        public bool HasJezykError => !string.IsNullOrEmpty(JezykError);

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(LektorId):
                        return LektorId.HasValue ? SprawdzCzyLektorMaJuzJezyk() : "Wybierz lektora.";
                    case nameof(JezykId):
                        return JezykId.HasValue ? SprawdzCzyLektorMaJuzJezyk() : "Wybierz język.";
                    case nameof(PoziomKompetencji):
                        if (string.IsNullOrWhiteSpace(item.PoziomKompetencji))
                            return "Poziom kompetencji jest wymagany.";
                        return item.PoziomKompetencji.Trim().Length < 2
                            ? "Poziom kompetencji jest za krótki."
                            : null;
                    case nameof(DoswiadczenieLat):
                        if (!item.DoswiadczenieLat.HasValue)
                        {
                            return "Doświadczenie (lata) jest wymagane.";
                        }
                        return item.DoswiadczenieLat.HasValue && (item.DoswiadczenieLat < 0 || item.DoswiadczenieLat > 60)
                            ? "Doświadczenie (lata) musi być z zakresu 0–60."
                            : null;
                    default:
                        return null;
                }
            }
        }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(LektorError)
                && string.IsNullOrEmpty(JezykError)
                && string.IsNullOrEmpty(PoziomError)
                && string.IsNullOrEmpty(DoswiadczenieError);
        }

        private string SprawdzCzyLektorMaJuzJezyk()
        {
            if (!LektorId.HasValue)
                return null;

            var istnieje = db.LektorJezyk.Any(lj => lj.LektorId == LektorId.Value);
            return istnieje ? "Ten lektor ma już przypisany język." : null;
        }

        public override void Save()
        {
            db.LektorJezyk.Add(item);
            db.SaveChanges();
        }

        private void OnLektorSelected(Lektor lektor)
        {
            if (lektor == null)
                return;

            LektorId = lektor.LektorId;
            LektorDane = $"{lektor.Imie} {lektor.Nazwisko}";
        }

        private void OnJezykSelected(Jezyk jezyk)
        {
            if (jezyk == null)
                return;

            JezykId = jezyk.JezykId;
            JezykDane = jezyk.Nazwa;
        }
    }
}
