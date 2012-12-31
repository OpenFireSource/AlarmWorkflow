using System.Windows.Input;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Commands

        #region Command "OpenControlFileCommand"

        /// <summary>
        /// The OpenControlFileCommand command.
        /// </summary>
        public ICommand OpenControlFileCommand { get; private set; }

        private void OpenControlFileCommand_Execute(object parameter)
        {

        }

        #endregion

        #region Command "SaveControlFileCommand"

        /// <summary>
        /// The SaveControlFileCommand command.
        /// </summary>
        public ICommand SaveControlFileCommand { get; private set; }

        private void SaveControlFileCommand_Execute(object parameter)
        {

        }

        #endregion

        #region Command "ExitCommand"

        /// <summary>
        /// The ExitCommand command.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        private void ExitCommand_Execute(object parameter)
        {
            App.Current.Shutdown();
        }

        #endregion

        #endregion
    }
}
