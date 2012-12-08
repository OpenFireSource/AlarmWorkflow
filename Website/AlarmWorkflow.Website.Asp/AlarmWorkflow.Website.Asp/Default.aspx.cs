using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Models;

namespace AlarmWorkflow.Website.Asp
{
    /// <summary>
    /// Logic of the _Default-page.
    /// </summary>
    public partial class _Default : System.Web.UI.Page
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="_Default"/> class.
        /// </summary>
        public _Default()
        {
        }

        #endregion

        #region Methods

        private void CheckAndDisplayLatestAlarm()
        {
            DateStampLabel.Text = DateTime.Now.ToString();

            // Access internal service to see if we have new alarms
            Operation operation = null;
            if (!TryGetLatestOperation(out operation))
            {
                SetContentToConnectionError();
            }
            else
            {
                if (operation == null)
                {
                    SetContentToNoAlarm();
                }
                else
                {
                    SetContentToLatestAlarm(operation);
                }
            }
        }

        private bool TryGetLatestOperation(out Operation operation)
        {
            // TODO: We may read the values from the settings?
            const int maxAgeInMinutes = 800 * 60;
            const bool onlyNonAcknowledged = false;
            // For the moment, we are only interested about the latest operation (if any).
            const int limitAmount = 1;

            operation = null;

            try
            {
                using (var service = InternalServiceProxy.GetServiceInstance())
                {
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
                // We can ignore this exception. It usually occurs if the service is just starting up.
                // TODO: But we may show this information in a red label on the website, still?
            }
            return false;
        }

        private void SetContentToConnectionError()
        {
            // TODO: Find a more elegant way than just switching the panels' visibility!
            pnlProgress.Visible = true;
            pnlAlarm.Visible = false;
            pnlNoAlarm.Visible = false;

            lblProgressText.Text = "Verbindungsfehler! Server nicht erreichbar!";
            lblProgressText.ForeColor = System.Drawing.Color.Red;
        }

        private void SetContentToNoAlarm()
        {
            // TODO: Find a more elegant way than just switching the panels' visibility!
            pnlProgress.Visible = false;
            pnlAlarm.Visible = false;
            pnlNoAlarm.Visible = true;
        }

        private void SetContentToLatestAlarm(Operation operation)
        {
            // TODO: Find a more elegant way than just switching the panels' visibility!
            pnlProgress.Visible = false;
            pnlAlarm.Visible = true;
            pnlNoAlarm.Visible = false;

            // TODO
            tcOperationNumber.Text = operation.OperationNumber;
            tcTimestamp.Text = operation.Timestamp.ToLocalTime().ToString();
            tcDestinationLocation.Text = operation.GetDestinationLocation().ToString();
            tcKeyword.Text = operation.Keyword;
            tcMessenger.Text = operation.Messenger;
            tcComment.Text = operation.Comment;

            LoadOperationRouteImage(operation);
        }

        private void LoadOperationRouteImage(Operation operation)
        {
            string fileUrl = string.Format("~/Cache/RouteImages/{0}.png", Regex.Replace(operation.OperationNumber, "[^a-zA-Z0-9]", "").ToString());
            string filePath = MapPath(fileUrl);

            if (!File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write the route image
                if (operation.RouteImage != null && operation.RouteImage.Length > 0)
                {
                    File.WriteAllBytes(filePath, operation.RouteImage);
                }
                else
                {
                    // Write empty file to designate that no image is available
                    File.WriteAllText(filePath, "");
                }
            }

            // If it is a zero-sized file, then no route image existed (see above).
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length == 0L)
            {
                return;
            }

            this.imgRouteImage.ImageUrl = fileUrl;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // This page should not feature any postbacks (like the result from the user clicking on a link, button or such).
            if (this.IsPostBack)
            {
                return;
            }

            CheckAndDisplayLatestAlarm();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the Tick event of the UpdateTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            CheckAndDisplayLatestAlarm();
        }

        #endregion
    }
}
