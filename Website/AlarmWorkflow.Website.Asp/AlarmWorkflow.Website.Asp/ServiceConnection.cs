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

using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Models;

namespace AlarmWorkflow.Website.Asp
{
    public class ServiceConnection
    {
        #region Singleton

        private static ServiceConnection _instance;

        /// <summary>
        /// Gets the singleton Instance of this type.
        /// </summary>
        public static ServiceConnection Instance
        {
            get { return _instance ?? (_instance = new ServiceConnection()); }
        }

        #endregion

        #region Methods

        internal void CheckForUpdate(ref Page page)
        {
            Operation operation;
            if (!TryGetLatestOperation(out operation, ref page))
            {
                if (page.GetType().BaseType != typeof (Error))
                {
                    RedirectToErrorPage(ref page);
                }
            }
            else
            {
                if (operation == null)
                {
                    if (page.GetType().BaseType != typeof (Idle))
                    {
                        RedirectToNoAlarm(ref page);
                    }
                }
                else
                {
                    if (operation.Id.ToString(CultureInfo.InvariantCulture) == HttpContext.Current.Request["id"])
                    {
                        if (operation.IsAcknowledged)
                        {
                            RedirectToNoAlarm(ref page);
                        }
                    }
                    else
                    {
                        page.Response.Redirect("Default.aspx?id=" + operation.Id);
                    }
                }
            }
        }

        internal bool TryGetLatestOperation(out Operation operation, ref Page page)
        {
            int maxAgeInMinutes = WebsiteConfiguration.Instance.MaxAge;
            bool onlyNonAcknowledged = WebsiteConfiguration.Instance.NonAcknowledgedOnly;
            // For the moment, we are only interested about the latest operation (if any).
            const int limitAmount = 1;

            operation = null;

            try
            {
                using (WrappedService<IAlarmWorkflowServiceInternal> service = InternalServiceProxy.GetServiceInstance())
                {
                    if (service.IsFaulted)
                    {
                        return false;
                    }
                    IList<int> ids = service.Instance.GetOperationIds(maxAgeInMinutes, onlyNonAcknowledged, limitAmount);
                    if (ids.Count > 0)
                    {
                        // Retrieve the operation with full detail to allow us to access the route image
                        OperationItem operationItem = service.Instance.GetOperationById(ids[0]);
                        operation = operationItem.ToOperation();
                    }
                    return true;
                }
            }
            catch (EndpointNotFoundException)
            {
                if (page.GetType().BaseType == typeof (Default) || page.GetType().BaseType == typeof (Idle))
                {
                    RedirectToErrorPage(ref page);
                }
            }
            return false;
        }

        internal void RedirectToNoAlarm(ref Page page)
        {
            page.Response.Redirect("Idle.aspx");
        }

        internal void RedirectToErrorPage(ref Page page)
        {
            page.Response.Redirect("Error.aspx");
        }

        #endregion
    }
}