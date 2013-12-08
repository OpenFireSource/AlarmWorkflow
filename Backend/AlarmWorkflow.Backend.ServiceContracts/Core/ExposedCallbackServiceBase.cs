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
using System.ServiceModel;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Provides the base implementation for a callback service running at the backend and exposed over WCF.
    /// </summary>
    public abstract class ExposedCallbackServiceBase<TCallback> : ExposedServiceBase
    {
        #region Properties

        /// <summary>
        /// Gets the callback that is used for this service.
        /// </summary>
        protected TCallback Callback { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExposedCallbackServiceBase&lt;TCallback&gt;"/> class.
        /// </summary>
        protected ExposedCallbackServiceBase()
            : base()
        {

        }

        #endregion

        #region IExposedService Members

        /// <summary>
        /// Overridden to store the used callback for future use.
        /// </summary>
        public override void Ping()
        {
            this.Callback = OperationContext.Current.GetCallbackChannel<TCallback>();
        }

        #endregion
    }
}
