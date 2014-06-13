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

using System.Diagnostics;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    [DebuggerDisplay("Op.: Id = {Operation.Id}, Timestamp/I = {Operation.TimestampIncome}, Timestamp/A = {Operation.Timestamp}")]
    class OperationViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the operation that this VM is based on.
        /// </summary>
        public Operation Operation { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationViewModel"/> class.
        /// </summary>
        public OperationViewModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationViewModel"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationViewModel(Operation operation)
            : this()
        {
            this.Operation = operation;
        }

        #endregion

    }
}