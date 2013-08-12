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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    [Export("Cuneiform", typeof(IOcrSoftware))]
    [Information(DisplayName = "ExportCuneiformOcrDisplayName", Description = "ExportCuneiformOcrDescription")]
    sealed class CuneiformOcrSoftware : IOcrSoftware
    {
        #region IOcrSoftware Members

        string[] IOcrSoftware.ProcessImage(OcrProcessOptions options)
        {
            List<string> analyzedLines = new List<string>();

            foreach (string singlepageTiffFileName in SplitMultipageTiff(options.ImagePath))
            {
                analyzedLines.AddRange(AnalyzeSinglepageTiff(singlepageTiffFileName, options));
            }

            // Finally write all analyzed lines to the desired path
            File.WriteAllLines(options.AnalyzedFileDestinationPath + ".txt", analyzedLines);
            return analyzedLines.ToArray();
        }

        private string[] AnalyzeSinglepageTiff(string singlepageTiffFileName, OcrProcessOptions options)
        {
            using (ProcessWrapper proc = new ProcessWrapper())
            {
                // If there is no custom directory
                if (string.IsNullOrEmpty(options.SoftwarePath))
                {
                    proc.WorkingDirectory = Path.Combine(Utilities.GetWorkingDirectory(), "cuneiform");
                }
                else
                {
                    proc.WorkingDirectory = options.SoftwarePath;
                }

                proc.FileName = Path.Combine(proc.WorkingDirectory, "cuneiform.exe");


                string singlepageTiffAnalyzedFile = Path.Combine(Path.GetDirectoryName(options.AnalyzedFileDestinationPath), Path.GetFileName(singlepageTiffFileName) + ".txt");
                proc.Arguments = string.Format("-l ger --singlecolumn -o \"{0}\" \"{1}\"", singlepageTiffAnalyzedFile, singlepageTiffFileName);

                proc.StartAndWait();

                string[] lines = File.ReadAllLines(singlepageTiffAnalyzedFile);

                TryDeleteFile(singlepageTiffFileName);
                TryDeleteFile(singlepageTiffAnalyzedFile);

                return lines;
            }
        }

        private IEnumerable<string> SplitMultipageTiff(string tiffFileName)
        {
            // Idea taken from http://code.msdn.microsoft.com/windowsdesktop/Split-multi-page-tiff-file-058050cc

            using (Image tiffImage = Image.FromFile(tiffFileName))
            {
                Guid objGuid = tiffImage.FrameDimensionsList[0];
                FrameDimension dimension = new FrameDimension(objGuid);
                int noOfPages = tiffImage.GetFrameCount(dimension);

                foreach (Guid guid in tiffImage.FrameDimensionsList)
                {
                    for (int index = 0; index < noOfPages; index++)
                    {
                        FrameDimension currentFrame = new FrameDimension(guid);
                        tiffImage.SelectActiveFrame(currentFrame, index);

                        string fileName = Path.Combine(Path.GetDirectoryName(tiffFileName), Path.GetFileNameWithoutExtension(tiffFileName) + "_cp" + index + ".bmp");
                        tiffImage.Save(fileName, ImageFormat.Bmp);
                        yield return fileName;
                    }
                }
            }
        }

        private void TryDeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.CuneiformDeleteTempFileError, fileName);
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion
    }
}