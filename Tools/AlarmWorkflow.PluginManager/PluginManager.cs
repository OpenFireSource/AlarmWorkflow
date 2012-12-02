using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;

using System.Reflection;

namespace AlarmWorkflow.Tools.PluginManager
{
    public partial class PluginManager : Form
    {
        #region Fields

        List<Plugin> _plugins;

        #endregion

        #region Constants

        string _path;

        #endregion

        #region Constructor

        public PluginManager()
        {
            InitializeComponent();

            _plugins = new List<Plugin>();
        }

        #endregion


        private void PluginManager_Load(object sender, EventArgs e)
        {
            DownloadPluginDetails();
            showPluginsToForm();
        }

        #region Methods

        private string TryGetValue(XmlElement ele, string Attribut)
        {
            string returnvalue = "";
            try
            {
                returnvalue = ele.GetAttribute(Attribut);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return returnvalue;
        }

        private bool IsDownloadedNow(string file)
        {
            bool returnvalue = false;
            if (File.Exists(file))
                returnvalue = true;
            return returnvalue;
        }

        private static Version ReadVersionFromAssembly(string filename)
        {
            Version InvalidVersion = new Version("0.0.0.1");
            string sharedAssemblyFileName = Path.Combine(Application.StartupPath, filename);
            if (File.Exists(sharedAssemblyFileName))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(sharedAssemblyFileName);

                    AssemblyFileVersionAttribute versionAttribute = (AssemblyFileVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).FirstOrDefault();
                    return new Version(versionAttribute.Version);
                }
                catch (Exception)
                {
                }
            }
            return InvalidVersion;
        }

        private void DownloadPluginDetails()
        {
            WebClient webclient = new WebClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    string serverVersionUri = "http://openfiresource.de/packages/plugins/plugins.xml";

                    using (MemoryStream stream = new MemoryStream(client.DownloadData(serverVersionUri)))
                    {
                        XmlDocument doc=new XmlDocument();
                        doc.Load(stream);
                        foreach (XmlElement ele_plugins in doc.GetElementsByTagName("plugins"))
                        {
                            foreach (XmlElement ele_plugin in ele_plugins.GetElementsByTagName("plugin"))
                            {
                                string id = TryGetValue(ele_plugin, "id");
                                string name = TryGetValue(ele_plugin, "name");
                                string description = ele_plugin.InnerText;
                                string url = TryGetValue(ele_plugin, "url");
                                string version = TryGetValue(ele_plugin, "version");
                                string category = TryGetValue(ele_plugin, "category");
                                string filename = TryGetValue(ele_plugin, "filename");
                                string versionnow = "0.0.0.0";
                                bool isdownloaded = IsDownloadedNow(filename);
                                if (isdownloaded)
                                    versionnow = ReadVersionFromAssembly(filename).ToString();
                                Plugin p = new Plugin(id, name, description, url, version, versionnow, isdownloaded, category, filename);
                                _plugins.Add(p);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void showPluginsToForm()
        {
            int x = 10;
            int y = 10;
            foreach (Plugin plu in _plugins)
            {
                PluginSteuerelement steuer = new PluginSteuerelement(plu);
                steuer.Location = new Point(x, y);
                this.Controls.Add(steuer);
                y += 195;
            }
        }

        #endregion
    }
}
