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

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWorkflow.AlarmSource.Fax.Extensibility
{
    class Tiff2Pdf
    {
        /// <summary>
        /// Convert a TIFF Image to a PDF-Document
        /// Output file get the same name with another extension
        /// </summary>
        /// <param name="source">Tiff-Image</param>
        /// <returns>PDF-Path</returns>
        public static string Convert(string source)
        {
            string target = Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source) + ".pdf");
            Convert(source, target);

            return target;
        }

        /// <summary>
        /// Convert a TIFF Image to a PDF-Document
        /// </summary>
        /// <param name="source">Tiff-Image</param>
        /// <param name="target">Output PDF-Document</param>
        public static void Convert(string source, string target)
        {
            using (PdfDocument doc = new PdfDocument())
            using (Image sourceImage = Bitmap.FromFile(source))
            {
                var pageCount = sourceImage.GetFrameCount(FrameDimension.Page);

                for (int i = 0; i < pageCount; i++)
                {
                    PdfPage page = new PdfPage();

                    Image tiffImg = getTiffImage(sourceImage, i);
                    XImage img = XImage.FromGdiPlusImage(tiffImg);

                    page.Width = img.PointWidth;
                    page.Height = img.PointHeight;
                    doc.Pages.Add(page);

                    XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[i]);
                    xgr.DrawImage(img, 0, 0);

                }

                doc.Save(target);
                doc.Close();
            }
        }

        private static Image getTiffImage(Image sourceImage, int pageNumber)
        {
            MemoryStream ms = null;
            Image returnImage = null;

            try
            {
                ms = new MemoryStream();
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);
                sourceImage.SelectActiveFrame(objDimension, pageNumber);
                sourceImage.Save(ms, ImageFormat.Tiff);
                returnImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                returnImage = null;
            }

            return returnImage;
        }
    }
}
