    using PollyglotDesktopApp.ViewModels.Abstract;
    using PollyglotDesktopApp.Models;

    namespace PollyglotDesktopApp.ViewModels.Add
    {
        public class NewJezykViewModel : JedenViewModel<Jezyk>
        {
            #region Konstruktor
            public NewJezykViewModel()
                : base()
            {
                DisplayName = "Nowy język";
                item = new Jezyk();
            }
            #endregion

            #region Właściwości
            public string Kod
            {
                get => item.Kod;
                set
                {
                    if (item.Kod != value)
                    {
                        item.Kod = value;
                        OnPropertyChanged(nameof(Kod));
                    }
                }
            }

            public string Nazwa
            {
                get => item.Nazwa;
                set
                {
                    if (item.Nazwa != value)
                    {
                        item.Nazwa = value;
                        OnPropertyChanged(nameof(Nazwa));
                    }
                }
            }

            public string Opis
            {
                get => item.Opis;
                set
                {
                    if (item.Opis != value)
                    {
                        item.Opis = value;
                        OnPropertyChanged(nameof(Opis));
                    }
                }
            }
            #endregion

            #region Komendy
            public override void Save()
            {
                db.Jezyk.Add(item);
                db.SaveChanges();
            }
            #endregion
        }
    }
