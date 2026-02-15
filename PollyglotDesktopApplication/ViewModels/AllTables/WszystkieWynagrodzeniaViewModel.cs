using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.BusinessLogic;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieWynagrodzeniaViewModel : WszystkieViewModel<WynagrodzenieForAllView>
    {
        private readonly Dictionary<string, Func<WynagrodzenieForAllView, IComparable>> _sortSelectors;
        private readonly Dictionary<string, Func<WynagrodzenieForAllView, string>> _findSelectors;
        private readonly WynagrodzenieRaportB _raportB;
        private List<WynagrodzenieForAllView> _allItems;

        private List<string> _okresy = new List<string>();
        private string _okres;
        private ObservableCollection<WynagrodzenieRaportRow> _rows = new ObservableCollection<WynagrodzenieRaportRow>();
        private decimal _totalKwota;
        private decimal _totalGodzin;

        public WszystkieWynagrodzeniaViewModel()
        {
            DisplayName = "Wynagrodzenia";

            _sortSelectors = new Dictionary<string, Func<WynagrodzenieForAllView, IComparable>>
            {
                { "Lektor", x => (IComparable)(x?.Lektor ?? string.Empty) },
                { "Okres", x => (IComparable)(x?.Okres ?? string.Empty) },
                { "Kwota", x => (IComparable)(x?.KwotaDoWyplaty ?? 0m) },
                { "Liczba godzin", x => (IComparable)(x?.LiczbaGodzin ?? 0m) },
                { "Stawka", x => (IComparable)(x?.StawkaGodzinowa ?? 0m) },
                { "Data wypłaty", x => (IComparable)(x?.DataWyplaty ?? DateTime.MinValue) },
                { "Status", x => (IComparable)(x?.Status ?? string.Empty) },
            };

            _findSelectors = new Dictionary<string, Func<WynagrodzenieForAllView, string>>
            {
                { "Lektor", x => x?.Lektor ?? string.Empty },
                { "Okres", x => x?.Okres ?? string.Empty },
                { "Status", x => x?.Status ?? string.Empty }
            };

            SetSortComboboxItems(_sortSelectors.Keys);
            SortField = _sortSelectors.Keys.FirstOrDefault();

            _raportB = new WynagrodzenieRaportB(db);

            SetFindComboboxItems(_findSelectors.Keys);
            FindField = _findSelectors.Keys.FirstOrDefault();

            LoadOkresy();
        }

        public override void load()
        {
            var data = (
                from w in db.Wynagrodzenie
                select new WynagrodzenieForAllView
                {
                    WynagrodzenieId = w.WynagrodzenieId,
                    Lektor = w.Lektor.Imie + " " + w.Lektor.Nazwisko,
                    Okres = w.Okres,
                    LiczbaGodzin = w.LiczbaGodzin,
                    StawkaGodzinowa = w.StawkaGodzinowa,
                    KwotaDoWyplaty = w.KwotaDoWyplaty,
                    DataWyplaty = w.DataWyplaty,
                    Status = w.Status
                }).ToList();

            _allItems = data;
            List = new ObservableCollection<WynagrodzenieForAllView>(data);
        }

        protected override void Sort()
        {
            if (List == null || string.IsNullOrWhiteSpace(SortField))
                return;

            if (!_sortSelectors.TryGetValue(SortField, out var selector))
                return;

            var ordered = SortDescending
                ? List.OrderByDescending(selector)
                : List.OrderBy(selector);

            List = new ObservableCollection<WynagrodzenieForAllView>(ordered);
            _allItems = List.ToList();
        }

        protected override void Find()
        {
            if (_allItems == null)
                return;

            if (string.IsNullOrWhiteSpace(FindText))
            {
                List = new ObservableCollection<WynagrodzenieForAllView>(_allItems);
                return;
            }

            if (!_findSelectors.TryGetValue(FindField, out var selector))
                return;

            var filter = FindText.Trim();
            var filtered = _allItems.Where(x =>
                (selector(x) ?? string.Empty)
                    .StartsWith(filter, StringComparison.CurrentCultureIgnoreCase));

            List = new ObservableCollection<WynagrodzenieForAllView>(filtered);
        }

        private void LoadOkresy(bool keepCurrent = false)
        {
            var okresy = _raportB.GetOkresy();
            if (!okresy.Any())
                okresy = new List<string> { DateTime.Today.ToString("yyyy-MM") };

            var previous = keepCurrent ? Okres : null;
            Okresy = okresy.ToList();

            if (keepCurrent && !string.IsNullOrWhiteSpace(previous) && Okresy.Contains(previous))
                Okres = previous;
            else
                Okres = Okresy.FirstOrDefault();
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
                _raportB.GenerujWynagrodzeniaZaOkres(Okres);
                LoadOkresy(true);
                load();
                obliczClick();
                ShowMessageBox("Wygenerowano wynagrodzenia.");
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Błąd generowania wynagrodzeń: {ex.Message}");
            }
        }

        private void obliczClick()
        {
            if (string.IsNullOrWhiteSpace(Okres))
            {
                Rows = new ObservableCollection<WynagrodzenieRaportRow>();
                TotalKwota = 0m;
                TotalGodzin = 0m;
                return;
            }

            var raport = _raportB.GetRaport(Okres);
            Rows = new ObservableCollection<WynagrodzenieRaportRow>(raport);
            TotalKwota = raport.Sum(r => r.KwotaDoWyplaty);
            TotalGodzin = raport.Sum(r => r.LiczbaGodzin ?? 0m);
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
                var raport = _raportB.GetRaport(Okres);
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

        public IEnumerable<string> Okresy
        {
            get => _okresy;
            private set
            {
                _okresy = value?.ToList() ?? new List<string>();
                OnPropertyChanged(() => Okresy);
            }
        }

        public string Okres
        {
            get => _okres;
            set
            {
                if (_okres != value)
                {
                    _okres = value;
                    OnPropertyChanged(() => Okres);
                }
            }
        }

        public ObservableCollection<WynagrodzenieRaportRow> Rows
        {
            get => _rows;
            set
            {
                _rows = value ?? new ObservableCollection<WynagrodzenieRaportRow>();
                OnPropertyChanged(() => Rows);
            }
        }

        public decimal TotalKwota
        {
            get => _totalKwota;
            set
            {
                _totalKwota = value;
                OnPropertyChanged(() => TotalKwota);
            }
        }

        public decimal TotalGodzin
        {
            get => _totalGodzin;
            set
            {
                _totalGodzin = value;
                OnPropertyChanged(() => TotalGodzin);
            }
        }

        private ICommand _ObliczCommand;
        public ICommand ObliczCommand
        {
            get
            {
                if (_ObliczCommand == null)
                    _ObliczCommand = new BaseCommand(obliczClick);
                return _ObliczCommand;
            }
        }

        private ICommand _EksportWynagrodzenCommand;
        public ICommand EksportWynagrodzenCommand
        {
            get
            {
                if (_EksportWynagrodzenCommand == null)
                    _EksportWynagrodzenCommand = new BaseCommand(eksportujClick);
                return _EksportWynagrodzenCommand;
            }
        }

        public override bool ShowAddButton => false;

        private ICommand _GenerujWynagrodzeniaCommand;
        public ICommand GenerujWynagrodzeniaCommand
        {
            get
            {
                if (_GenerujWynagrodzeniaCommand == null)
                    _GenerujWynagrodzeniaCommand = new BaseCommand(generujClick);
                return _GenerujWynagrodzeniaCommand;
            }
        }

    }
}
