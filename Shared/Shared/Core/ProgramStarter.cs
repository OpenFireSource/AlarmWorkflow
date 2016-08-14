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
using System.IO;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides the functionality to start an application including arguments/path in a single string.
    /// </summary>
    public class ProgramStarter
    {
        #region Constants

        private static readonly string[] SupportedExtensions = { ".bat", ".cmd", ".exe" };

        private const string ProgramNotSupported = "The program '{0}' is not supported. Supported program extensions are: {1}.";

        private const string ProgramStartFailed = "Could not start program '{0}'. If this program requires user interaction, it is not usable by this job.";

        #endregion

        #region Methods

        /// <summary>
        /// Starts the program
        /// </summary>
        /// <param name="fileNameWithArguments">The filename including all arguments and the path.</param>
        /// <param name="starter">The starting object. Required for logging.</param>
        public static void StartProgramTask(string fileNameWithArguments, object starter)
        {
            string fileName = "";
            string arguments = "";

            try
            {
                // Search for the extension. Take everything before as file name, and everything after as arguments.
                int iExt = -1;
                foreach (string ext in SupportedExtensions)
                {
                    iExt = fileNameWithArguments.IndexOf(ext);
                    if (iExt > -1)
                    {
                        fileName = fileNameWithArguments.Substring(0, iExt + ext.Length);
                        arguments = fileNameWithArguments.Remove(0, fileName.Length).Trim();

                        break;
                    }
                }

                // If program file is unsupported, skip execution and warn user.
                if (iExt == -1)
                {
                    Logger.Instance.LogFormat(LogType.Warning, starter, ProgramNotSupported, fileNameWithArguments, string.Join(", ", SupportedExtensions));
                    return;
                }

                ProcessWrapper proc = new ProcessWrapper();
                proc.FileName = fileName;
                proc.WorkingDirectory = Path.GetDirectoryName(fileName);
                proc.Arguments = arguments;

                proc.Start();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, starter, ProgramStartFailed, fileNameWithArguments);
                Logger.Instance.LogException(starter, ex);
            }

        }

        #endregion
    }
}