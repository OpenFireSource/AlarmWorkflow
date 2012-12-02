using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlarmWorkflow.Tools.PluginManager
{
    public partial class PluginSteuerelement : UserControl
    {
        #region Constants

        private Plugin _plugin;

        #endregion

        #region Constructer

        public PluginSteuerelement(Plugin Plugin)
        {
            InitializeComponent();

            plugin = Plugin;
        }

        #endregion

        #region Methods

        private void Load()
        {
            L_Name.Text = _plugin.Name;
            if (_plugin.versionnow != "0.0.0.0")
                L_VersionInstalled.Text = _plugin.versionnow;
            else
                L_VersionInstalled.Text = "noch nicht installiert!!!";
            L_Version.Text = _plugin.version;
            L_ID.Text = _plugin.Id;

            RTB_Description.Text = _plugin.Description;

            B_Download.Enabled = !_plugin.IsDownloaded;
            if (plugin.versionnow != _plugin.version&&_plugin.IsDownloaded)
                B_Update.Enabled = true;
            else
                B_Update.Enabled = false;
        }

        #endregion

        public Plugin plugin 
        {
            get
            {
                return _plugin;
            }
            private set
            {
                _plugin = value;
                Load();
            }
        }

        private void B_Download_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        private void B_Update_Click(object sender, EventArgs e)
        {

        }
    }
}
