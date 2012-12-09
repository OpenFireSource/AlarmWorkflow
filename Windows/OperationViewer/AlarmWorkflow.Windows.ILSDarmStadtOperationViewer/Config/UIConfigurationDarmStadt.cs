using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using AlarmWorkflow.Shared.Core;
using System.Xml.Linq;
using System.Linq;
using System;

namespace AlarmWorkflow.Windows.ILSDarmStadtOperationViewer
{
    /// <summary>
    /// Represents the configuration for the Windows UI.
    /// </summary>
    internal sealed class UIConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets the abbreviations that must be contained within the resource name.
        /// </summary>
        public string[] VehicleMustContainAbbreviations { get; private set; }
        /// <summary>
        /// Gets the configuration of each vehicle.
        /// </summary>
        public IList<Vehicle> Vehicles { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIConfiguration"/> class.
        /// </summary>
        public UIConfiguration()
        {
            Vehicles = new List<Vehicle>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Searches for the <see cref="Vehicle"/>-instance that is configured for the given resource name string.
        /// </summary>
        /// <param name="resourceName">The full name of the resource to find the vehicle for.</param>
        /// <returns>The <see cref="Vehicle"/>-instance representing the resource.
        /// -or- null, if the resource is either not allowed (does not contain the mandatory abbreviation) or is not configured.</returns>
        public Vehicle FindMatchingResource(string resourceName)
        {
            // If the resource does not contain any of the abbreviations, don't go further.
            if (!VehicleMustContainAbbreviations.Any(v => resourceName.Contains(v)))
            {
                return null;
            }
            // Even if the resource name seems allowed, check if this resource is configured, and return it if it is (otherwise null).
            return Vehicles.FirstOrDefault(v => resourceName.ToUpperInvariant().Contains(v.Identifier));
        }

        /// <summary>
        /// Loads the UIConfiguration from its default path.
        /// </summary>
        /// <returns></returns>
        public static UIConfiguration Load()
        {
            string configFile = Path.Combine(Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly()), "Config\\IlsAnsbachOperationViewerConfig.xml");
            if (configFile == null)
            {
                return null;
            }

            UIConfiguration configuration = new UIConfiguration();

            XDocument doc = XDocument.Load(configFile);

            XElement vehicleE = doc.Root.Element("Vehicles");
            configuration.VehicleMustContainAbbreviations = vehicleE.Attribute("MustContainAbbreviations").Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (XElement resE in vehicleE.Elements("Vehicle"))
            {
                Vehicle vehicle = new Vehicle();
                vehicle.Identifier = resE.Attribute("Identifier").Value;
                vehicle.Name = resE.Attribute("Name").Value;

                FileInfo imageFile = new FileInfo(Path.Combine(Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly()), resE.Attribute("Image").Value));
                vehicle.Image = imageFile.FullName;

                configuration.Vehicles.Add(vehicle);
            }

            return configuration;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Provides detail for each of the fire department's vehicles.
        /// </summary>
        public class Vehicle
        {
            /// <summary>
            /// Gets/sets the identifier of this vehicle as it appears in the alarmfax.
            /// </summary>
            [XmlAttribute()]
            public string Identifier { get; set; }
            /// <summary>
            /// Gets/sets the display name of the vehicle, if it differs from the <see cref="Identifier"/>-name.
            /// If this is null or empty, the name from <see cref="Identifier"/> is used in the UI.
            /// </summary>
            [XmlAttribute()]
            public string Name { get; set; }
            /// <summary>
            /// Gets/sets the path to the image that is used for this vehicle in the UI.
            /// </summary>
            [XmlAttribute()]
            public string Image { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Vehicle"/> class.
            /// </summary>
            public Vehicle()
            {

            }
        }

        #endregion
    }
}
