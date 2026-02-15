using PollyglotDesktopApp.Helper;
using PollyglotDesktopApp.Models;
using System;
using System.Windows.Input;

namespace PollyglotDesktopApp.ViewModels.Abstract
{
    public abstract class JedenViewModel<T> : WorkspaceViewModel
    {
        protected PollyglotDBEntities db;
        protected T item;

        public JedenViewModel()
        {
            db = new PollyglotDBEntities();
        }

        private ICommand _SaveAndCloseCommand;
        public ICommand SaveAndCloseCommand
        {
            get
            {
                if (_SaveAndCloseCommand == null)
                    _SaveAndCloseCommand = new BaseCommand(SaveAndClose);
                return _SaveAndCloseCommand;
            }
        }

        private ICommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new BaseCommand(() => OnRequestClose());
                return _CancelCommand;
            }
        }

        private void SaveAndClose()
        {
            if (!IsValid())
            {
                ShowMessageBoxError("Popraw błędy");
                return;
            }
            Save();
            OnRequestClose(); 
        }
        public virtual bool IsValid()
        {
            return true;
        }

        public abstract void Save();
    }
}
