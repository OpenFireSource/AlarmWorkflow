using System.Diagnostics;
using System.IO;
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    [Export("Tesseract", typeof(IOcrSoftware))]
    class TesseractOcrSoftware : IOcrSoftware
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
                    proc.StartInfo.WorkingDirectory = Path.Combine(Utilities.GetWorkingDirectory(), "tesseract");
                }
                else
                {
                    proc.StartInfo.WorkingDirectory = options.SoftwarePath;
                }

                // TODO: Verify!!!
                proc.StartInfo.FileName = Path.Combine(proc.StartInfo.WorkingDirectory, "tesseract.exe");
                proc.StartInfo.Arguments = options.ImagePath + ".bmp " + options.AnalyzedFileDestinationPath + " -l deu";

                proc.Start();
                proc.WaitForExit();
            }

            // Correct txt path for tesseract (it will append .txt under windows always)
            string analyzedFile = options.AnalyzedFileDestinationPath + ".txt";

            return File.ReadAllLines(analyzedFile);
        }

        #endregion
    }
}
