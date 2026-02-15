using PollyglotDesktopApp.ViewModels.AllTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PollyglotDesktopApp.Views.AllTables
{
    /// <summary>
    /// Interaction logic for WszystkieSaleView.xaml
    /// </summary>
    public partial class WszystkieSaleView : WszystkieViewBase
    {
        public WszystkieSaleView()
        {
            InitializeComponent();
            DataContext = new WszystkieSaleViewModel();
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