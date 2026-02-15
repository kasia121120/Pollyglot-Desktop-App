using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.Models.Validatory;
using PollyglotDesktopApp.ViewModels.Abstract;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewGrupaUczenViewModel : JedenViewModel<GrupaUczen>, IDataErrorInfo
    {
        private ICommand _showGrupyCommand;
        private ICommand _showUczniowieCommand;
        private string _grupaDane = "Brak wybranej grupy";
        private string _uczenDane = "Brak wybranego ucznia";

        public NewGrupaUczenViewModel()
            : base()
        {
            DisplayName = "Nowy uczeń - grupa";
            item = new GrupaUczen
            {
                DataDolaczenia = DateTime.Today
            };

            Messenger.Default.Register<GrupaForAllView>(this, OnGrupaSelected);
            Messenger.Default.Register<Uczen>(this, OnUczenSelected);
        }

        public ICommand ShowGrupyCommand
        {
            get
            {
                if (_showGrupyCommand == null)
                    _showGrupyCommand = new BaseCommand(() => Messenger.Default.Send("Grupy Show"));
                return _showGrupyCommand;
            }
        }

        public ICommand ShowUczniowieCommand
        {
            get
            {
                if (_showUczniowieCommand == null)
                    _showUczniowieCommand = new BaseCommand(() => Messenger.Default.Send("Uczniowie Show"));
                return _showUczniowieCommand;
            }
        }

        public string GrupaDane
        {
            get => _grupaDane;
            private set
            {
                if (_grupaDane == value)
                    return;

                _grupaDane = value;
                OnPropertyChanged(nameof(GrupaDane));
            }
        }

        public string UczenDane
        {
            get => _uczenDane;
            private set
            {
                if (_uczenDane == value)
                    return;

                _uczenDane = value;
                OnPropertyChanged(nameof(UczenDane));
            }
        }

        public int? GrupaId
        {
            get => item.GrupaId != 0 ? item.GrupaId : (int?)null;
            private set
            {
                if (value == null)
                {
                if (item.GrupaId == 0)
                    return;

                item.GrupaId = 0;
                OnPropertyChanged(nameof(GrupaId));
                OnPropertyChanged(nameof(GrupaError));
                return;
            }

            if (item.GrupaId == value.Value)
                return;

            item.GrupaId = value.Value;
            OnPropertyChanged(nameof(GrupaId));
            OnPropertyChanged(nameof(GrupaError));
        }
    }

        public int? UczenId
        {
            get => item.UczenId != 0 ? item.UczenId : (int?)null;
            private set
            {
                if (value == null)
                {
                    if (item.UczenId == 0)
                        return;

                    item.UczenId = 0;
                    OnPropertyChanged(nameof(UczenId));
                    OnPropertyChanged(nameof(UczenError));
                    return;
                }

                if (item.UczenId == value.Value)
                    return;

                item.UczenId = value.Value;
                OnPropertyChanged(nameof(UczenId));
                OnPropertyChanged(nameof(UczenError));
            }
        }

        public DateTime DataDolaczenia
        {
            get => item.DataDolaczenia ?? DateTime.Today;
            set
            {
                if (item.DataDolaczenia == value)
                    return;

                item.DataDolaczenia = value;
                OnPropertyChanged(nameof(DataDolaczenia));
                OnPropertyChanged(nameof(DataDolaczeniaError));
                OnPropertyChanged(nameof(DataZakonczeniaError));
            }
        }

        public DateTime? DataZakonczenia
        {
            get => item.DataZakonczenia;
            set
            {
                if (item.DataZakonczenia == value)
                    return;

                item.DataZakonczenia = value;
                OnPropertyChanged(nameof(DataZakonczenia));
                OnPropertyChanged(nameof(DataZakonczeniaError));
            }
        }

        public override void Save()
        {
            if (!IsValid())
                return;
                item.Status = "aktywny";
            item.DataDolaczenia = DataDolaczenia;
            item.DataZakonczenia = DataZakonczenia;
            db.GrupaUczen.Add(item);
            db.SaveChanges();
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(GrupaId):
                        return GrupaId.HasValue ? null : "Wybierz grupę.";
                    case nameof(UczenId):
                        if (!UczenId.HasValue)
                            return "Wybierz ucznia.";
                        return SprawdzCzyUczenJuzJestWGrupie();
                    case nameof(DataDolaczenia):
                        return DataDolaczenia == default ? "Data dołączenia jest wymagana." : null;
                    case nameof(DataZakonczenia):
                        return DateValidator.SprawdzCzyDataZakonczeniaNieJestPrzedDolaczeniem(DataDolaczenia, DataZakonczenia);
                    default:
                        return null;
                }
            }
        }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(GrupaId)])
                && string.IsNullOrEmpty(this[nameof(UczenId)])
                && string.IsNullOrEmpty(this[nameof(DataDolaczenia)])
                && string.IsNullOrEmpty(this[nameof(DataZakonczenia)]);
        }

        public string GrupaError => this[nameof(GrupaId)];
        public string UczenError => this[nameof(UczenId)];
        public string DataDolaczeniaError => this[nameof(DataDolaczenia)];
        public string DataZakonczeniaError => this[nameof(DataZakonczenia)];

        private string SprawdzCzyUczenJuzJestWGrupie()
        {
            if (!UczenId.HasValue)
                return null;

            var dzis = DateTime.Today;
            var aktywnaGrupa = db.GrupaUczen
                .Where(gu => gu.UczenId == UczenId.Value)
                .ToList() 
                .Where(gu => !gu.DataZakonczenia.HasValue || gu.DataZakonczenia.Value.Date >= dzis)
                .Select(gu => gu.Grupa.Nazwa)
                .FirstOrDefault();


            if (!string.IsNullOrEmpty(aktywnaGrupa))
                return $"Uczeń jest już w grupie {aktywnaGrupa}.";

            return null;
        }

        private void OnGrupaSelected(GrupaForAllView grupa)
        {
            if (grupa == null)
                return;

            GrupaId = grupa.GrupaId;
            GrupaDane = $"{grupa.Nazwa} ({grupa.Kurs})";
        }

        private void OnUczenSelected(Uczen uczen)
        {
            if (uczen == null)
                return;

            UczenId = uczen.UczenId;
            UczenDane = $"{uczen.Imie} {uczen.Nazwisko}";
        }
    }
}
