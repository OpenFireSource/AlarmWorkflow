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

using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.System.Data;
using AlarmWorkflow.BackendService.SystemContracts;

namespace AlarmWorkflow.BackendService.System
{
    class SystemServiceInternal : InternalServiceBase, ISystemServiceInternal
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemServiceInternal"/> class.
        /// </summary>
        public SystemServiceInternal()
            : base()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to ensure that a connection to the database can be established.
        /// Waits as long as a connection becomes available so that further flow can execute as intended.
        /// </summary>
        protected override void InitializeOverride()
        {
            base.InitializeOverride();

            SystemEntities.EnsureDatabaseReachable();
        }

        #endregion
    }
}
