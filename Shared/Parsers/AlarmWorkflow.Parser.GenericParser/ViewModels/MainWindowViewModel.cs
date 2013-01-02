using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using Microsoft.Win32;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parser definition VM.
        /// </summary>
        public ParserDefinitionViewModel ParserDefinition { get; private set; }

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
                ParserDefinition.Load(dialog.FileName);
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
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = Properties.Resources.Controlfile_FilterText;
            dialog.InitialDirectory = Utilities.GetWorkingDirectory();
            if (dialog.ShowDialog() == true)
            {
                ParserDefinition.Save(dialog.FileName);
            }
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
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            ParserDefinition = new ParserDefinitionViewModel();
        }

        #endregion
    }
}
