// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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
        }

        #endregion
    }
}