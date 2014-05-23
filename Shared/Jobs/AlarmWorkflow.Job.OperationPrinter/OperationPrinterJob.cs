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
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Job.OperationPrinter.Properties;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Specialized.Printing;

namespace AlarmWorkflow.Job.OperationPrinter
{
    [Export("OperationPrinterJob", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class OperationPrinterJob : IJob
    {
        #region Fields

        private ISettingsServiceInternal _settings;

        // FIXME: This should not be an instance-variable. In worst case, it could be overwritten with a new operation, if it is fast enough.
        private Operation _operation;

        #endregion

        #region Methods

        private string GetTemplateFile()
        {
            string templateFile = _settings.GetSetting(SettingKeysJob.TemplateFile).GetValue<string>();
            if (!Path.IsPathRooted(templateFile))
            {
                templateFile = Path.Combine(Utilities.GetWorkingDirectory(), templateFile);
            }

            if (!File.Exists(templateFile))
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.OperationPrintTemplateNotFoundError, templateFile);
                return null;
            }

            return templateFile;
        }

        private PropertyLocation GetSourceLocation()
        {
            return new PropertyLocation()
            {
                Street = _settings.GetSetting(SettingKeys.FDStreet).GetValue<string>(),
                StreetNumber = _settings.GetSetting(SettingKeys.FDStreetNumber).GetValue<string>(),
                City = _settings.GetSetting(SettingKeys.FDCity).GetValue<string>(),
                ZipCode = _settings.GetSetting(SettingKeys.FDZipCode).GetValue<string>(),
            };
        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase == JobPhase.AfterOperationStored)
            {
                PrintOperation(operation);
            }
        }

        private void PrintOperation(Operation operation)
        {
            foreach (string queueName in _settings.GetSetting(SettingKeysJob.PrintingQueueNames).GetStringArray())
            {
                PrintingQueuesConfiguration queues = _settings.GetSetting(SettingKeys.PrintingQueuesConfiguration).GetValue<PrintingQueuesConfiguration>();

                PrintingQueue pq = queues.GetPrintingQueue(queueName);
                if (pq == null || !pq.IsEnabled)
                {
                    continue;
                }

                _operation = operation;
                PrintWithQueue(pq, _operation);
            }
        }

        private void PrintWithQueue(PrintingQueue pq, Operation operation)
        {
            Thread printThread = new Thread(() => GdiPrinter.Print(pq, GdiPrinterPrintAction));
            // STA needed because of the use of WebBrowser (WinForms).
            printThread.SetApartmentState(ApartmentState.STA);
            printThread.Start();
            // Intentionally synchronize thread until its done.
            printThread.Join();
        }

        private bool GdiPrinterPrintAction(int pageIndex, Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings, ref object state)
        {
            pageSettings.Landscape = false;

            Image renderedImage = state as Image;
            if (renderedImage == null)
            {
                // Store the whole rendered image and share it across multiple pages.
                Size renderBounds = new Size(pageBounds.Width, 0);

                string templateFile = GetTemplateFile();
                if (templateFile == null)
                {
                    return false;
                }

                renderedImage = TemplateRenderer.RenderOperation(GetSourceLocation(), _operation, templateFile, renderBounds);
                state = renderedImage;
            }

            // Calculate the source rectangle (the portion of the rendered image) depending on which page we are in.
            int pagesNeeded = (int)Math.Ceiling((double)renderedImage.Height / (double)pageBounds.Size.Height);
            Rectangle destRect = pageBounds;
            Rectangle srcRect = new Rectangle(0, pageBounds.Height * (pageIndex - 1), pageBounds.Width, pageBounds.Height);

            graphics.DrawImage(renderedImage, destRect, srcRect, GraphicsUnit.Pixel);

            return pageIndex < pagesNeeded;
        }

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();

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
            _settings = null;
        }

        #endregion
    }
}