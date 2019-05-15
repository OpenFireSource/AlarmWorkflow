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

using System.IO;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;
using System;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    class TesseractOcrSoftware : IOcrSoftware
    {

        #region Constants

        private const string EnvTessdata = "TESSDATA_PREFIX";

        #endregion

        #region IOcrSoftware Members

        string[] IOcrSoftware.ProcessImage(OcrProcessOptions options)
        {
            using (ProcessWrapper proc = new ProcessWrapper())
            {
                string tesseractPath = Path.Combine(Utilities.GetWorkingDirectory(), "tesseract");

                try
                {
                    Environment.SetEnvironmentVariable(EnvTessdata, tesseractPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.OcrSoftwareTesseractError);
                    Logger.Instance.LogException(this, ex);
                }
                
                
                // If there is no custom directory
                if (string.IsNullOrEmpty(options.SoftwarePath))
                {
                    proc.WorkingDirectory = tesseractPath;
                }
                else
                {
                    proc.WorkingDirectory = options.SoftwarePath;
                }

                proc.FileName = Path.Combine(proc.WorkingDirectory, "tesseract.exe");
                proc.Arguments = string.Format("-l deu \"{0}\" \"{1}\" --psm 6 quiet", options.ImagePath, options.AnalyzedFileDestinationPath);

                proc.StartAndWait();
            }

            // Correct txt path for tesseract (it will append .txt under windows always)
            string analyzedFile = options.AnalyzedFileDestinationPath + ".txt";

            return File.ReadAllLines(analyzedFile);
        }

        #endregion
    }
}
