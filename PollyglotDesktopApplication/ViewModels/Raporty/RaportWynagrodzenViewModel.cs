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
    public class RaportWynagrodzenViewModel : WorkspaceViewModel
    {
        #region Baza danych
        // obiekt bazy danych
        private readonly PollyglotDBEntities db;
        #endregion

        #region Logika biznesowa
        private readonly WynagrodzenieRaportB raportB;
        #endregion

        #region Konstruktor
        public RaportWynagrodzenViewModel()
        {
            base.DisplayName = "Raport wynagrodzeń";
            db = new PollyglotDBEntities();
            raportB = new WynagrodzenieRaportB(db);

            LoadOkresy();
            LoadLata();

            WybranyLektorId = Lektorzy.FirstOrDefault()?.Key ?? 0;

            var domyslnyRok = Lata.FirstOrDefault();
            if (domyslnyRok == 0)
                domyslnyRok = DateTime.Today.Year;

            WybranyRok = domyslnyRok;

            if (!string.IsNullOrWhiteSpace(Okres))
                obliczClick();
        }
        #endregion

        #region Pola i właściwości

        private ObservableCollection<WynagrodzenieRaportRow> _Rows = new ObservableCollection<WynagrodzenieRaportRow>();
        public ObservableCollection<WynagrodzenieRaportRow> Rows
        {
            get { return _Rows; }
            set
            {
                if (_Rows != value)
                {
                    _Rows = value ?? new ObservableCollection<WynagrodzenieRaportRow>();
                    OnPropertyChanged(() => Rows);
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
                OnPropertyChanged(() => Okresy);
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
                    OnPropertyChanged(() => Okres);
                }
            }
        }

        private decimal _TotalKwota;
        public decimal TotalKwota
        {
            get { return _TotalKwota; }
            set
            {
                if (_TotalKwota != value)
                {
                    _TotalKwota = value;
                    OnPropertyChanged(() => TotalKwota);
                }
            }
        }

        private decimal _TotalGodzin;
        public decimal TotalGodzin
        {
            get { return _TotalGodzin; }
            set
            {
                if (_TotalGodzin != value)
                {
                    _TotalGodzin = value;
                    OnPropertyChanged(() => TotalGodzin);
                }
            }
        }

        // ComboBox lektorów 
        public IQueryable<KeyAndValue<int>> Lektorzy
        {
            get
            {
                return new LektorB(db).GetLektorzyKeyAndValueItems();
            }
        }

        private List<int> _Lata = new List<int>();
        public IEnumerable<int> Lata
        {
            get { return _Lata; }
            set
            {
                _Lata = value?.ToList() ?? new List<int>();
                OnPropertyChanged(() => Lata);
            }
        }

        private int _WybranyLektorId;
        public int WybranyLektorId
        {
            get { return _WybranyLektorId; }
            set
            {
                if (_WybranyLektorId != value)
                {
                    _WybranyLektorId = value;
                    OnPropertyChanged(() => WybranyLektorId);
                }
            }
        }

        private int _WybranyRok;
        public int WybranyRok
        {
            get { return _WybranyRok; }
            set
            {
                if (_WybranyRok != value)
                {
                    _WybranyRok = value;
                    OnPropertyChanged(() => WybranyRok);
                }
            }
        }

        private decimal _ZarobkiRoczne;
        public decimal ZarobkiRoczne
        {
            get { return _ZarobkiRoczne; }
            set
            {
                if (_ZarobkiRoczne != value)
                {
                    _ZarobkiRoczne = value;
                    OnPropertyChanged(() => ZarobkiRoczne);
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

        private BaseCommand _GenerujWynagrodzeniaCommand;
        public ICommand GenerujWynagrodzeniaCommand
        {
            get
            {
                if (_GenerujWynagrodzeniaCommand == null)
                    _GenerujWynagrodzeniaCommand = new BaseCommand(generujClick);
                return _GenerujWynagrodzeniaCommand;
            }
        }

        private BaseCommand _EksportWynagrodzenCommand;
        public ICommand EksportWynagrodzenCommand
        {
            get
            {
                if (_EksportWynagrodzenCommand == null)
                    _EksportWynagrodzenCommand = new BaseCommand(eksportujClick);
                return _EksportWynagrodzenCommand;
            }
        }

        private BaseCommand _ObliczRocznieCommand;
        public ICommand ObliczRocznieCommand
        {
            get
            {
                if (_ObliczRocznieCommand == null)
                    _ObliczRocznieCommand = new BaseCommand(przeliczZarobkiRoczne);
                return _ObliczRocznieCommand;
            }
        }

        #endregion

        #region Metody prywatne 

        private void obliczClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                Rows = new ObservableCollection<WynagrodzenieRaportRow>();
                TotalKwota = 0m;
                TotalGodzin = 0m;
                return;
            }

            var raport = raportB.GetRaport(Okres);
            Rows = new ObservableCollection<WynagrodzenieRaportRow>(raport);
            TotalKwota = raport.Sum(r => r.KwotaDoWyplaty);
            TotalGodzin = raport.Sum(r => r.LiczbaGodzin ?? 0m);
        }

        private void generujClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                ShowMessageBox("Wybierz okres przed wygenerowaniem wynagrodzeń.");
                return;
            }

            try
            {
                raportB.GenerujWynagrodzeniaZaOkres(Okres);
                LoadOkresy(true);
                LoadLata();
                OnPropertyChanged(() => Lektorzy);
                obliczClick();
                ShowMessageBox("Wygenerowano wynagrodzenia.");
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Błąd generowania wynagrodzeń: {ex.Message}");
            }
        }

        private void eksportujClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                ShowMessageBox("Wybierz okres przed eksportem raportu wynagrodzeń.");
                return;
            }

            try
            {
                var raport = raportB.GetRaport(Okres);
                var lines = new List<string>
                {
                    "Lektor;Nazwisko;Okres;Liczba godzin;Stawka;Kwota;Status"
                };

                foreach (var row in raport)
                {
                    lines.Add(string.Join(";",
                        CsvValueHelper.Sanitize(row.Imie),
                        CsvValueHelper.Sanitize(row.Nazwisko),
                        row.Okres,
                        CsvValueHelper.FormatDecimal(row.LiczbaGodzin ?? 0m),
                        CsvValueHelper.FormatDecimal(row.StawkaGodzinowa ?? 0m),
                        CsvValueHelper.FormatDecimal(row.KwotaDoWyplaty),
                        CsvValueHelper.Sanitize(row.Status)));
                }

                if (CsvExportHelper.ExportToCsv("raport wynagrodzeń", $"raport_wynagrodzen_{Okres}.csv", lines))
                    ShowMessageBox("Zapisano raport do pliku CSV.");
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Błąd zapisu raportu: {ex.Message}");
            }
        }

        private void LoadOkresy(bool keepCurrent = false)
        {
            var okresy = raportB.GetOkresy();
            if (!okresy.Any())
                okresy = new List<string> { DateTime.Today.ToString("yyyy-MM") };

            var previous = keepCurrent ? Okres : null;
            Okresy = okresy;

            if (keepCurrent && !string.IsNullOrWhiteSpace(previous) && Okresy.Contains(previous))
                Okres = previous;
            else
                Okres = Okresy.FirstOrDefault();
        }

        private void LoadLata()
{
    var lata = db.Wynagrodzenie
        .Where(w => w.Okres != null && w.Okres != "" && w.Okres.Length >= 4)
        .Select(w => w.Okres.Substring(0, 4))
        .ToList();

    var lataInt = lata
        .Select(t =>
        {
            int year;
            return int.TryParse(t, out year) ? year : 0;
        })
        .Where(y => y > 0)
        .Distinct()
        .OrderByDescending(y => y)
        .ToList();

    if (!lataInt.Any())
        lataInt.Add(DateTime.Today.Year);

    Lata = lataInt;
}


        private void przeliczZarobkiRoczne()
        {
            if (WybranyLektorId <= 0 || WybranyRok <= 0)
            {
                ZarobkiRoczne = 0m;
                return;
            }

            var wynagrodzenia = raportB.GetWynagrodzeniaLektoraZaRok(WybranyLektorId, WybranyRok);
            ZarobkiRoczne = wynagrodzenia.Sum(w => w.KwotaDoWyplaty ?? 0m);
        }

        #endregion
    }
}
