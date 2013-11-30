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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Threading;
using AlarmWorkflow.Job.AlarmSourcePrinterJob.Properties;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Job.AlarmSourcePrinterJob
{
    [Export("AlarmSourcePrinterJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class AlarmSourcePrinterJob : IJob
    {
        #region Methods

        private void PrintFaxes(IJobContext context, Operation operation)
        {
            if (!context.Parameters.ContainsKey("ArchivedFilePath") || !context.Parameters.ContainsKey("ImagePath"))
            {
                Logger.Instance.LogFormat(LogType.Trace, this, Resources.NoPrintingPossible);
                return;
            }

            System.IO.FileInfo sourceImageFile = new System.IO.FileInfo((string)context.Parameters["ImagePath"]);
            if (!sourceImageFile.Exists)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.FileNotFound, sourceImageFile.FullName);
                return;
            }

            // Grab all created files to print
            string imagePath = (string)context.Parameters["ImagePath"];

            foreach (string queueName in SettingsManager.Instance.GetSetting("AlarmSourcePrinterJob", "PrintingQueueNames").GetStringArray())
            {
                PrintingQueue pq = PrintingQueueManager.GetInstance().GetPrintingQueue(queueName);
                if (pq == null || !pq.IsEnabled)
                {
                    continue;
                }

                PrintFaxTask task = new PrintFaxTask();
                task.ImagePath = imagePath;
                task.Print(pq);
            }
        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            // TODO: Job phase could be "surfaced" as well?
            if (context.Phase != JobPhase.OnOperationSurfaced)
            {
                return;
            }

            // TODO: May be made modular using a plug-in system and/or delegates in the future!
            switch (context.AlarmSourceName)
            {
                case "FaxAlarmSource": PrintFaxes(context, operation); break;
                default:
                    Logger.Instance.LogFormat(LogType.Trace, this, Resources.NoPrintingPossible);
                    break;
            }
        }

        bool IJob.Initialize()
        {
            return true;
        }

        bool IJob.IsAsync
        {
            get { return true; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion

        #region Nested types

        class PrintFaxTask
        {
            private IList<Image> _pages;

            internal string ImagePath { private get; set; }

            internal void Print(PrintingQueue queue)
            {
                _pages = SplitMultipageTiff(ImagePath);

                ThreadPool.QueueUserWorkItem(w => GdiPrinter.Print(queue, GdiPrinterPrintAction));
            }

            private bool GdiPrinterPrintAction(int pageIndex, Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings, ref object state)
            {
                graphics.DrawImage(_pages[pageIndex - 1], Point.Empty);

                return pageIndex < _pages.Count;
            }

            private static IList<Image> SplitMultipageTiff(string tiffFileName)
            {
                List<Image> images = new List<Image>();

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
                            images.Add((Image)tiffImage.Clone());
                        }
                    }
                }

                return images;
            }

        }

        #endregion
    }
}