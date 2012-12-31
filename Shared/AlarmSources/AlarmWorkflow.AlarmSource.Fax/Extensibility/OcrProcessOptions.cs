using System;

namespace AlarmWorkflow.AlarmSource.Fax.Extensibility
{
    /// <summary>
    /// Defines the options that shall be used by the OCR-software when processing a new image.
    /// </summary>
    public sealed class OcrProcessOptions
    {
        /// <summary>
        /// Gets the manually specified path to the OCR-software to use. If this is null, then the default path shall be used.
        /// </summary>
        public string SoftwarePath { get; internal set; }
        /// <summary>
        /// Gets the path to the image file to process.
        /// </summary>
        public string ImagePath { get; internal set; }
        /// <summary>
        /// Gets the file name where the analyzed file shall be stored.
        /// </summary>
        public string AnalyzedFileDestinationPath { get; internal set; }
    }
}
