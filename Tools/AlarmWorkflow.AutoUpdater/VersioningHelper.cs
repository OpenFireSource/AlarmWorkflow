using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AlarmWorkflow.Tools.AutoUpdater
{
    static class VersioningHelper
    {
        internal static readonly Version InvalidVersion = new Version("0.0.0.0");

        internal static Version GetLocalVersion()
        {
            string versionFileName = Path.Combine(Application.StartupPath, "version.config");
            if (File.Exists(versionFileName))
            {
                return ReadVersionFromVersionConfig(versionFileName);
            }
            return ReadVersionFromAlarmWorkflowSharedAssembly();
        }

        private static Version ReadVersionFromVersionConfig(string versionFileName)
        {
            XDocument doc = XDocument.Load(versionFileName);
            return ReadVersionFromVersionXml(doc);
        }

        private static Version ReadVersionFromVersionXml(XDocument doc)
        {
            string versionString = doc.Root.Attribute("text").Value;
            return new Version(versionString);
        }

        private static Version ReadVersionFromAlarmWorkflowSharedAssembly()
        {
            string sharedAssemblyFileName = Path.Combine(Application.StartupPath, "AlarmWorkflow.Shared.dll");
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

        internal static Version GetServerVersion()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string serverVersionUri = string.Format("{0}/{1}/{2}", Properties.Settings.Default.UpdateServerName, Properties.Settings.Default.UpdateFilesDirectory, Properties.Settings.Default.UpdateServerVersionFileName);

                    using (MemoryStream stream = new MemoryStream(client.DownloadData(serverVersionUri)))
                    {
                        XDocument doc = XDocument.Load(stream);
                        return ReadVersionFromVersionXml(doc);
                    }
                }
                catch (WebException ex)
                {
                    Utilities.ShowMessageBox(MessageBoxIcon.Error, "Error while downloading version info: {0}", ex.Message);
                }
                finally
                {

                }
                return InvalidVersion;
            }
        }
    }
}
