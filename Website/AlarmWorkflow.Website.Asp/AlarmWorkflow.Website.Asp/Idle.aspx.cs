using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Models;

namespace AlarmWorkflow.Website.Asp
{
    public partial class Idle : Page
    {
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            _UpdateTimer.Interval = WebsiteConfiguration.Instance.UpdateIntervall;
            LastUpdate.Text = "Letztes Update: " + DateTime.Now.ToString();
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            CheckForUpdate();
            LastUpdate.Text = "Letztes Update: " + DateTime.Now.ToString();
        }

        #endregion

        #region Methods

        private bool TryGetLatestOperation(out Operation operation)
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
                        OperationItem operationItem = service.Instance.GetOperationById(ids[0], OperationItemDetailLevel.Full);
                        operation = operationItem.ToOperation();
                    }
                    return true;
                }
            }
            catch (EndpointNotFoundException)
            {
                RedirectErrorPage();
            }
            return false;
        }

        private void CheckForUpdate()
        {
            Operation operation;
            if (!TryGetLatestOperation(out operation))
            {
                RedirectErrorPage();
            }
            else
            {
                if (operation == null)
                {
                    //Do nothing :=)
                }
                else
                {
                    Response.Redirect("Default.aspx?id=" + operation.Id);
                }
            }
        }

        private void RedirectErrorPage()
        {
            Response.Redirect("Error.aspx");
        }

        #endregion
    }
}