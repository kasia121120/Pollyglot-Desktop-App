using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using PollyglotDesktopApp.ViewModels.Abstract;
using PollyglotDesktopApp.ViewModels.AllTables;
using PollyglotDesktopApp.ViewModels.Add;
using PollyglotDesktopApp.ViewModels.Raporty;
using PollyglotDesktopApp.Helper;
using GalaSoft.MvvmLight.Messaging;

namespace PollyglotDesktopApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields
        private ReadOnlyCollection<CommandViewModel> _Commands;
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        private WorkspaceViewModel _currentWorkspace;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            Messenger.Default.Register<string>(this, HandleAddRequest);
            ShowAll(new WszyscyUczniowieViewModel());
        }
        #endregion


        #region Commands
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_Commands == null)
                {
                    var cmds = CreateCommands();
                    _Commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _Commands;
            }
        }

        private List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>()
            {

                new CommandViewModel("Języki", new BaseCommand(
                    () => ShowAll(new WszystkieJezykiViewModel()))),

                new CommandViewModel("Lektorzy", new BaseCommand(
                    () => ShowAll(new WszyscyLektorzyViewModel()))),
                new CommandViewModel("Uczniowie", new BaseCommand(
                    () => ShowAll(new WszyscyUczniowieViewModel()))),
                new CommandViewModel("Sale", new BaseCommand(
                    () => ShowAll(new WszystkieSaleViewModel()))),

                new CommandViewModel("Rodzaje kursu", new BaseCommand(
                    () => ShowAll(new WszystkieRodzajeKursuViewModel()))),

                new CommandViewModel("Zajęcia", new BaseCommand(
                    () => ShowAll(new WszystkieZajeciaViewModel()))),


                new CommandViewModel("Kursy", new BaseCommand(
                    () => ShowAll(new WszystkieKursyViewModel()))),

                new CommandViewModel("Grupy - lista uczniów", new BaseCommand(
                    () => ShowAll(new WszystkieGrupaUczenViewModel()))),

                new CommandViewModel("Kompetencje lektorów", new BaseCommand(
                    () => ShowAll(new WszystkieLektorJezykViewModel()))),

                new CommandViewModel("Obecności", new BaseCommand(
                    () => ShowAll(new WszystkieObecnosciViewModel()))),

                new CommandViewModel("Płatności", new BaseCommand(
                    () => ShowAll(new WszystkiePlatnosciViewModel()))),

                new CommandViewModel("Podręczniki", new BaseCommand(
                    () => ShowAll(new WszystkiePodrecznikiViewModel()))),

                new CommandViewModel("Uczniowie - zajęcia indywidualne", new BaseCommand(
                    () => ShowAll(new WszystkieUczenKursViewModel()))),

                new CommandViewModel("Wynagrodzenia", new BaseCommand(
                    () => ShowAll(new WszystkieWynagrodzeniaViewModel()))),

                new CommandViewModel("Grupy", new BaseCommand(
                    () => ShowAll(new WszystkieGrupyViewModel()))),

                new CommandViewModel("Oceny", new BaseCommand(
                    () => ShowAll(new WszystkieOcenyViewModel()))),

                new CommandViewModel("Raport płatności", new BaseCommand(
                    () => ShowAll(new RaportPlatnosciViewModel()))),

                new CommandViewModel("Raport frekwencji", new BaseCommand(
                    () => ShowAll(new RaportFrekwencjiViewModel()))),

                new CommandViewModel("Raport wynagrodzeń", new BaseCommand(
                    () => ShowAll(new RaportWynagrodzenViewModel()))),

            };
        }
        #endregion

        #region Workspaces
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_Workspaces == null)
                {
                    _Workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _Workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _Workspaces;
            }
        }

        public WorkspaceViewModel CurrentWorkspace
        {
            get => _currentWorkspace;
            set
            {
                _currentWorkspace = value;
                OnPropertyChanged(nameof(CurrentWorkspace));
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (WorkspaceViewModel ws in e.NewItems)
                    ws.RequestClose += OnWorkspaceRequestClose;

            if (e.OldItems != null)
                foreach (WorkspaceViewModel ws in e.OldItems)
                    ws.RequestClose -= OnWorkspaceRequestClose;
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            if (sender is WorkspaceViewModel ws)
            {
                Workspaces.Remove(ws);
                CurrentWorkspace = Workspaces.LastOrDefault();
            }
        }
        #endregion

        #region Helpers
        public void CreateView(WorkspaceViewModel workspace)
        {
            CurrentWorkspace = GetOrAddWorkspace(workspace);
        }

        private void ShowAll(WorkspaceViewModel workspace)
        {
            CurrentWorkspace = GetOrAddWorkspace(workspace);
        }

        private WorkspaceViewModel GetOrAddWorkspace(WorkspaceViewModel workspace)
        {
            var existing = Workspaces.FirstOrDefault(w => w.GetType() == workspace.GetType());

            if (existing == null)
            {
                Workspaces.Add(workspace);
                existing = workspace;
            }
            return existing;
        }

        private void HandleAddRequest(string name)
        {
            switch (name)
            {
                case "Wszystkie języki Add":
                    CreateView(new NewJezykViewModel());
                    break;
                case "Wszystkie sale Add":
                    CreateView(new NewSalaViewModel());
                    break;
                case "Wszystkie rodzaje kursu Add":
                    CreateView(new NewRodzajKursuViewModel());
                    break;
                case "Wszyscy lektorzy Add":
                    CreateView(new NewLektorViewModel());
                    break;
                case "Wszyscy uczniowie Add":
                    CreateView(new NewUczenViewModel());
                    break;
                case "Wszystkie zajecia Add":
                    CreateView(new NewZajeciaViewModel());
                    break;
                case "GrupaUczen Add":
                    CreateView(new NewGrupaUczenViewModel());
                    break;
                case "UczenKurs Add":
                    CreateView(new NewUczenKursViewModel());
                    break;
                case "Kompetencje lektorów Add":
                    CreateView(new NewLektorJezykViewModel());
                    break;
                case "Grupy Show":
                    ShowAll(new WszystkieGrupyViewModel());
                    break;
                case "Uczniowie Show":
                    ShowAll(new WszyscyUczniowieViewModel());
                    break;
                case "Kursy Show":
                    ShowAll(new WszystkieKursyViewModel());
                    break;
                case "Jezyki Show":
                    ShowAll(new WszystkieJezykiViewModel());
                    break;
                case "RodzajeKursu Show":
                    ShowAll(new WszystkieRodzajeKursuViewModel());
                    break;
                case "Lektorzy Show":
                    ShowAll(new WszyscyLektorzyViewModel());
                    break;
                case "Podreczniki Show":
                    ShowAll(new WszystkiePodrecznikiViewModel());
                    break;
            }
        }
        #endregion
    }
}
