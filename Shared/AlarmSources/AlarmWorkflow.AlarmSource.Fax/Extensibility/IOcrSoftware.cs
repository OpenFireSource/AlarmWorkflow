using System;

namespace AlarmWorkflow.AlarmSource.Fax.Extensibility
{
    /// <summary>
    /// Defines mechanisms for a type that represents an OCR-software that is used for parsing incoming faxes.
    /// </summary>
    public interface IOcrSoftware
    {
        /// <summary>
        /// Processes the specified image using the OCR software.
        /// </summary>
        /// <param name="options">The processing options that shall be used.</param>
        /// <returns>The processed text contents from the image.</returns>
        string[] ProcessImage(OcrProcessOptions options);
    }
}
