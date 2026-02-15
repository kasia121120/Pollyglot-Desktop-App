using PollyglotDesktopApp.ViewModels.AllTables;
using System.Windows.Controls;
using System.Windows.Input;

namespace PollyglotDesktopApp.Views.AllTables
{
    public partial class WszystkieGrupyView : WszystkieViewBase
    {
        public WszystkieGrupyView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is WszystkieGrupyViewModel vm && vm.SelectedGroup != null)
            {
                vm.SelectGroup(vm.SelectedGroup);
            }
        }
    }
}
