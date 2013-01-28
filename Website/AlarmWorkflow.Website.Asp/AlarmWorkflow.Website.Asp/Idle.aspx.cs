using System;
using System.Web.UI;

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
            Page page = this;
            ServiceConnection.Instance.CheckForUpdate(ref page);
            LastUpdate.Text = "Letztes Update: " + DateTime.Now.ToString();
        }

        #endregion
    }
}