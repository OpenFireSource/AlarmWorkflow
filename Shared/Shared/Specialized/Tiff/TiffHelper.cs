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

namespace AlarmWorkflow.Shared.Specialized.Tiff
{
    /// <summary>
    /// Convert a Tiff image to a PDF
    /// </summary>
    public static class TiffHelper
    {
        /// <summary>
        /// Split a multipage tiff
        /// </summary>
        /// <param name="fileName">Tiff filename</param>
        /// <returns>Images</returns>
        public static IList<Image> SplitMultipage(string fileName)
        {
            List<Image> images = new List<Image>();

            using (Image tiffImage = Image.FromFile(fileName))
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
                        images.Add((Image)tiffImage.Clone());
                    }
                }
            }

            return images;
        }
        
        /// <summary>
        /// Convert a TIFF Image to a PDF-Document
        /// Output file get the same name with another extension
        /// </summary>
        /// <param name="source">Tiff-Image</param>
        /// <returns>PDF-Path</returns>
        public static string ConvertToPdf(string source)
        {
            string target = Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source) + ".pdf");
            ConvertToPdf(source, target);

            return target;
        }

        /// <summary>
        /// Convert a TIFF Image to a PDF-Document
        /// </summary>
        /// <param name="source">Tiff-Image</param>
        /// <param name="target">Output PDF-Document</param>
        public static void ConvertToPdf(string source, string target)
        {
            IList<Image> imageList = SplitMultipage(source);

            using (PdfDocument doc = imageList.ToPdf())
            {
                doc.Save(target);
            }

            foreach (Image image in imageList)
            {
                image.Dispose();
            }
        }

        /// <summary>
        /// Split a tiff and convert it to a jpeg image
        /// </summary>
        /// <param name="fileName">Tiff filename</param>
        /// <returns>Jpeg images</returns>
        public static string[] ConvertToJpegAndSplit(string fileName)
        {
            using (Image imageFile = Image.FromFile(fileName))
            {
                FrameDimension frameDimensions = new FrameDimension(imageFile.FrameDimensionsList[0]);
                int frameNum = imageFile.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    imageFile.SelectActiveFrame(frameDimensions, frame);
                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        string tempFileName = Path.GetTempFileName();

                        FileInfo fileInfo = new FileInfo(tempFileName);
                        fileInfo.Attributes = FileAttributes.Temporary;

                        jpegPaths[frame] = tempFileName;

                        bmp.Save(jpegPaths[frame], ImageFormat.Jpeg);
                    }
                }

                return jpegPaths;
            }
        }
    }
}