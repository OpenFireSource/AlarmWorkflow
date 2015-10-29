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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;

namespace AlarmWorkflow.Job.MailingJob
{
    static class Helpers
    {
        internal static MailAddress ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }
            return new MailAddress(address);
        }

        internal static Stream ToStream(this Image image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
        
        internal static Bitmap CombineBitmap(string[] files)
        {
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            int width = 0;
            int height = 0;

            foreach (string image in files)
            {
                Bitmap bitmap = new Bitmap(image);
                height += bitmap.Height;
                width = bitmap.Width > width ? bitmap.Width : width;
                images.Add(bitmap);
            }

            finalImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.Clear(Color.Black);

                int offset = 0;

                foreach (Bitmap image in images)
                {
                    g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                    offset += image.Height;
                    image.Dispose();
                }
            }

            return finalImage;
        }

    }
}