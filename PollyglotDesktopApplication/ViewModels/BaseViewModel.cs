using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using PollyglotDesktopApp.Helper;

namespace PollyglotDesktopApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region DisplayName
        public virtual string DisplayName { get; protected set; }
        #endregion


        #region Window Commands

        public ICommand Close => new BaseCommand(CloseApplication);

        public ICommand Maximize => new BaseCommand(MaximizeApplication);

        public ICommand Minimize => new BaseCommand(MinimizeApplication);

        public ICommand DragMove => new BaseCommand(DragMoveCommand);

        public ICommand Restart => new BaseCommand(RestartCommand);


        private static void RestartCommand()
        {
            Application.Current.Shutdown();
        }

        private static void DragMoveCommand()
        {
            Application.Current.MainWindow.DragMove();
        }

        private static void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        private static void MaximizeApplication()
        {
            var w = Application.Current.MainWindow;

            if (w.WindowState == WindowState.Maximized)
                w.WindowState = WindowState.Normal;
            else
                w.WindowState = WindowState.Maximized;
        }

        private static void MinimizeApplication()
        {
            var w = Application.Current.MainWindow;

            if (w.WindowState == WindowState.Minimized)
            {
                w.Opacity = 1;
                w.WindowState = WindowState.Normal;
            }
            else
            {
                w.Opacity = 0;
                w.WindowState = WindowState.Minimized;
            }
        }

        #endregion


        #region Message Box

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowMessageBoxError(string message)
        {
            MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion


        #region PropertyChanged

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var name = GetPropertyName(propertyExpression);
            OnPropertyChanged(name);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression)?.Member?.Name
                   ?? throw new InvalidOperationException("Invalid property expression");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
