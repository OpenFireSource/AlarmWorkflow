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
            using (ProcessWrapper proc = new ProcessWrapper())
            {
                // If there is no custom directory
                if (string.IsNullOrEmpty(options.SoftwarePath))
                {
                    proc.WorkingDirectory = Path.Combine(Utilities.GetWorkingDirectory(), "tesseract");
                }
                else
                {
                    proc.WorkingDirectory = options.SoftwarePath;
                }

                proc.FileName = Path.Combine(proc.WorkingDirectory, "tesseract.exe");
                proc.Arguments = options.ImagePath + " " + options.AnalyzedFileDestinationPath + " -psm 6 quiet";

                proc.StartAndWait();
            }

            // Correct txt path for tesseract (it will append .txt under windows always)
            string analyzedFile = options.AnalyzedFileDestinationPath + ".txt";

            return File.ReadAllLines(analyzedFile);
        }

        #endregion
    }
}
