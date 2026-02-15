using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.BusinessLogic;
using System;
using PollyglotDesktopApp.Models.ForAllView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Raporty
{
    public class RaportPlatnosciViewModel : WorkspaceViewModel
    {
        #region Baza danych
        // obiekt bazy danych
        private readonly PollyglotDBEntities db;
        #endregion

        #region Logika biznesowa
        private readonly PlatnoscB platnoscB;       // do list (ComboBox)
        private readonly ZaleglosciB zaleglosciB;   // do raportu
        #endregion

        #region Konstruktor
        public RaportPlatnosciViewModel()
        {
            base.DisplayName = "Raport płatności";

            db = new PollyglotDBEntities();
            platnoscB = new PlatnoscB(db);
            zaleglosciB = new ZaleglosciB(db);

            LoadOkresy();

            if (!string.IsNullOrWhiteSpace(Okres))
                obliczClick();
        }
        #endregion

        #region Pola i właściwości

        private ObservableCollection<PlatnoscRaportRow> _Rows = new ObservableCollection<PlatnoscRaportRow>();
        public ObservableCollection<PlatnoscRaportRow> Rows
        {
            get { return _Rows; }
            set
            {
                if (_Rows != value)
                {
                    _Rows = value ?? new ObservableCollection<PlatnoscRaportRow>();
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

        private decimal _ExpectedTotal;
        public decimal ExpectedTotal
        {
            get { return _ExpectedTotal; }
            set
            {
                if (_ExpectedTotal != value)
                {
                    _ExpectedTotal = value;
                    OnPropertyChanged(nameof(ExpectedTotal));
                }
            }
        }

        private decimal _PaidTotal;
        public decimal PaidTotal
        {
            get { return _PaidTotal; }
            set
            {
                if (_PaidTotal != value)
                {
                    _PaidTotal = value;
                    OnPropertyChanged(nameof(PaidTotal));
                }
            }
        }

        private decimal _BalanceTotal;
        public decimal BalanceTotal
        {
            get { return _BalanceTotal; }
            set
            {
                if (_BalanceTotal != value)
                {
                    _BalanceTotal = value;
                    OnPropertyChanged(nameof(BalanceTotal));
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

        private BaseCommand _EksportPlatnosciCommand;
        public ICommand EksportPlatnosciCommand
        {
            get
            {
                if (_EksportPlatnosciCommand == null)
                    _EksportPlatnosciCommand = new BaseCommand(eksportujClick);
                return _EksportPlatnosciCommand;
            }
        }

        #endregion

        #region Metody prywatne

        private void LoadOkresy()
        {
            var okresy = platnoscB
                .GetOkresyKeyAndValueItems()
                .Select(x => x.Key)
                .ToList();

            if (!okresy.Any())
                okresy = new List<string> { DateTime.Today.ToString("yyyy-MM") };

            Okresy = okresy;
            Okres = Okresy.FirstOrDefault();
        }

        private void obliczClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                Rows = new ObservableCollection<PlatnoscRaportRow>();
                ExpectedTotal = 0m;
                PaidTotal = 0m;
                BalanceTotal = 0m;
                return;
            }

            // raport liczony w klasie logiki biznesowej
            var raport = zaleglosciB.RaportZaleglosci(Okres);

            Rows = new ObservableCollection<PlatnoscRaportRow>(raport);
            ExpectedTotal = raport.Sum(r => r.ExpectedAmount);
            PaidTotal = raport.Sum(r => r.PaidAmount);
            BalanceTotal = raport.Sum(r => r.Balance);
        }

        private void eksportujClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                ShowMessageBox("Wybierz okres przed eksportem raportu płatności.");
                return;
            }

            try
            {
                var lines = new List<string>
                {
                    "ID ucznia;Imię;Nazwisko;Okres;Oczekiwane;Zapłacone;Saldo;Status;Uwagi"
                };

                foreach (var row in Rows)
                {
                    lines.Add(string.Join(";",
                        row.UczenId,
                        CsvValueHelper.Sanitize(row.Imie),
                        CsvValueHelper.Sanitize(row.Nazwisko),
                        row.Okres,
                        CsvValueHelper.FormatDecimal(row.ExpectedAmount),
                        CsvValueHelper.FormatDecimal(row.PaidAmount),
                        CsvValueHelper.FormatDecimal(row.Balance),
                        CsvValueHelper.Sanitize(row.StatusRaportu),
                        CsvValueHelper.Sanitize(row.Uwagi)));
                }

                if (CsvExportHelper.ExportToCsv("raport płatności", $"raport_platnosci_{Okres}.csv", lines))
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
