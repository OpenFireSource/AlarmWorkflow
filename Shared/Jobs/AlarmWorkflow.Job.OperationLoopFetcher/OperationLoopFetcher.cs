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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;

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

        private Configuration _configuration;

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
            // This is a Pre-Job. Thus it only has to run right after the operation has surfaced and before being stored.
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
            if (File.Exists(_configuration.LoopsFilePath))
            {
                string[] lines = File.ReadAllLines(_configuration.LoopsFilePath);
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
                    if (!DateTime.TryParseExact(timestampRaw, _configuration.EntryDateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out timestamp))
                    {
                        // Parsing was not successful. Skip line.
                        continue;
                    }

                    if ((DateTime.Now - timestamp) > _configuration.MaxEntryAge)
                    {
                        // Speed optimization: As soon as we encounter an old entry, exit the whole process immediately.
                        // We assume that the newest entries are last in the file, so we can easily break.
                        break;
                    }

                    yield return loop;
                }
            }
        }

        bool IJob.Initialize()
        {
            _configuration = new Configuration();
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