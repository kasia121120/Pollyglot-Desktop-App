using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PollyglotDesktopApp.ViewModels.AllTables
{
    public class WszyscyUczniowieViewModel : WszystkieViewModel<Uczen>
    {
        private readonly Dictionary<string, Func<Uczen, IComparable>> _sortSelectors;
        private readonly Dictionary<string, Func<Uczen, string>> _findSelectors;
        private List<Uczen> _allItems;
        private Uczen _selectedUczen;
        private ObservableCollection<UczenKursForAllView> _selectedUczenKursy = new ObservableCollection<UczenKursForAllView>();
        private ObservableCollection<PlatnoscForAllView> _selectedPlatnosci = new ObservableCollection<PlatnoscForAllView>();
        private ObservableCollection<ObecnoscForAllView> _selectedObecnosci = new ObservableCollection<ObecnoscForAllView>();

        public WszyscyUczniowieViewModel()
        {
            DisplayName = "Wszyscy uczniowie";

            _sortSelectors = new Dictionary<string, Func<Uczen, IComparable>>
            {
                { "Nazwisko", x => (IComparable)(x?.Nazwisko ?? string.Empty) },
                { "Imię", x => (IComparable)(x?.Imie ?? string.Empty) },
                { "Data zapisu", x => (IComparable)(x?.DataZapisu ?? DateTime.MinValue) },
                { "Email", x => (IComparable)(x?.Email ?? string.Empty) }
            };

            _findSelectors = new Dictionary<string, Func<Uczen, string>>
            {
                { "Nazwisko", x => x?.Nazwisko ?? string.Empty },
                { "Imię", x => x?.Imie ?? string.Empty },
                { "Email", x => x?.Email ?? string.Empty },
                { "Telefon", x => x?.Telefon ?? string.Empty },
                { "PESEL", x => x?.PESEL ?? string.Empty }
            };

            SetSortComboboxItems(_sortSelectors.Keys);
            SortField = _sortSelectors.Keys.FirstOrDefault();
            SetFindComboboxItems(_findSelectors.Keys);
            FindField = _findSelectors.Keys.FirstOrDefault();
        }

        public override void load()
        {
            _allItems = db.Uczen.ToList();
            List = new ObservableCollection<Uczen>(_allItems);
            SelectedUczen = null;
        }

        public void SelectUczen(Uczen uczen)
        {
            if (uczen == null)
                return;

            Messenger.Default.Send(uczen);
            OnRequestClose();
        }

        protected override void Sort()
        {
            if (_allItems == null || string.IsNullOrWhiteSpace(SortField))
                return;

            if (!_sortSelectors.TryGetValue(SortField, out var selector))
                return;

            var ordered = SortDescending
                ? _allItems.OrderByDescending(selector)
                : _allItems.OrderBy(selector);

            List = new ObservableCollection<Uczen>(ordered);
            _allItems = List.ToList();
            SelectedUczen = null;
        }

        protected override void Find()
        {
            load();

            if (string.IsNullOrWhiteSpace(FindText))
                return;

            if (!_findSelectors.TryGetValue(FindField, out var selector))
                return;

            var filter = FindText.Trim();
            var filtered = _allItems.Where(u =>
                (selector(u) ?? string.Empty)
                    .StartsWith(filter, StringComparison.OrdinalIgnoreCase));

            List = new ObservableCollection<Uczen>(filtered);
            SelectedUczen = null;
        }

        public Uczen SelectedUczen
        {
            get => _selectedUczen;
            set
            {
                if (Equals(_selectedUczen, value))
                    return;
                _selectedUczen = value;
                OnPropertyChanged(nameof(SelectedUczen));
                OnPropertyChanged(nameof(IsUczenSelected));
                RefreshUczenDetails();
            }
        }

        public bool IsUczenSelected => SelectedUczen != null;

        public ObservableCollection<UczenKursForAllView> SelectedUczenKursy
        {
            get => _selectedUczenKursy;
            private set
            {
                _selectedUczenKursy = value;
                OnPropertyChanged(nameof(SelectedUczenKursy));
            }
        }

        public ObservableCollection<PlatnoscForAllView> SelectedPlatnosci
        {
            get => _selectedPlatnosci;
            private set
            {
                _selectedPlatnosci = value;
                OnPropertyChanged(nameof(SelectedPlatnosci));
            }
        }

        public ObservableCollection<ObecnoscForAllView> SelectedObecnosci
        {
            get => _selectedObecnosci;
            private set
            {
                _selectedObecnosci = value;
                OnPropertyChanged(nameof(SelectedObecnosci));
            }
        }


        private void RefreshUczenDetails()
        {
            if (SelectedUczen == null)
            {
                SelectedUczenKursy = new ObservableCollection<UczenKursForAllView>();
                SelectedPlatnosci = new ObservableCollection<PlatnoscForAllView>();
                SelectedObecnosci = new ObservableCollection<ObecnoscForAllView>();
                return;
            }

            var kursy = db.UczenKurs
                .Where(uk => uk.UczenId == SelectedUczen.UczenId)
                .Select(uk => new
                {
                    UkUczenImie = uk.Uczen.Imie,
                    UkUczenNazwisko = uk.Uczen.Nazwisko,
                    KursNazwa = uk.Kurs.NazwaKursu,
                    JezykNazwa = uk.Jezyk.Nazwa,
                    RodzajNazwa = uk.RodzajKursu.Nazwa,
                    LektorImie = uk.Lektor.Imie,
                    LektorNazwisko = uk.Lektor.Nazwisko,
                    PodrecznikTytul = uk.Podrecznik.Tytul,
                    uk.Status
                })
                .ToList();

            SelectedUczenKursy = new ObservableCollection<UczenKursForAllView>(
                kursy.Select(uk => new UczenKursForAllView
                {
                    Uczen = $"{uk.UkUczenImie} {uk.UkUczenNazwisko}".Trim(),
                    Kurs = uk.KursNazwa,
                    Jezyk = uk.JezykNazwa,
                    RodzajKursu = uk.RodzajNazwa,
                    Lektor = $"{uk.LektorImie} {uk.LektorNazwisko}".Trim(),
                    Podrecznik = uk.PodrecznikTytul,
                    Status = uk.Status
                }));

            var platnosci = db.Platnosc
                .Where(p => p.UczenId == SelectedUczen.UczenId)
                .OrderByDescending(p => p.DataPlatnosci ?? DateTime.MinValue)
                .Take(5)
                .Select(p => new
                {
                    p.PlatnoscId,
                    UczenImie = p.Uczen.Imie,
                    UczenNazwisko = p.Uczen.Nazwisko,
                    KursNazwa = p.Kurs.NazwaKursu,
                    p.Okres,
                    p.Kwota,
                    p.DataPlatnosci,
                    p.Metoda,
                    p.Status
                })
                .ToList();

            SelectedPlatnosci = new ObservableCollection<PlatnoscForAllView>(
                platnosci.Select(p => new PlatnoscForAllView
                {
                    PlatnoscId = p.PlatnoscId,
                    Uczen = $"{p.UczenImie} {p.UczenNazwisko}".Trim(),
                    Kurs = p.KursNazwa,
                    Okres = p.Okres,
                    Kwota = p.Kwota,
                    DataPlatnosci = p.DataPlatnosci,
                    Metoda = p.Metoda,
                    Status = p.Status
                }));

            var obecnosci = db.Obecnosc
                .Where(o => o.UczenId == SelectedUczen.UczenId)
                .OrderByDescending(o => o.ObecnoscId)
                .Take(5)
                .Select(o => new
                {
                    o.ObecnoscId,
                    ZajeciaTemat = o.Zajecia != null ? o.Zajecia.Temat : string.Empty,
                    ZajeciaData = o.Zajecia != null ? o.Zajecia.Data : (DateTime?)null,
                    ZajeciaDzien = o.Zajecia != null ? o.Zajecia.DzienTygodnia : string.Empty,
                    o.Status,
                    o.Uwagi,
                    o.RecordStatus
                })
                .ToList();

            SelectedObecnosci = new ObservableCollection<ObecnoscForAllView>(
                obecnosci.Select(o => new ObecnoscForAllView
                {
                    ObecnoscId = o.ObecnoscId,
                    Zajecia = $"{(o.ZajeciaData.HasValue ? o.ZajeciaData.Value.ToString("dd.MM.yyyy") : string.Empty)} {o.ZajeciaTemat}".Trim(),
                    Uczen = $"{SelectedUczen.Imie} {SelectedUczen.Nazwisko}".Trim(),
                    Status = o.Status,
                    Uwagi = o.Uwagi,
                    RecordStatus = o.RecordStatus
                }));
        }
                
        public override void Add()
        {
            Messenger.Default.Send("Wszyscy uczniowie Add");
        }
        }
    }

 
