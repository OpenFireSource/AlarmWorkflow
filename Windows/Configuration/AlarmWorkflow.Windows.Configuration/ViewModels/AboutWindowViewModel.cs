using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.Properties;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class AboutWindowViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets a list containing information about the installed packages.
        /// </summary>
        public IList<PackageInfo> InstalledPackagesInfo { get; private set; }
        /// <summary>
        /// Gets a list containing advanced info entries.
        /// </summary>
        public IList<AdvancedInfoEntry> AdvancedInfoEntries { get; private set; }

        #endregion

        #region Commands

        #region Command "CopyToClipboardCommand"

        /// <summary>
        /// The CopyToClipboardCommand command.
        /// </summary>
        public ICommand CopyToClipboardCommand { get; private set; }

        private void CopyToClipboardCommand_Execute(object parameter)
        {
            CopyToClipboard();
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindowViewModel"/> class.
        /// </summary>
        public AboutWindowViewModel()
            : base()
        {
            BuildPackageInfoList();
            BuildAdvancedInfoEntries();
        }

        #endregion

        #region Methods

        private void BuildPackageInfoList()
        {
            List<PackageInfo> list = new List<PackageInfo>();

            foreach (string file in Directory.GetFiles(Utilities.GetWorkingDirectory()).Where(f => f.EndsWith(".dll") || f.EndsWith(".exe")))
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(file);

                    AlarmWorkflowPackageAttribute attribute = null;
                    if (!AlarmWorkflowPackageAttribute.TryGetAttribute(asm, out attribute))
                    {
                        continue;
                    }

                    list.Add(PackageInfo.FromAssembly(asm));
                }
                catch
                {
                    // Intentionally left empty
                }
            }

            InstalledPackagesInfo = new List<PackageInfo>(list.OrderBy(i => i.Name));
        }

        private void BuildAdvancedInfoEntries()
        {
            AdvancedInfoEntries = new List<AdvancedInfoEntry>();

            AdvancedInfoEntries.Add(new AdvancedInfoEntry("OSVersion", Environment.OSVersion));
            AdvancedInfoEntries.Add(new AdvancedInfoEntry("Is64BitOperatingSystem", Environment.Is64BitOperatingSystem));
            AdvancedInfoEntries.Add(new AdvancedInfoEntry("Is64BitProcess", Environment.Is64BitProcess));

            AdvancedInfoEntries.Add(new AdvancedInfoEntry("IsCurrentUserAdministrator", Helper.IsCurrentUserAdministrator()));

            AdvancedInfoEntries.Add(new AdvancedInfoEntry("IsServiceInstalled", ServiceHelper.IsServiceInstalled()));
            AdvancedInfoEntries.Add(new AdvancedInfoEntry("IsServiceRunning", ServiceHelper.IsServiceRunning()));
            if (ServiceHelper.IsServiceRunning())
            {
                AdvancedInfoEntries.Add(new AdvancedInfoEntry("ServiceState", ServiceHelper.GetServiceState()));
            }

            DirectoryInfo dirInfo = new DirectoryInfo(Utilities.GetWorkingDirectory());
            AdvancedInfoEntries.Add(new AdvancedInfoEntry("WorkingDirectory", dirInfo.FullName));

            DriveInfo driveInfo = new DriveInfo(dirInfo.FullName[0].ToString());

            bool isDriveReady = driveInfo.IsReady;
            AdvancedInfoEntries.Add(new AdvancedInfoEntry("ExecutingDriveIsReady", isDriveReady));
            if (isDriveReady)
            {
                try
                {
                    AdvancedInfoEntries.Add(new AdvancedInfoEntry("ExecutingDriveType", driveInfo.DriveType));
                    AdvancedInfoEntries.Add(new AdvancedInfoEntry("ExecutingDriveFormat", driveInfo.DriveFormat));
                    AdvancedInfoEntries.Add(new AdvancedInfoEntry("ExecutingDriveAvailableFreeSpace", driveInfo.AvailableFreeSpace));
                }
                catch (Exception)
                {
                    // Intentionally suppressed. IsReady is not reliable http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.IO.DRIVEINFO.ISREADY);k(DevLang-CSHARP)&rd=true.
                    AdvancedInfoEntries.Add(new AdvancedInfoEntry("ExecutingDriveError", "(error fetching drive info)"));
                }
            }
        }

        private void CopyToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("");
            sb.AppendLine("AlarmWorkflow Windows diagnostics");
            sb.AppendFormat("Created {0}", DateTime.Now).AppendLine();
            sb.AppendLine("");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("Installed packages:");
            sb.AppendLine("");

            foreach (PackageInfo pkgInfo in InstalledPackagesInfo)
            {
                sb.AppendFormat("{0} : {1}", pkgInfo.Name, pkgInfo.Version).AppendLine();
            }

            sb.AppendLine("");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("System information:");
            sb.AppendLine("");

            foreach (AdvancedInfoEntry entry in AdvancedInfoEntries)
            {
                sb.AppendFormat("{0} = {1}", entry.Key, entry.Value).AppendLine();
            }


            Clipboard.SetText(sb.ToString());
            UIUtilities.ShowInfo(Resources.CopyToClipboardDoneMessage);
        }

        #endregion

        #region Nested types

        internal class PackageInfo
        {
            public string Name { get; set; }
            public string Version { get; set; }

            internal static PackageInfo FromAssembly(Assembly assembly)
            {
                PackageInfo pi = new PackageInfo();
                pi.Name = assembly.GetName().Name;
                pi.Version = assembly.GetName().Version.ToString();

                return pi;
            }
        }

        internal class AdvancedInfoEntry
        {
            public string Key { get; set; }
            public object Value { get; set; }

            public AdvancedInfoEntry(string key, object value)
            {
                Assertions.AssertNotEmpty(key, "key");

                Key = key;
                Value = value;
            }
        }

        #endregion
    }
}
