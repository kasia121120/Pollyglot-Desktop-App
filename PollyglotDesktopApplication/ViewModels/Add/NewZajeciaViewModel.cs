using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.Models.Validatory;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewZajeciaViewModel : JedenViewModel<Zajecia>, IDataErrorInfo
    {
        private const int EarliestStartHour = 8;
        private const int LatestStartHour = 20;
        private const double DefaultDurationHours = 1.0;

        private string _selectedStartSlot;
        private string _selectedEndSlot;
        private string _rodzajZajec;
        private bool _rodzajAuto;
        private string _grupaLanguageInfo = "Brak przypisanego języka grupy";
        private string _uczenLanguageInfo = "Brak przypisanego języka ucznia";
        private string _podsumowanieGrupy = string.Empty;

        private static readonly string[] ValidationProperties = new[]
        {
            nameof(GrupaId),
            nameof(UczenId),
            nameof(RodzajZajec),
            nameof(Data),
            nameof(DzienTygodnia),
            nameof(Tryb),
            nameof(SalaId),
            nameof(LektorId),
            nameof(Temat),
            nameof(Uwagi)
        };

        public NewZajeciaViewModel()
            : base()
        {
            DisplayName = "Nowe zajecia";
            item = new Zajecia
            {
                Data = DateTime.Today,
                Status = "aktywne"
            };

            StartTimeSlots = Enumerable.Range(EarliestStartHour, LatestStartHour - EarliestStartHour + 1)
                .Select(h => FormatSlot(TimeSpan.FromHours(h)))
                .ToList();

            SelectedStartSlot = StartTimeSlots.First();
            item.Tryb = Tryb;
            _rodzajAuto = true;
            RefreshRodzajZajec();
            UpdateGroupLanguageInfo();
            UpdateStudentLanguageInfo();
        }

        public List<string> StartTimeSlots { get; }
        public IEnumerable<string> AvailableEndTimes
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedStartSlot))
                    return Enumerable.Empty<string>();

                var start = ParseSlot(SelectedStartSlot);
                var options = new[]
                {
                    start.Add(TimeSpan.FromHours(DefaultDurationHours)),
                    start.Add(TimeSpan.FromMinutes(90))
                }
                .Where(ts => ts <= TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(30)))
                .Select(FormatSlot);

                return options;
            }
        }

        public string SelectedStartSlot
        {
            get => _selectedStartSlot;
            set
            {
                if (string.IsNullOrEmpty(value) || string.Equals(_selectedStartSlot, value))
                    return;

                _selectedStartSlot = value;
                item.GodzinaStart = ParseSlot(value);
                OnPropertyChanged(nameof(SelectedStartSlot));
                OnPropertyChanged(nameof(AvailableEndTimes));
                UpdateDefaultEndSlot();
            }
        }

        public string SelectedEndSlot
        {
            get => _selectedEndSlot;
            set
            {
                if (string.IsNullOrEmpty(value) || string.Equals(_selectedEndSlot, value))
                    return;

                _selectedEndSlot = value;
                item.GodzinaKoniec = ParseSlot(value);
                OnPropertyChanged(nameof(SelectedEndSlot));
            }
        }

        public DateTime? Data
        {
            get => item.Data;
            set
            {
                if (item.Data != value)
                {
                    item.Data = value;
                    OnPropertyChanged(nameof(Data));
                }
            }
        }

        public int? LektorId
        {
            get => item.LektorId;
            set
            {
                if (item.LektorId != value)
                {
                    item.LektorId = value;
                    OnPropertyChanged(nameof(LektorId));
                }
            }
        }

        public int? SalaId
        {
            get => item.SalaId;
            set
            {
                if (item.SalaId == value)
                    return;

                item.SalaId = value;
                OnPropertyChanged(nameof(SalaId));
            }
        }

        public int? GrupaId
        {
            get => item.GrupaId;
            set
            {
                if (item.GrupaId == value)
                    return;

                item.GrupaId = value;

                if (value.HasValue && item.UczenId.HasValue)
                {
                    item.UczenId = null;
                    OnPropertyChanged(nameof(UczenId));
                }

                OnPropertyChanged(nameof(GrupaId));
                RefreshRodzajZajec();
                UpdateGroupLanguageInfo();
                AktualizujPodsumowanieGrupy(value);
            }
        }

        public int? UczenId
        {
            get => item.UczenId;
            set
            {
                if (item.UczenId == value)
                    return;

                item.UczenId = value;

                if (value.HasValue && item.GrupaId.HasValue)
                {
                    item.GrupaId = null;
                    OnPropertyChanged(nameof(GrupaId));
                }

                OnPropertyChanged(nameof(UczenId));
                RefreshRodzajZajec();
                UpdateStudentLanguageInfo();
                UpdateGroupLanguageInfo();
                AktualizujPodsumowanieGrupy(null);
            }
        }

        public string Temat
        {
            get => item.Temat;
            set
            {
                if (item.Temat != value)
                {
                    item.Temat = value;
                    OnPropertyChanged(nameof(Temat));
                }
            }
        }

        public string Uwagi
        {
            get => item.Uwagi;
            set
            {
                if (item.Uwagi != value)
                {
                    item.Uwagi = value;
                    OnPropertyChanged(nameof(Uwagi));
                }
            }
        }

        public string DzienTygodnia
        {
            get => item.DzienTygodnia;
            set
            {
                if (item.DzienTygodnia != value)
                {
                    item.DzienTygodnia = value;
                    OnPropertyChanged(nameof(DzienTygodnia));
                }
            }
        }

        public string Tryb
        {
            get => item.Tryb;
            set
            {
                if (item.Tryb == value)
                    return;

                item.Tryb = value;
                OnPropertyChanged(nameof(Tryb));
            }
        }

        public IEnumerable<string> TrybOptions { get; } = new[] { "online", "stacjonarne" };

        public IEnumerable<string> RodzajOptions { get; } = new[] { "grupowe", "indywidualne" };

        public string RodzajZajec
        {
            get => _rodzajZajec;
            set
            {
                if (_rodzajZajec == value)
                    return;

                _rodzajZajec = value;
                item.RodzajZajec = value;
                _rodzajAuto = false;
                OnPropertyChanged(nameof(RodzajZajec));
            }
        }

        public IEnumerable<KeyAndValue<int>> LektorComboBoxItems
            => db.Lektor
                .Select(l => new
                {
                    l.LektorId,
                    l.Imie,
                    l.Nazwisko,
                    Jezyki = l.LektorJezyk.Select(lj => lj.Jezyk.Nazwa)
                })
                .AsEnumerable()
                .Select(l => new KeyAndValue<int>
                {
                    Key = l.LektorId,
                    Value = $"{l.Imie} {l.Nazwisko} ({(l.Jezyki.Any() ? string.Join(", ", l.Jezyki) : "brak języka")})"
                })
                .OrderBy(x => x.Value)
                .ToList();

        public IEnumerable<KeyAndValue<int>> SalaComboBoxItems
            => db.Sala
                .Select(s => new KeyAndValue<int>
                {
                    Key = s.SalaId,
                    Value = s.Nazwa
                })
                .OrderBy(s => s.Value)
                .ToList();

        public IEnumerable<KeyAndValue<int>> GrupaComboBoxItems
            => db.Grupa
                .Select(g => new KeyAndValue<int>
                {
                    Key = g.GrupaId,
                    Value = g.Nazwa
                })
                .OrderBy(g => g.Value)
                .ToList();

        public IEnumerable<KeyAndValue<int>> UczenComboBoxItems
            => db.Uczen
                .Select(u => new KeyAndValue<int>
                {
                    Key = u.UczenId,
                    Value = u.Imie + " " + u.Nazwisko
                })
                .OrderBy(u => u.Value)
                .ToList();

        public string GrupaLanguageInfo
        {
            get => _grupaLanguageInfo;
            private set
            {
                if (_grupaLanguageInfo == value)
                    return;
                _grupaLanguageInfo = value;
                OnPropertyChanged(nameof(GrupaLanguageInfo));
            }
        }

        public string UczenLanguageInfo
        {
            get => _uczenLanguageInfo;
            private set
            {
                if (_uczenLanguageInfo == value)
                    return;
                _uczenLanguageInfo = value;
                OnPropertyChanged(nameof(UczenLanguageInfo));
            }
        }

        public string PodsumowanieGrupy
        {
            get => _podsumowanieGrupy;
            private set
            {
                if (_podsumowanieGrupy == value)
                    return;
                _podsumowanieGrupy = value;
                OnPropertyChanged(nameof(PodsumowanieGrupy));
                OnPropertyChanged(nameof(CzyPokazacPodsumowanieGrupy));
            }
        }

        public bool CzyPokazacPodsumowanieGrupy => !string.IsNullOrWhiteSpace(PodsumowanieGrupy);

        public override void Save()
        {
            item.Tryb = Tryb;
            item.RodzajZajec = RodzajZajec;
            db.Zajecia.Add(item);
            db.SaveChanges();
        }

        #region Validation
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(GrupaId):
                    case nameof(UczenId):
                        return ZajeciaValidator.ValidateGroupOrStudent(GrupaId, UczenId);
                    case nameof(RodzajZajec):
                        return ZajeciaValidator.ValidateRodzajZajec(RodzajZajec, GrupaId, UczenId);
                    case nameof(Data):
                        return ZajeciaValidator.ValidateData(Data);
                    case nameof(DzienTygodnia):
                        return ZajeciaValidator.ValidateDzienTygodnia(DzienTygodnia);
                    case nameof(Tryb):
                        return ZajeciaValidator.ValidateTryb(Tryb);
                    case nameof(SalaId):
                        return ZajeciaValidator.ValidateSala(SalaId, Tryb);
                    case nameof(LektorId):
                        return ZajeciaValidator.ValidateLektor(LektorId);
                    case nameof(Temat):
                        return ZajeciaValidator.ValidateTemat(Temat);
                    case nameof(Uwagi):
                        return ZajeciaValidator.ValidateUwagi(Uwagi);
                    default:
                        return null;
                }
            }
        }

        public override bool IsValid()
        {
            foreach (var property in ValidationProperties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        private static TimeSpan ParseSlot(string slot) => TimeSpan.Parse(slot);

        private static string FormatSlot(TimeSpan slot) => slot.ToString(@"hh\:mm");

        private void RefreshRodzajZajec()
        {
            if (_rodzajAuto)
            {
                var defaultValue = UczenId.HasValue ? "indywidualne" : "grupowe";
                _rodzajZajec = defaultValue;
                item.RodzajZajec = defaultValue;
            }
            else
            {
                item.RodzajZajec = _rodzajZajec;
            }

            OnPropertyChanged(nameof(RodzajZajec));
        }

        private void UpdateGroupLanguageInfo()
        {
            if (!GrupaId.HasValue)
            {
                GrupaLanguageInfo = "Brak przypisanego języka grupy";
                return;
            }

            var jezyk = db.Grupa
                .Where(g => g.GrupaId == GrupaId.Value)
                .Select(g => g.Kurs.Jezyk.Nazwa)
                .FirstOrDefault();

            GrupaLanguageInfo = string.IsNullOrWhiteSpace(jezyk)
                ? "Brak przypisanego języka grupy"
                : $"Język grupy: {jezyk}";
        }

        private void UpdateStudentLanguageInfo()
        {
            if (!UczenId.HasValue)
            {
                UczenLanguageInfo = "Brak przypisanego języka ucznia";
                return;
            }

            var jezyk = db.UczenKurs
                .Where(uk => uk.UczenId == UczenId.Value)
                .Select(uk => uk.Kurs.Jezyk.Nazwa)
                .FirstOrDefault();

            UczenLanguageInfo = string.IsNullOrWhiteSpace(jezyk)
                ? "Brak przypisanego języka ucznia"
                : $"Język ucznia: {jezyk}";
        }

        private void AktualizujPodsumowanieGrupy(int? grupaId)
        {
            if (!grupaId.HasValue)
            {
                PodsumowanieGrupy = string.Empty;
                return;
            }

            var info = db.Grupa
                .Where(g => g.GrupaId == grupaId.Value)
                .Select(g => new
                {
                    g.DzienTygodnia,
                    g.GodzinaStart,
                    g.GodzinaKoniec,
                    LektorImie = g.Lektor != null ? g.Lektor.Imie : null,
                    LektorNazwisko = g.Lektor != null ? g.Lektor.Nazwisko : null
                })
                .FirstOrDefault();

            if (info == null)
            {
                PodsumowanieGrupy = "Brak danych grupy";
                return;
            }

            var lektor = !string.IsNullOrWhiteSpace(info.LektorImie) && !string.IsNullOrWhiteSpace(info.LektorNazwisko)
                ? $"{info.LektorImie} {info.LektorNazwisko}"
                : "brak lektora";
            var dzien = string.IsNullOrWhiteSpace(info.DzienTygodnia) ? "brak dnia" : info.DzienTygodnia;
            var start = info.GodzinaStart.HasValue ? info.GodzinaStart.Value.ToString(@"hh\:mm") : "-";
            var end = info.GodzinaKoniec.HasValue ? info.GodzinaKoniec.Value.ToString(@"hh\:mm") : "-";

            PodsumowanieGrupy = $"Sta\u0142y lektor: {lektor} | {dzien} {start}\u2013{end}";
        }

        private void UpdateDefaultEndSlot()
        {
            var newEndSlot = AvailableEndTimes.FirstOrDefault();
            if (newEndSlot != null)
            {
                _selectedEndSlot = newEndSlot;
                item.GodzinaKoniec = ParseSlot(newEndSlot);
                OnPropertyChanged(nameof(SelectedEndSlot));
            }
        }
    }
}

