using System.Diagnostics;
using System.IO;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    [Export("Cuneiform", typeof(IOcrSoftware))]
    sealed class CuneiformOcrSoftware : IOcrSoftware
    {
        #region IOcrSoftware Members

        string[] IOcrSoftware.ProcessImage(OcrProcessOptions options)
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
                proc.StartInfo.Arguments = "-l ger --singlecolumn -o " + options.AnalyzedFileDestinationPath + " " + options.ImagePath;

                proc.Start();
                proc.WaitForExit();
            }

            return File.ReadAllLines(options.AnalyzedFileDestinationPath);
        }

        #endregion
    }
}
