using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Config
{
    /// <summary>
    /// Represents the current configuration.
    /// </summary>
    public sealed class Configuration
    {
        #region Fields

        private static Configuration _instance;
        /// <summary>
        /// Gets the current Configuration.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }

        #endregion

        #region Properties

        internal string OperationStoreAlias { get; private set; }
        internal string RoutePlanProviderAlias { get; private set; }
        internal bool DownloadRoutePlan { get; private set; }
        internal int RouteImageHeight { get; private set; }
        internal int RouteImageWidth { get; private set; }

        /// <summary>
        /// Gets the information about the current fire department site.
        /// </summary>
        /// <remarks>This information is used (among others) to provide the route information to the operation destination.</remarks>
        public FireDepartmentInfo FDInformation { get; private set; }

        internal ReadOnlyCollection<string> EnabledJobs { get; private set; }
        internal ReadOnlyCollection<string> EnabledAlarmSources { get; private set; }

        #endregion

        #region Constructors

        private Configuration()
        {
            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Config\AlarmWorkflow.xml";
            if (!File.Exists(fileName))
            {
                return;
            }

            XDocument doc = XDocument.Load(fileName);

            this.OperationStoreAlias = doc.Root.Element("OperationStore").Value;
            this.RoutePlanProviderAlias = doc.Root.Element("RoutePlanProvider").Value;

            this.DownloadRoutePlan = bool.Parse(doc.Root.Element("DownloadRoutePlan").Value);
            this.RouteImageHeight = int.Parse(doc.Root.Element("RouteImageHeight").Value);
            this.RouteImageWidth = int.Parse(doc.Root.Element("RouteImageWidth").Value);

            // FD Information
            {
                XElement fdInfoE = doc.Root.Element("FDInformation");
                this.FDInformation = new FireDepartmentInfo();
                this.FDInformation.Name = fdInfoE.Element("Name").Value;
                this.FDInformation.Location = new PropertyLocation();
                this.FDInformation.Location.ZipCode = fdInfoE.Element("ZipCode").Value;
                this.FDInformation.Location.City = fdInfoE.Element("City").Value;
                this.FDInformation.Location.Street = fdInfoE.Element("Street").Value;
                this.FDInformation.Location.StreetNumber = fdInfoE.Element("StreetNumber").Value;
            }

            // Jobs configuration
            {
                XElement jobsConfigE = doc.Root.Element("JobsConfiguration");
                List<string> jobs = new List<string>();
                foreach (XElement exportE in jobsConfigE.Elements("Job"))
                {
                    if (!bool.Parse(exportE.Attribute("IsEnabled").Value))
                    {
                        continue;
                    }

                    string jobName = exportE.TryGetAttributeValue("Name", null);
                    if (string.IsNullOrWhiteSpace(jobName))
                    {
                        continue;
                    }
                    jobs.Add(jobName);
                }

                this.EnabledJobs = new ReadOnlyCollection<string>(jobs);
            }
            // AlarmSources configuration
            {
                XElement sourcesConfigE = doc.Root.Element("AlarmSourcesConfiguration");
                List<string> sources = new List<string>();
                foreach (XElement exportE in sourcesConfigE.Elements("AlarmSource"))
                {
                    if (!bool.Parse(exportE.Attribute("IsEnabled").Value))
                    {
                        continue;
                    }

                    string sourceName = exportE.TryGetAttributeValue("Name", null);
                    if (string.IsNullOrWhiteSpace(sourceName))
                    {
                        continue;
                    }
                    sources.Add(sourceName);
                }

                this.EnabledAlarmSources = new ReadOnlyCollection<string>(sources);
            }

            _instance = this;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents information about the current fire department site.
        /// </summary>
        public class FireDepartmentInfo
        {
            /// <summary>
            /// Gets the name of the site.
            /// </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Gets the location of the site.
            /// </summary>
            public PropertyLocation Location { get; internal set; }
        }


        #endregion
    }
}
