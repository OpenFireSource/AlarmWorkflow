using System;
using System.Windows.Input;

namespace AlarmWorkflow.Windows.UI
{
    /// <summary>
    /// Provides an implementation of the ICommand interface which allows for delegated commanding.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// A relay command with only an execute handler.
        /// </summary>
        /// <param name="execute">The method to execute.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {

        }

        /// <summary>
        /// A relay command with an execute and can-execute handler.
        /// </summary>
        /// <param name="execute">The method to execute.</param>
        /// <param name="canExecute">The method that evaluates whether or not this command can be executed.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Whether or not this command can execute.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
        }

        #endregion
    }
}
