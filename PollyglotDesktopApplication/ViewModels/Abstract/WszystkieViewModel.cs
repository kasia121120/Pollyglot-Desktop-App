using GalaSoft.MvvmLight.Messaging;
using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.Add;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Abstract
{
    public abstract class WszystkieViewModel<T> : WorkspaceViewModel
    {
        protected readonly PollyglotDBEntities db;

        private ObservableCollection<T> _List;
        public ObservableCollection<T> List
        {
            get
            {
                if (_List == null) load();
                return _List;
            }
            set
            {
                _List = value;
                OnPropertyChanged(nameof(List));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand FindCommand { get; }
        public ICommand SortAscendingCommand { get; }
        public ICommand SortDescendingCommand { get; }

        protected readonly ObservableCollection<string> _sortComboboxItems = new ObservableCollection<string>();
        protected readonly ObservableCollection<string> _findComboboxItems = new ObservableCollection<string>();
        private string _sortField;
        private string _findField;
        private string _findText;
        private bool _sortDescending;
        private bool _showSortFilter = true;

        public WszystkieViewModel()
        {
            db = new PollyglotDBEntities();

            LoadCommand = new BaseCommand(load);
            AddCommand = new BaseCommand(Add);
            EditCommand = new BaseCommand(Edit);
            DeleteCommand = new BaseCommand(Delete);
            RefreshCommand = new BaseCommand(load);
            SortCommand = new BaseCommand(Sort);
            FindCommand = new BaseCommand(Find);
            SortAscendingCommand = new BaseCommand(() => ExecuteSort(false));
            SortDescendingCommand = new BaseCommand(() => ExecuteSort(true));
        }

        public abstract void load();

        public ObservableCollection<string> SortComboboxItems => _sortComboboxItems;

        public ObservableCollection<string> FindComboboxItems => _findComboboxItems;

        public string SortField
        {
            get => _sortField;
            set
            {
                _sortField = value;
                OnPropertyChanged(nameof(SortField));
            }
        }

        public string FindField
        {
            get => _findField;
            set
            {
                _findField = value;
                OnPropertyChanged(nameof(FindField));
            }
        }

        public string FindText
        {
            get => _findText;
            set
            {
                _findText = value;
                OnPropertyChanged(nameof(FindText));
            }
        }

        public bool SortDescending
        {
            get => _sortDescending;
            set
            {
                _sortDescending = value;
                OnPropertyChanged(nameof(SortDescending));
                OnPropertyChanged(nameof(SortDirectionText));
            }
        }

        public string SortDirectionText => SortDescending ? "Malejąco" : "Rosnąco";

        public bool ShowSortFilter
        {
            get => _showSortFilter;
            set
            {
                if (_showSortFilter == value)
                    return;
                _showSortFilter = value;
                OnPropertyChanged(nameof(ShowSortFilter));
            }
        }

        protected virtual void Sort() { }

        protected virtual void Find() { }

        protected void ExecuteSort(bool descending)
        {
            SortDescending = descending;
            Sort();
        }

        protected void SetSortComboboxItems(IEnumerable<string> values)
        {
            _sortComboboxItems.Clear();
            if (values == null)
                return;
            foreach (var value in values)
                _sortComboboxItems.Add(value);
        }

        protected void SetFindComboboxItems(IEnumerable<string> values)
        {
            _findComboboxItems.Clear();
            if (values == null)
                return;
            foreach (var value in values)
                _findComboboxItems.Add(value);
        }

        public virtual bool ShowAddButton => true;

        public virtual void Add() => Messenger.Default.Send($"{DisplayName} Add");
        public virtual void Edit() { }
        public virtual void Delete() { }
    }
}
