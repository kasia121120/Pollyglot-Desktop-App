using System.Windows;
using PollyglotDesktopApp.ViewModels;

namespace PollyglotDesktopApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(); 
        }
    }
}
