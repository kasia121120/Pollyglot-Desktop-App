using PollyglotDesktopApp.ViewModels.AllTables;
using PollyglotDesktopApp.Views;
using System.Collections;
using System.Windows.Controls;

namespace PollyglotDesktopApp.Views.AllTables
{
    public partial class WszystkieJezykiView : WszystkieViewBase
    {
        public WszystkieJezykiView()
        {
            InitializeComponent();
        }
        // Usuwa kolumny typu ICollection<> z DataGrid
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (typeof(IEnumerable).IsAssignableFrom(e.PropertyType)
                && e.PropertyType != typeof(string))
            {
                e.Cancel = true;
            }
        }
    }
}
