using System.Windows.Input;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Fields

        private ControlInformation _controlInformation;

        #endregion

        #region Properties

        public string Keywords { get; set; }

        public ControlInformation ControlInformation
        {
            get { return _controlInformation; }
            set
            {
                _controlInformation = value;
                OnPropertyChanged("ControlInformation");
            }
        }

        #endregion

        #region Commands

        #region Command "OpenControlFileCommand"

        /// <summary>
        /// The OpenControlFileCommand command.
        /// </summary>
        public ICommand OpenControlFileCommand { get; private set; }

        private void OpenControlFileCommand_Execute(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Properties.Resources.Controlfile_FilterText;
            dialog.InitialDirectory = Utilities.GetWorkingDirectory();
            if (dialog.ShowDialog() == true)
            {
                ControlInformation = ControlInformation.Load(dialog.FileName);
            }
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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {

        }

        #endregion
    }
}
