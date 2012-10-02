using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using AlarmWorkflow.Shared.Core;
using System.Xml.Linq;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Represents the configuration for the Windows UI.
    /// </summary>
    public sealed class UIConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets/sets the location of the fire department. This is required for route calculation.
        /// </summary>
        [XmlElement()]
        public PropertyLocation FireDepartmentProperty { get; set; }
        /// <summary>
        /// Gets/sets the scale factor of the UI.
        /// </summary>
        [XmlElement()]
        public double ScaleFactor { get; set; }
        /// <summary>
        /// Gets/sets the dimensions of the route image.
        /// </summary>
        [XmlElement()]
        public int RouteImageHeight { get; set; }
        /// <summary>
        /// Gets/sets the dimensions of the route image.
        /// </summary>
        [XmlElement()]
        public int RouteImageWidth { get; set; }
        /// <summary>
        /// Gets/sets the configuration of each vehicle.
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(Vehicle))]
        public List<Vehicle> Vehicles { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIConfiguration"/> class.
        /// </summary>
        public UIConfiguration()
        {
            FireDepartmentProperty = new PropertyLocation();
            Vehicles = new List<Vehicle>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the UIConfiguration from its default path.
        /// </summary>
        /// <returns></returns>
        public static UIConfiguration Load()
        {
            string configFile = Path.Combine(Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly()), "Config\\UIConfiguration.xml");
            if (configFile == null)
            {
                return null;
            }

            using (Stream stream = File.OpenRead(configFile))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UIConfiguration));
                return (UIConfiguration)serializer.Deserialize(stream);
            }
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
