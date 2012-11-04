using System;
using System.Collections.ObjectModel;
using AlarmWorkflow.Shared.Core;
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
        internal OcrSoftware OCRSoftware { get; private set; }
        internal string OCRSoftwarePath { get; private set; }
        internal string AlarmFaxParserAlias { get; private set; }
        internal int RoutineInterval { get; private set; }
        internal ReadOnlyCollection<string> TestFaxKeywords { get; private set; }
        private ReplaceDictionary ReplaceDictionary { get; set; }

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

            string ocr = SettingsManager.Instance.GetSetting("FaxAlarmSource", "OCR.Software").GetString();
            this.OCRSoftware = (OcrSoftware)Enum.Parse(typeof(OcrSoftware), ocr);
            this.OCRSoftwarePath = SettingsManager.Instance.GetSetting("FaxAlarmSource", "OCR.Path").GetString();

            this.RoutineInterval = SettingsManager.Instance.GetSetting("FaxAlarmSource", "Routine.Interval").GetInt32();
            this.TestFaxKeywords = new ReadOnlyCollection<string>(SettingsManager.Instance.GetSetting("FaxAlarmSource", "TestFaxKeywords").GetString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

            // Parse replace dictionary
            this.ReplaceDictionary = SettingsManager.Instance.GetSetting("FaxAlarmSource", "ReplaceDictionary").GetValue<ReplaceDictionary>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the given line and replaces its contents according to the ReplaceDictionary.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal string PerformReplace(string line)
        {
            foreach (var pair in this.ReplaceDictionary.Pairs)
            {
                line = line.Replace(pair.Key, pair.Value);
            }
            return line;
        }

        #endregion
    }
}
