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

using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Job.OperationFileExporter
{
    class Configuration
    {
        #region Properties

        internal bool AMExportEnabled { get; private set; }
        internal string AMDestinationFileName { get; private set; }
        internal bool EvaExportEnabled { get; private set; }
        internal string EvaDestinationFileName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            AMExportEnabled = SettingsManager.Instance.GetSetting("OperationFileExporter", "AMExportEnabled").GetBoolean();
            AMDestinationFileName = SettingsManager.Instance.GetSetting("OperationFileExporter", "AMDestinationFileName").GetString();
            EvaExportEnabled = SettingsManager.Instance.GetSetting("OperationFileExporter", "EVAExportEnabled").GetBoolean();
            EvaDestinationFileName = SettingsManager.Instance.GetSetting("OperationFileExporter", "EVADestinationFileName").GetString();
        }

        #endregion
    }
}
