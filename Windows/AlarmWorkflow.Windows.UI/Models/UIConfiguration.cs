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
        /// Gets/sets the Id of the screen to show the window at. Set to 0 (zero) to pick the primary screen.
        /// </summary>
        public int ScreenId { get; set; }
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
            configuration.OperationViewer = doc.Root.TryGetElementValue("OperationViewer", null);
            configuration.ScaleFactor = (double)doc.Root.TryGetElementValue("ScaleFactor", 2.0f);
            configuration.ScreenId = doc.Root.TryGetElementValue("ScreenId", 0);

            XElement aoaE = doc.Root.Element("AutomaticOperationAcknowledgementSettings");
            configuration.AutomaticOperationAcknowledgement.IsEnabled = aoaE.TryGetAttributeValue("IsEnabled", true);
            configuration.AutomaticOperationAcknowledgement.MaxAge = aoaE.TryGetAttributeValue("MaxAge", 480);

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
