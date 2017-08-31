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

using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.PrintingContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// View model for the system printer dialog
    /// </summary>
    public class SystemPrintingQueuesViewModel : ViewModelBase
    {
        #region Fields

        private bool _close;

        #endregion

        #region Properties 

        public bool Ok { get; set; }

        /// <summary>
        /// Gets or sets the Close Property. Causes closing the dialog
        /// </summary>
        public bool Close
        {
            get { return _close; }
            set
            {
                if (_close == value)
                    return;
                _close = value;
                OnPropertyChanged("Close");
            }
        }

        /// <summary>
        /// Gets or sets the system printers
        /// </summary>
        public ObservableCollection<string> Printers { get; set; }

        /// <summary>
        /// Gets or sets the selection
        /// </summary>
        public string Selection { get; set; }

        #endregion

        #region Commands

        public ICommand CloseCommand { get; set; }

        private void CloseCommand_Execute(object parameter)
        {
            Ok = (bool) parameter;
            Close = true;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPrintingQueuesViewModel"/> class.
        /// </summary>
        public SystemPrintingQueuesViewModel()
        {
            using (var printingService = ServiceFactory.GetServiceWrapper<IPrintingService>())
            {
                Printers = new ObservableCollection<string>(printingService.Instance.GetPrinters());
            }
        }
    }
}
