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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.OperationLoopFetcher
{
    /// <summary>
    /// Represents a job that runs right after an operation has surfaced. It has the intention to supply loop information to the operation so that it can be further used in the application.
    /// The loop information is gathered by various devices.
    /// </summary>
    [Export("OperationLoopFetcher", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class OperationLoopFetcher : IJob
    {
        #region Fields

        private ISettingsServiceInternal _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLoopFetcher"/> class.
        /// </summary>
        public OperationLoopFetcher()
        {

        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase == JobPhase.OnOperationSurfaced)
            {
                AnalyzeLoopsInfoFile(operation);
            }
        }

        private void AnalyzeLoopsInfoFile(Operation operation)
        {
            operation.Loops.AddRange(GetLoopsSinceNow());
        }

        private IEnumerable<string> GetLoopsSinceNow()
        {
            string loopsFilePath = _settings.GetSetting(SettingKeys.LoopsFilePath).GetValue<string>();

            if (File.Exists(loopsFilePath))
            {
                TimeSpan maxEntryAge = TimeSpan.FromSeconds(_settings.GetSetting(SettingKeys.MaxEntryAge).GetValue<int>());
                string entryDateTimeFormat = _settings.GetSetting(SettingKeys.EntryDateTimeFormat).GetValue<string>();

                string[] lines = File.ReadAllLines(loopsFilePath);

                // Read lines in reverse to save some time (most recent entries are appended).
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    string line = lines[i];

                    string[] tokens = line.Split(';');
                    if (tokens.Length != 2)
                    {
                        continue;
                    }

                    string loop = tokens[0];
                    string timestampRaw = tokens[1];

                    if (string.IsNullOrWhiteSpace(loop))
                    {
                        continue;
                    }

                    DateTime timestamp = DateTime.Now;
                    if (!DateTime.TryParseExact(timestampRaw, entryDateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out timestamp))
                    {
                        continue;
                    }

                    if ((DateTime.Now - timestamp) > maxEntryAge)
                    {
                        // Speed optimization: As soon as we encounter an old entry, exit the whole process immediately.
                        // We assume that the newest entries are last in the file, so we can easily break.
                        break;
                    }

                    yield return loop;
                }
            }
        }

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

            return true;
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}