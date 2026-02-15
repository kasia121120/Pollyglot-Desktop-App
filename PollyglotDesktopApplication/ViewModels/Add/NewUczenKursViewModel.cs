using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.Models.ForAllView;
using PollyglotDesktopApp.ViewModels.Abstract;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Add
{
    public class NewUczenKursViewModel : JedenViewModel<UczenKurs>, IDataErrorInfo
    {
        private ICommand _showUczniowieCommand;
        private ICommand _showKursyCommand;
        private ICommand _showJezykiCommand;
        private ICommand _showRodzajeCommand;
        private ICommand _showLektorzyCommand;
        private ICommand _showPodrecznikiCommand;

        private string _uczenDane = "Brak wybranego ucznia";
        private string _kursDane = "Brak wybranego kursu";
        private string _jezykDane = "Brak wybranego języka";
        private string _rodzajDane = "Brak wybranego rodzaju kursu";
        private string _lektorDane = "Brak wybranego lektora";
        private string _podrecznikDane = "Brak wybranego podręcznika";
        private int? _kursJezykId;
        private int? _kursRodzajId;

        public NewUczenKursViewModel()
            : base()
        {
            DisplayName = "Nowe zajęcia indywidualne";
            item = new UczenKurs
            {
                Status = "aktywne"
            };

            Messenger.Default.Register<Uczen>(this, OnUczenSelected);
            Messenger.Default.Register<KursForAllView>(this, OnKursSelected);
            Messenger.Default.Register<Jezyk>(this, OnJezykSelected);
            Messenger.Default.Register<RodzajKursu>(this, OnRodzajSelected);
            Messenger.Default.Register<Lektor>(this, OnLektorSelected);
            Messenger.Default.Register<PodrecznikForAllView>(this, OnPodrecznikSelected);
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

        public ICommand ShowKursyCommand
        {
            get
            {
                if (_showKursyCommand == null)
                    _showKursyCommand = new BaseCommand(() => Messenger.Default.Send("Kursy Show"));
                return _showKursyCommand;
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

        public ICommand ShowRodzajeCommand
        {
            get
            {
                if (_showRodzajeCommand == null)
                    _showRodzajeCommand = new BaseCommand(() => Messenger.Default.Send("RodzajeKursu Show"));
                return _showRodzajeCommand;
            }
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

        public ICommand ShowPodrecznikiCommand
        {
            get
            {
                if (_showPodrecznikiCommand == null)
                    _showPodrecznikiCommand = new BaseCommand(() => Messenger.Default.Send("Podreczniki Show"));
                return _showPodrecznikiCommand;
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

        public string KursDane
        {
            get => _kursDane;
            private set
            {
                if (_kursDane == value)
                    return;
                _kursDane = value;
                OnPropertyChanged(nameof(KursDane));
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

        public string RodzajDane
        {
            get => _rodzajDane;
            private set
            {
                if (_rodzajDane == value)
                    return;
                _rodzajDane = value;
                OnPropertyChanged(nameof(RodzajDane));
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

        public string PodrecznikDane
        {
            get => _podrecznikDane;
            private set
            {
                if (_podrecznikDane == value)
                    return;
                _podrecznikDane = value;
                OnPropertyChanged(nameof(PodrecznikDane));
            }
        }

        public int? UczenId
        {
            get => item.UczenId != 0 ? (int?)item.UczenId : null;
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

        public int? KursId
        {
            get => item.KursId != 0 ? (int?)item.KursId : null;
            private set
            {
                if (value == null)
                {
                    if (item.KursId == 0)
                        return;
                    item.KursId = 0;
                    OnPropertyChanged(nameof(KursId));
                    OnPropertyChanged(nameof(KursError));
                    return;
                }

                if (item.KursId == value.Value)
                    return;

                item.KursId = value.Value;
                OnPropertyChanged(nameof(KursId));
                OnPropertyChanged(nameof(KursError));
            }
        }

        public int? JezykId
        {
            get => item.JezykId != 0 ? (int?)item.JezykId : null;
            private set
            {
                if (value == null)
                {
                    if (item.JezykId == 0)
                        return;
                    item.JezykId = 0;
                    OnPropertyChanged(nameof(JezykId));
                    OnPropertyChanged(nameof(JezykError));
                    return;
                }
                if (item.JezykId == value.Value)
                    return;
                item.JezykId = value.Value;
                OnPropertyChanged(nameof(JezykId));
                OnPropertyChanged(nameof(JezykError));
                OnPropertyChanged(nameof(LektorError));
                OnPropertyChanged(nameof(PodrecznikError));
            }
        }

        public int? RodzajKursuId
        {
            get => item.RodzajKursuId != 0 ? (int?)item.RodzajKursuId : null;
            private set
            {
                if (value == null)
                {
                    if (item.RodzajKursuId == 0)
                        return;
                    item.RodzajKursuId = 0;
                    OnPropertyChanged(nameof(RodzajKursuId));
                    OnPropertyChanged(nameof(RodzajError));
                    return;
                }
                if (item.RodzajKursuId == value.Value)
                    return;
                item.RodzajKursuId = value.Value;
                OnPropertyChanged(nameof(RodzajKursuId));
                OnPropertyChanged(nameof(RodzajError));
            }
        }

        public int? LektorId
        {
            get => item.LektorId != 0 ? (int?)item.LektorId : null;
            private set
            {
                if (value == null)
                {
                    if (item.LektorId == 0)
                        return;
                    item.LektorId = 0;
                    OnPropertyChanged(nameof(LektorId));
                    OnPropertyChanged(nameof(LektorError));
                    return;
                }
                if (item.LektorId == value.Value)
                    return;
                item.LektorId = value.Value;
                OnPropertyChanged(nameof(LektorId));
                OnPropertyChanged(nameof(LektorError));
            }
        }

        public int? PodrecznikId
        {
            get => item.PodrecznikId != 0 ? (int?)item.PodrecznikId : null;
            private set
            {
                if (value == null)
                {
                    if (item.PodrecznikId == 0)
                        return;
                    item.PodrecznikId = 0;
                    OnPropertyChanged(nameof(PodrecznikId));
                    OnPropertyChanged(nameof(PodrecznikError));
                    return;
                }
                if (item.PodrecznikId == value.Value)
                    return;
                item.PodrecznikId = value.Value;
                OnPropertyChanged(nameof(PodrecznikId));
                OnPropertyChanged(nameof(PodrecznikError));
            }
        }

        public override void Save()
        {
            if (!IsValid())
                return;

            item.Status = "aktywne";
            db.UczenKurs.Add(item);
            db.SaveChanges();
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(UczenId):
                        if (!UczenId.HasValue)
                            return "Wybierz ucznia.";
                        return SprawdzCzyUczenMaAktywnyKurs();
                    case nameof(KursId):
                        if (!KursId.HasValue)
                            return "Wybierz kurs.";
                        return SprawdzCzyKursJestGrupowy();
                    case nameof(JezykId):
                        if (!JezykId.HasValue)
                            return "Wybierz język.";
                        return SprawdzCzyJezykZKursemNiePasuje();
                    case nameof(RodzajKursuId):
                        if (!RodzajKursuId.HasValue)
                            return "Wybierz rodzaj kursu.";
                        return SprawdzCzyRodzajZKursemNiePasuje();
                    case nameof(LektorId):
                        if (!LektorId.HasValue)
                            return "Wybierz lektora.";
                        return SprawdzCzyLektorUczyJezyka();
                    case nameof(PodrecznikId):
                        if (!PodrecznikId.HasValue)
                            return "Wybierz podręcznik.";
                        return SprawdzCzyPodrecznikPasujeDoJezyka();
                    default:
                        return null;
                }
            }
        }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(UczenId)])
                && string.IsNullOrEmpty(this[nameof(KursId)])
                && string.IsNullOrEmpty(this[nameof(JezykId)])
                && string.IsNullOrEmpty(this[nameof(RodzajKursuId)])
                && string.IsNullOrEmpty(this[nameof(LektorId)])
                && string.IsNullOrEmpty(this[nameof(PodrecznikId)]);
        }

        public string UczenError => this[nameof(UczenId)];
        public string KursError => this[nameof(KursId)];
        public string JezykError => this[nameof(JezykId)];
        public string RodzajError => this[nameof(RodzajKursuId)];
        public string LektorError => this[nameof(LektorId)];
        public string PodrecznikError => this[nameof(PodrecznikId)];

        private string SprawdzCzyUczenMaAktywnyKurs()
        {
            if (!UczenId.HasValue)
                return null;

            var istnieje = db.UczenKurs.Any(uk => uk.UczenId == UczenId.Value && uk.Status == "Aktywne");
            return istnieje ? "Ten uczeń ma już przypisany kurs." : null;
        }

        private string SprawdzCzyKursJestGrupowy()
        {
            if (string.IsNullOrWhiteSpace(RodzajDane))
                return null;

            var nazwa = RodzajDane.ToLowerInvariant();
            return nazwa.Contains("grup")
                ? "Wybierz kurs indywidualny (nie grupowy)."
                : null;
        }

        private string SprawdzCzyJezykZKursemNiePasuje()
        {
            if (!_kursJezykId.HasValue || !JezykId.HasValue)
                return null;

            return _kursJezykId.Value != JezykId.Value
                ? "Wybrany język nie jest zgodny z kursem."
                : null;
        }

        private string SprawdzCzyRodzajZKursemNiePasuje()
        {
            if (!_kursRodzajId.HasValue || !RodzajKursuId.HasValue)
                return null;

            return _kursRodzajId.Value != RodzajKursuId.Value
                ? "Wybrany rodzaj kursu nie jest zgodny z kursem."
                : null;
        }

        private string SprawdzCzyLektorUczyJezyka()
        {
            if (!LektorId.HasValue || !JezykId.HasValue)
                return null;

            var uczy = db.LektorJezyk.Any(lj => lj.LektorId == LektorId.Value && lj.JezykId == JezykId.Value);
            return uczy ? null : "Wybrany lektor nie uczy tego języka.";
        }

        private string SprawdzCzyPodrecznikPasujeDoJezyka()
        {
            if (!PodrecznikId.HasValue || !JezykId.HasValue)
                return null;

            var podrecznik = db.Podrecznik.Find(PodrecznikId.Value);
            if (podrecznik == null)
                return null;

            return podrecznik.JezykId != JezykId.Value
                ? "Wybrany podręcznik nie pasuje do języka."
                : null;
        }

        private void OnUczenSelected(Uczen uczen)
        {
            if (uczen == null)
                return;
            UczenId = uczen.UczenId;
            UczenDane = $"{uczen.Imie} {uczen.Nazwisko}";
        }

        private void OnKursSelected(KursForAllView kurs)
        {
            if (kurs == null)
                return;
            KursId = kurs.KursId;
            KursDane = $"{kurs.NazwaKursu}";
            _kursJezykId = kurs.JezykId;
            _kursRodzajId = kurs.RodzajKursuId;
            JezykId = kurs.JezykId;
            JezykDane = kurs.Jezyk;
            RodzajKursuId = kurs.RodzajKursuId;
            RodzajDane = kurs.RodzajKursu;
            OnPropertyChanged(nameof(KursError));
            OnPropertyChanged(nameof(JezykError));
            OnPropertyChanged(nameof(RodzajError));
            OnPropertyChanged(nameof(PodrecznikError));
        }

        private void OnJezykSelected(Jezyk jezyk)
        {
            if (jezyk == null)
                return;
            JezykId = jezyk.JezykId;
            JezykDane = jezyk.Nazwa;
        }

        private void OnRodzajSelected(RodzajKursu rodzaj)
        {
            if (rodzaj == null)
                return;
            RodzajKursuId = rodzaj.RodzajKursuId;
            RodzajDane = rodzaj.Nazwa;
        }

        private void OnLektorSelected(Lektor lektor)
        {
            if (lektor == null)
                return;
            LektorId = lektor.LektorId;
            var jezyki = lektor.LektorJezyk?
                .Select(l => l.Jezyk.Nazwa)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();
            var jezykInfo = jezyki != null && jezyki.Any()
                ? $"język: {string.Join(", ", jezyki)}"
                : "język: brak";
            LektorDane = $"{lektor.Imie} {lektor.Nazwisko} ({jezykInfo})";
        }

        private void OnPodrecznikSelected(PodrecznikForAllView podrecznik)
        {
            if (podrecznik == null)
                return;
            PodrecznikId = podrecznik.PodrecznikId;
            PodrecznikDane = podrecznik.Tytul;
        }
    }
}
