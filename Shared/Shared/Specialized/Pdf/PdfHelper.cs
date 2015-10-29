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

using Ghostscript.NET;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AlarmWorkflow.Shared.Specialized.Pdf
{
    public static class PdfHelper
    {
        /// <summary>
        /// Convert a pdf to a tiff image with 300 dpi
        /// </summary>
        /// <param name="inputFile">PDF filename</param>
        /// <returns>New tiff filename</returns>
        public static string ConvertToTiff(string inputFile)
        {
            string filename = Path.GetFileNameWithoutExtension(inputFile);
            string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), filename + ".tif");

            ConvertToTiff(inputFile, outputFile);

            return outputFile;
        }

        /// <summary>
        /// Convert a pdf to a tiff image with 300 dpi
        /// </summary>
        /// <param name="inputFile">PDF filename</param>
        /// <param name="outputFile">New Tiff filename</param>
        public static void ConvertToTiff(string inputFile, string outputFile)
        {
            if(!File.Exists(inputFile))
            {
                throw new FileNotFoundException("PDF file not found", inputFile);
            }

            try
            {
                GhostscriptImageDevice dev = new GhostscriptImageDevice();
                dev.Device = "tiffgray";
                dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.Resolution = 300;
                dev.InputFiles.Add(inputFile);
                dev.OutputPath = outputFile;

                dev.Process();
            }
            catch (GhostscriptException e)
            {
                throw new PdfConvertException(e.Message, e.CodeName);
            }
        }

        /// <summary>
        /// Extract the images from a pdf
        /// </summary>
        /// <param name="inputFile">Pdf filename</param>
        /// <returns>IEnumerable of extracted images</returns>
        public static IList<Image> ExtractImages(string inputFile)
        {
            List<Image> images = new List<Image>();

            using (PdfDocument document = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import))
            {
                foreach (Image image in document.GetImages())
                {
                    images.Add((Image)image.Clone());
                }
            }

            return images;
        }

    }
}
