using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UI.Models
{
    /// <summary>
    /// Represents the configuration for the Windows UI.
    /// </summary>
    public sealed class UIConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets/sets the alias of the operation viewer to use. Empty means that the default viewer shall be used.
        /// </summary>
        public string OperationViewer { get; set; }
        /// <summary>
        /// Gets/sets the scale factor of the UI.
        /// </summary>
        public double ScaleFactor { get; set; }
        /// <summary>
        /// Gets/sets the automatic operation acknowledgement settings.
        /// </summary>
        public AutomaticOperationAcknowledgementSettings AutomaticOperationAcknowledgement { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIConfiguration"/> class.
        /// </summary>
        public UIConfiguration()
        {
            AutomaticOperationAcknowledgement = new AutomaticOperationAcknowledgementSettings();
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

            XDocument doc = XDocument.Load(configFile);

            UIConfiguration configuration = new UIConfiguration();
            configuration.OperationViewer = doc.Root.Element("OperationViewer").Value;
            configuration.ScaleFactor = double.Parse(doc.Root.Element("ScaleFactor").Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
            configuration.AutomaticOperationAcknowledgement.IsEnabled = bool.Parse(doc.Root.Element("AutomaticOperationAcknowledgement").Attribute("IsEnabled").Value);
            configuration.AutomaticOperationAcknowledgement.MaxAge = int.Parse(doc.Root.Element("AutomaticOperationAcknowledgement").Attribute("MaxAge").Value);

            return configuration;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Configures the automatic operation acknowledgement.
        /// </summary>
        public class AutomaticOperationAcknowledgementSettings
        {
            /// <summary>
            /// Gets/sets whether or not the automatic operation acknowledgement is enabled.
            /// </summary>
            [XmlAttribute()]
            public bool IsEnabled { get; set; }
            /// <summary>
            /// Gets/sets the maximum age in minutes until an operation is automatically acknowleded.
            /// </summary>
            [XmlAttribute()]
            public int MaxAge { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="AutomaticOperationAcknowledgementSettings"/> class.
            /// </summary>
            public AutomaticOperationAcknowledgementSettings()
            {
            }
        }

        #endregion
    }
}
