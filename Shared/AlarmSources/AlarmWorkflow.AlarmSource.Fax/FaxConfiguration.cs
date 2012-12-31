using System;
using System.Collections.ObjectModel;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.AlarmSource.Fax
{
    /// <summary>
    /// Represents the current configuration. Wraps the SettingsManager-calls.
    /// </summary>
    internal sealed class FaxConfiguration
    {
        #region Properties

        internal string FaxPath { get; private set; }
        internal string ArchivePath { get; private set; }
        internal string AnalysisPath { get; private set; }
        internal string OCRSoftware { get; private set; }
        internal string OCRSoftwarePath { get; private set; }
        internal string AlarmFaxParserAlias { get; private set; }
        internal int RoutineInterval { get; private set; }
        internal ReadOnlyCollection<string> TestFaxKeywords { get; private set; }
        internal ReplaceDictionary ReplaceDictionary { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaxConfiguration"/> class.
        /// </summary>
        public FaxConfiguration()
        {
            this.FaxPath = SettingsManager.Instance.GetSetting("FaxAlarmSource", "FaxPath").GetString();
            this.ArchivePath = SettingsManager.Instance.GetSetting("FaxAlarmSource", "ArchivePath").GetString();
            this.AnalysisPath = SettingsManager.Instance.GetSetting("FaxAlarmSource", "AnalysisPath").GetString();
            this.AlarmFaxParserAlias = SettingsManager.Instance.GetSetting("FaxAlarmSource", "AlarmfaxParser").GetString();

            this.OCRSoftware = SettingsManager.Instance.GetSetting("FaxAlarmSource", "OCR.Software").GetString();
            this.OCRSoftwarePath = SettingsManager.Instance.GetSetting("FaxAlarmSource", "OCR.Path").GetString();

            this.RoutineInterval = SettingsManager.Instance.GetSetting("FaxAlarmSource", "Routine.Interval").GetInt32();
            this.TestFaxKeywords = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("FaxAlarmSource", "TestFaxKeywords").GetString().Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries));

            // Parse replace dictionary
            this.ReplaceDictionary = SettingsManager.Instance.GetSetting("FaxAlarmSource", "ReplaceDictionary").GetValue<ReplaceDictionary>();
        }

        #endregion
    }
}
