using PollyglotDesktopApp.ViewModels.AllTables;
using System.Windows;

namespace PollyglotDesktopApp.Views.AllTables
{
    public partial class WszystkieZajeciaView : WszystkieViewBase
    {
        public WszystkieZajeciaView()
        {
            InitializeComponent();
            DataContext = new WszystkieZajeciaViewModel();  
        }
    }
}
