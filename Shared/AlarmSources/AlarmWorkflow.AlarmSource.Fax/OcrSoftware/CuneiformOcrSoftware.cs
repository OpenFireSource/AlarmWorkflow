using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    [Export("Cuneiform", typeof(IOcrSoftware))]
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
            using (Process proc = new Process())
            {
                proc.EnableRaisingEvents = false;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                // If there is no custom directory
                if (string.IsNullOrEmpty(options.SoftwarePath))
                {
                    proc.StartInfo.WorkingDirectory = Path.Combine(Utilities.GetWorkingDirectory(), "cuneiform");
                }
                else
                {
                    proc.StartInfo.WorkingDirectory = options.SoftwarePath;
                }

                proc.StartInfo.FileName = Path.Combine(proc.StartInfo.WorkingDirectory, "cuneiform.exe");


                string singlepageTiffAnalyzedFile = Path.Combine(Path.GetDirectoryName(options.AnalyzedFileDestinationPath), Path.GetFileName(singlepageTiffFileName) + ".txt");
                proc.StartInfo.Arguments = "-l ger --singlecolumn -o " + singlepageTiffAnalyzedFile + " " + singlepageTiffFileName;

                proc.Start();
                proc.WaitForExit();

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
