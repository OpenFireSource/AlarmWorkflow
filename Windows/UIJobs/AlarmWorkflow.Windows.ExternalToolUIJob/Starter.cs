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
using System.IO;
using System.Linq;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ExternalToolUIJob.Properties;

namespace AlarmWorkflow.Windows.ExternalToolUIJob
{
    class Starter
    {
        #region Constants

        private static readonly string[] SupportedExtensions = { ".bat", ".cmd", ".exe" };

        #endregion

        #region Methods

        internal static void StartProgramTask(string fileNameWithArguments, object starter)
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
                   Logger.Instance.LogFormat(LogType.Warning, starter, Resources.ProgramNotSupported, fileNameWithArguments, string.Join(", ", SupportedExtensions));
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
               Logger.Instance.LogFormat(LogType.Error, starter, Resources.ProgramStartFailed, fileNameWithArguments);
               Logger.Instance.LogException(starter, ex);
           }

        }

        #endregion
    }
}
