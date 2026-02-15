using System;
using System.Windows.Input;
using PollyglotDesktopApp.Helper;

namespace PollyglotDesktopApp.ViewModels
{
    public abstract class WorkspaceViewModel : BaseViewModel
    {
        private ICommand _CloseCommand;

        public WorkspaceViewModel()
        {
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                    _CloseCommand = new BaseCommand(() => OnRequestClose());
                return _CloseCommand;
            }
        }

        public override string DisplayName { get; protected set; }

        public event EventHandler RequestClose;

        protected void OnRequestClose()
        {
            var handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
