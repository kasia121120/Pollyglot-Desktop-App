using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.BusinessLogic;
using PollyglotDesktopApp.Models.ForAllView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Raporty
{
    public class RaportFrekwencjiViewModel : WorkspaceViewModel
    {
        #region Baza danych
        // obiekt bazy danych
        private readonly PollyglotDBEntities db;
        #endregion

        #region Logika biznesowa
        private readonly FrekwencjaRaportB raportB;
        #endregion

        #region Konstruktor
        public RaportFrekwencjiViewModel()
        {
            base.DisplayName = "Raport frekwencji";

            db = new PollyglotDBEntities();
            raportB = new FrekwencjaRaportB(db);

            LoadOkresy();

            if (!string.IsNullOrWhiteSpace(Okres))
                obliczClick();
        }
        #endregion

        #region Pola i właściwości

        private ObservableCollection<FrekwencjaRaportRow> _Rows = new ObservableCollection<FrekwencjaRaportRow>();
        public ObservableCollection<FrekwencjaRaportRow> Rows
        {
            get { return _Rows; }
            set
            {
                if (_Rows != value)
                {
                    _Rows = value ?? new ObservableCollection<FrekwencjaRaportRow>();
                    OnPropertyChanged(nameof(Rows));
                }
            }
        }

        private List<string> _Okresy = new List<string>();
        public IEnumerable<string> Okresy
        {
            get { return _Okresy; }
            set
            {
                _Okresy = value?.ToList() ?? new List<string>();
                OnPropertyChanged(nameof(Okresy));
            }
        }

        private string _Okres;
        public string Okres
        {
            get { return _Okres; }
            set
            {
                if (_Okres != value)
                {
                    _Okres = value;
                    OnPropertyChanged(nameof(Okres));
                }
            }
        }

        private int _ZaplanowaneZajecia;
        public int ZaplanowaneZajecia
        {
            get { return _ZaplanowaneZajecia; }
            set
            {
                if (_ZaplanowaneZajecia != value)
                {
                    _ZaplanowaneZajecia = value;
                    OnPropertyChanged(nameof(ZaplanowaneZajecia));
                }
            }
        }

        private int _Obecnosci;
        public int Obecnosci
        {
            get { return _Obecnosci; }
            set
            {
                if (_Obecnosci != value)
                {
                    _Obecnosci = value;
                    OnPropertyChanged(nameof(Obecnosci));
                }
            }
        }

        private int _Nieobecnosci;
        public int Nieobecnosci
        {
            get { return _Nieobecnosci; }
            set
            {
                if (_Nieobecnosci != value)
                {
                    _Nieobecnosci = value;
                    OnPropertyChanged(nameof(Nieobecnosci));
                }
            }
        }

        private decimal _SredniaFrekwencja;
        public decimal SredniaFrekwencja
        {
            get { return _SredniaFrekwencja; }
            set
            {
                if (_SredniaFrekwencja != value)
                {
                    _SredniaFrekwencja = value;
                    OnPropertyChanged(nameof(SredniaFrekwencja));
                }
            }
        }

        #endregion

        #region Komendy 

        private BaseCommand _ObliczCommand;
        public ICommand ObliczCommand
        {
            get
            {
                if (_ObliczCommand == null)
                    _ObliczCommand = new BaseCommand(obliczClick);
                return _ObliczCommand;
            }
        }

        private BaseCommand _EksportFrekwencjiCommand;
        public ICommand EksportFrekwencjiCommand
        {
            get
            {
                if (_EksportFrekwencjiCommand == null)
                    _EksportFrekwencjiCommand = new BaseCommand(eksportujClick);
                return _EksportFrekwencjiCommand;
            }
        }

        #endregion

        #region Metody prywatne 

        private void LoadOkresy()
        {
            var okresy = raportB.GetOkresy();

            if (!okresy.Any())
                okresy = new List<string> { DateTime.Today.ToString("yyyy-MM") };

            Okresy = okresy;
            Okres = Okresy.FirstOrDefault();
        }

        private void obliczClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                Rows = new ObservableCollection<FrekwencjaRaportRow>();
                ZaplanowaneZajecia = 0;
                Obecnosci = 0;
                Nieobecnosci = 0;
                SredniaFrekwencja = 0m;
                return;
            }

            var raport = raportB.GetRaport(Okres);

            Rows = new ObservableCollection<FrekwencjaRaportRow>(raport);
            ZaplanowaneZajecia = raport.Sum(r => r.ZaplanowaneZajecia);
            Obecnosci = raport.Sum(r => r.Obecnosci);
            Nieobecnosci = raport.Sum(r => r.Nieobecnosci);

            SredniaFrekwencja = raport.Any()
                ? Math.Round((decimal)raport.Average(r => r.FrekwencjaProcent), 2)
                : 0m;
        }

        private void eksportujClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                ShowMessageBox("Wybierz okres przed eksportem raportu frekwencji.");
                return;
            }

            try
            {
                var raport = raportB.GetRaport(Okres);

                var lines = new List<string>
                {
                    "ID ucznia;Imię;Nazwisko;Grupa;Zaplanowane zajęcia;Obecności;Nieobecności;Frekwencja (%)"
                };

                foreach (var row in raport)
                {
                    lines.Add(string.Join(";",
                        row.UczenId,
                        CsvValueHelper.Sanitize(row.Imie),
                        CsvValueHelper.Sanitize(row.Nazwisko),
                        CsvValueHelper.Sanitize(row.Grupa),
                        row.ZaplanowaneZajecia,
                        row.Obecnosci,
                        row.Nieobecnosci,
                        CsvValueHelper.FormatDecimal(row.FrekwencjaProcent)));
                }

                if (CsvExportHelper.ExportToCsv("raport frekwencji", $"raport_frekwencji_{Okres}.csv", lines))
                    ShowMessageBox("Zapisano raport do pliku CSV.");
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Błąd zapisu raportu: {ex.Message}");
            }
        }

        #endregion
    }
}
