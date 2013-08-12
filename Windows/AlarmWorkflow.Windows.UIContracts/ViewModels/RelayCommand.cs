// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows.Input;

namespace AlarmWorkflow.Windows.UIContracts.ViewModels
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