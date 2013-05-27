using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Provides a helper class that reads the printing settings for a single print job from an assembly's settings.
    /// A printing configuration may consist of several print jobs, one for each printer or multiple jobs per printer, or similar.
    /// </summary>
    [Obsolete("Use the new 'PrintingQueueManager' instead!")]
    public class PrintJobConfiguration
    {
        #region Constants

        /// <summary>
        /// Defines the name of the "Copy count" setting.
        /// </summary>
        public const string CopyCountSettingName = "CopyCount";
        /// <summary>
        /// Defines the name of the "Print server" setting.
        /// </summary>
        public const string PrintServerSettingName = "PrintServer";
        /// <summary>
        /// Defines the name of the "Printer name" setting.
        /// </summary>
        public const string PrinterNameSettingName = "PrinterName";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Uri of the print server. Leave blank to use the local print server.
        /// </summary>
        public string PrintServer { get; private set; }
        /// <summary>
        /// Gets the name of the printer to use for printing. Blank uses the printer marked as default.
        /// </summary>
        public string PrinterName { get; private set; }
        /// <summary>
        /// Gets the amount of copies to print per job/operation.
        /// </summary>
        public int CopyCount { get; private set; }

        /// <summary>
        /// Returns true if the configuration represents the localhost print server.
        /// This is the case if there is no print server specified.
        /// </summary>
        public bool IsLocalPrintServer
        {
            get { return string.IsNullOrWhiteSpace(PrintServer); }
        }

        /// <summary>
        /// Returns true if the configuration represents the default printer for the print server.
        /// This is the case if there is no printer name specified.
        /// </summary>
        public bool IsDefaultPrinter
        {
            get { return string.IsNullOrWhiteSpace(PrinterName); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintJobConfiguration"/> class.
        /// </summary>
        public PrintJobConfiguration()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="T:PrintingConfiguration"/>-instance using the given identifier,
        /// and infers the settings from the default setting names (see constants).
        /// To use custom setting names, use the overload.
        /// </summary>
        /// <param name="identifier">The identifier to load the settings from.</param>
        /// <returns>The created <see cref="T:PrintingConfiguration"/>.</returns>
        public static PrintJobConfiguration FromSettings(string identifier)
        {
            return FromSettings(identifier, PrintServerSettingName, PrinterNameSettingName, CopyCountSettingName);
        }

        /// <summary>
        /// Creates a new <see cref="T:PrintingConfiguration"/>-instance using the given identifier,
        /// and infers the settings from the default setting names (see constants).
        /// To use custom setting names, use the overload.
        /// </summary>
        /// <param name="identifier">The identifier to load the settings from.</param>
        /// <param name="printServerSettingName">The custom name of the "Print server" setting.</param>
        /// <param name="printerNameSettingName">The custom name of the "Printer name" setting.</param>
        /// <param name="copyCountSettingName">The custom name of the "Copy count" setting.</param>
        /// <returns>The created <see cref="T:PrintingConfiguration"/>.</returns>
        public static PrintJobConfiguration FromSettings(string identifier,
            string printServerSettingName,
            string printerNameSettingName,
            string copyCountSettingName)
        {
            Assertions.AssertNotEmpty(identifier, "identifier");
            Assertions.AssertNotEmpty(printServerSettingName, "printServerSettingName");
            Assertions.AssertNotEmpty(printerNameSettingName, "printerNameSettingName");
            Assertions.AssertNotEmpty(copyCountSettingName, "copyCountSettingName");

            PrintJobConfiguration configuration = new PrintJobConfiguration();
            configuration.PrintServer = SettingsManager.Instance.GetSetting(identifier, printServerSettingName).GetString();
            configuration.PrinterName = SettingsManager.Instance.GetSetting(identifier, printerNameSettingName).GetString();
            configuration.CopyCount = SettingsManager.Instance.GetSetting(identifier, copyCountSettingName).GetInt32();

            return configuration;
        }

        #endregion
    }
}
