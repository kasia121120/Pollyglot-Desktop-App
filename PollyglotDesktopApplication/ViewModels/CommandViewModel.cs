using System;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels
{
    public class CommandViewModel : BaseViewModel
    {
        #region Properties
        public ICommand Command { get; private set; }
        #endregion

        #region Constructor
        public CommandViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            DisplayName = displayName;
            Command = command;
        }
        #endregion
    }
}
