using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Helper;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszystkieZajeciaViewModel : WszystkieViewModel<ZajeciaForAllView>
    {
        private readonly Dictionary<string, Func<ZajeciaForAllView, IComparable>> _sortSelectors;
        private readonly Dictionary<string, Func<ZajeciaForAllView, string>> _findSelectors;
        private List<ZajeciaForAllView> _allItems;
        private ZajeciaForAllView _selectedItem;
        private ICommand _filterTodayCommand;
        private ICommand _filterSelectedTeacherCommand;
        private ICommand _clearFiltersCommand;
        private ObservableCollection<string> _selectedGroupMembers = new ObservableCollection<string>();

        public WszystkieZajeciaViewModel()
        {
            DisplayName = "Zajęcia";

            _sortSelectors = new Dictionary<string, Func<ZajeciaForAllView, IComparable>>
            {
                { "Data", x => (IComparable)(x?.Data ?? DateTime.MinValue) },
                { "Start", x => (IComparable)(x?.GodzinaStart ?? TimeSpan.Zero) },
                { "Koniec", x => (IComparable)(x?.GodzinaKoniec ?? TimeSpan.Zero) },
                { "Lektor", x => (IComparable)(x?.Lektor ?? string.Empty) },
                { "Sala", x => (IComparable)(x?.Sala ?? string.Empty) },
                { "Grupa", x => (IComparable)(x?.Grupa ?? string.Empty) },
                { "Temat", x => (IComparable)(x?.Temat ?? string.Empty) },
                { "Uczeń", x => (IComparable)(x?.Uczen ?? string.Empty) },
                { "Tryb", x => (IComparable)(x?.Tryb ?? string.Empty) },
                { "Rodzaj", x => (IComparable)(x?.RodzajZajec ?? string.Empty) }
            };

            _findSelectors = new Dictionary<string, Func<ZajeciaForAllView, string>>
            {
                { "Temat", x => x?.Temat ?? string.Empty },
                { "Lektor", x => x?.Lektor ?? string.Empty },
                { "Uczeń", x => x?.Uczen ?? string.Empty },
                { "Grupa", x => x?.Grupa ?? string.Empty },
                { "Tryb", x => x?.Tryb ?? string.Empty },
                { "Rodzaj", x => x?.RodzajZajec ?? string.Empty },
                { "Sala", x => x?.Sala ?? string.Empty }
            };

            SetSortComboboxItems(_sortSelectors.Keys);
            SortField = _sortSelectors.Keys.FirstOrDefault();

            SetFindComboboxItems(_findSelectors.Keys);
            FindField = _findSelectors.Keys.FirstOrDefault();
        }

        public override void Add()
        {
            Messenger.Default.Send("Wszystkie zajecia Add");
        }

        public override void load()
        {
            var data = db.Zajecia
                .AsEnumerable()
                .Select(z => new ZajeciaForAllView
                {
                    ZajeciaId = z.ZajeciaId,
                    Data = z.Data,
                    GodzinaStart = z.GodzinaStart,
                    GodzinaKoniec = z.GodzinaKoniec,
                    Lektor = z.Lektor != null ? (z.Lektor.Imie ?? string.Empty) + " " + (z.Lektor.Nazwisko ?? string.Empty) : string.Empty,
                    Sala = z.Sala != null ? z.Sala.Nazwa : string.Empty,
                    Grupa = z.Grupa != null ? z.Grupa.Nazwa : string.Empty,
                    GrupaId = z.GrupaId,
                    Temat = z.Temat,
                    Uczen = z.Uczen != null ? (z.Uczen.Imie ?? string.Empty) + " " + (z.Uczen.Nazwisko ?? string.Empty) : string.Empty,
                    Tryb = z.Tryb,
                    DzienTygodnia = z.DzienTygodnia,
                    RodzajZajec = z.RodzajZajec
                })
                .ToList();

            _allItems = data;
            List = new ObservableCollection<ZajeciaForAllView>(data);
        }

        public ZajeciaForAllView SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(IsItemSelected));
                UpdateGroupMembers();
            }
        }

        public bool IsItemSelected => SelectedItem != null;

        public ObservableCollection<string> SelectedGroupMembers
        {
            get => _selectedGroupMembers;
            private set
            {
                if (_selectedGroupMembers == value)
                    return;
                _selectedGroupMembers = value ?? new ObservableCollection<string>();
                OnPropertyChanged(nameof(SelectedGroupMembers));
            }
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

            List = new ObservableCollection<ZajeciaForAllView>(ordered);
        }

        protected override void Find()
        {
            if (_allItems == null)
                return;

            if (string.IsNullOrWhiteSpace(FindText))
            {
                List = new ObservableCollection<ZajeciaForAllView>(_allItems);
                return;
            }

            if (!_findSelectors.TryGetValue(FindField, out var selector))
                return;

            var filter = FindText.Trim();
            var filtered = _allItems.Where(x =>
                (selector(x) ?? string.Empty)
                    .StartsWith(filter, StringComparison.CurrentCultureIgnoreCase));

            List = new ObservableCollection<ZajeciaForAllView>(filtered);
        }

        public ICommand FilterTodayCommand
        {
            get
            {
                if (_filterTodayCommand == null)
                    _filterTodayCommand = new BaseCommand(FilterToday);
                return _filterTodayCommand;
            }
        }

        public ICommand FilterSelectedTeacherCommand
        {
            get
            {
                if (_filterSelectedTeacherCommand == null)
                    _filterSelectedTeacherCommand = new BaseCommand(FilterSelectedTeacher);
                return _filterSelectedTeacherCommand;
            }
        }

        public ICommand ClearFiltersCommand
        {
            get
            {
                if (_clearFiltersCommand == null)
                    _clearFiltersCommand = new BaseCommand(ClearFilters);
                return _clearFiltersCommand;
            }
        }

        private void FilterToday()
        {
            if (_allItems == null)
                return;

            var today = DateTime.Today;
            var filtered = _allItems.Where(x => x.Data?.Date == today);
            List = new ObservableCollection<ZajeciaForAllView>(filtered);
        }

        private void FilterSelectedTeacher()
        {
            if (_allItems == null || SelectedItem == null || string.IsNullOrWhiteSpace(SelectedItem.Lektor))
                return;

            var name = SelectedItem.Lektor;
            var filtered = _allItems.Where(x =>
                string.Equals(x.Lektor, name, StringComparison.OrdinalIgnoreCase));
            List = new ObservableCollection<ZajeciaForAllView>(filtered);
        }

        private void ClearFilters()
        {
            if (_allItems == null)
                return;

            List = new ObservableCollection<ZajeciaForAllView>(_allItems);
        }

        private void UpdateGroupMembers()
        {
            if (SelectedItem == null || SelectedItem.GrupaId == null)
            {
                SelectedGroupMembers = new ObservableCollection<string>();
                return;
            }

            var members = db.GrupaUczen
                .Where(gu => gu.GrupaId == SelectedItem.GrupaId)
                .Select(gu => new
                {
                    imie = gu.Uczen.Imie ?? string.Empty,
                    nazwisko = gu.Uczen.Nazwisko ?? string.Empty
                })
                .AsEnumerable()
                .Select(uczen => $"{uczen.imie} {uczen.nazwisko}".Trim())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            SelectedGroupMembers = new ObservableCollection<string>(members);
        }
    }
}
