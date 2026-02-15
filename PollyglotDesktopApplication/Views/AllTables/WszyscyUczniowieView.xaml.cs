using PollyglotDesktopApp.Models;
using PollyglotDesktopApp.ViewModels.AllTables;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PollyglotDesktopApp.Views.AllTables
{
    public partial class WszyscyUczniowieView : WszystkieViewBase
    {
        public WszyscyUczniowieView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (typeof(IEnumerable).IsAssignableFrom(e.PropertyType)
                && e.PropertyType != typeof(string))
            {
                e.Cancel = true;
                return;
            }
            if (e.PropertyName == "Status")
            {
                e.Cancel = true;
                return;
            }

            if (e.PropertyName == "DataZapisu")
            {
                e.Column = new DataGridTextColumn
                {
                    Header = "Data zapisu",
                    Binding = new System.Windows.Data.Binding("DataZapisu")
                    {
                        StringFormat = "dd.MM.yyyy"
                    }
                };
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid grid))
                return;

            if (!(DataContext is WszyscyUczniowieViewModel vm))
                return;

            var source = e.OriginalSource as DependencyObject;
            var row = ItemsControl.ContainerFromElement(grid, source) as DataGridRow ?? grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
            if (row?.Item is Uczen uczen)
            {
                vm.SelectUczen(uczen);
            }
        }
    }
}
