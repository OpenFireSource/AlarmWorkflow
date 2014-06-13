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
using System.IO;
using System.Text;
using System.Xml.Linq;
using AlarmWorkflow.BackendService.EngineContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.OperationFileExporter
{
    [Export("OperationFileExporter", typeof(IJob))]
    [Information(DisplayName = "ExportJobDisplayName", Description = "ExportJobDescription")]
    class OperationFileExporter : IJob
    {
        #region Fields

        private Configuration _configuration;

        #endregion

        #region Methods

        private void ExportCustomTextFile(Operation operation)
        {
            using (StreamWriter sw = new StreamWriter(_configuration.CustomTextDestinationFileName, false, Encoding.UTF8))
            {
                sw.Write(operation.ToString(_configuration.CustomTextFormat));
            }
        }

        private void ExportEvaFile(Operation operation)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("Einsatz"));
            doc.Root.Add(CreateXElementSafe("Ort", operation.Einsatzort.City));
            doc.Root.Add(CreateXElementSafe("postalCode", operation.Einsatzort.ZipCode));
            doc.Root.Add(CreateXElementSafe("Strasse", operation.Einsatzort.Street));
            doc.Root.Add(CreateXElementSafe("Hausnummer", operation.Einsatzort.StreetNumber));
            doc.Root.Add(CreateXElementSafe("Objekt", operation.Einsatzort.Property));
            doc.Root.Add(CreateXElementSafe("GK_X", operation.Einsatzort.GeoLongitude));
            doc.Root.Add(CreateXElementSafe("GK_Y", operation.Einsatzort.GeoLatitude));
            doc.Root.Add(CreateXElementSafe("Meldender", operation.Messenger));
            doc.Root.Add(CreateXElementSafe("Bemerkung", operation.Comment));
            doc.Root.Add(CreateXElementSafe("Zusatzinformationen", operation.Picture));
            doc.Root.Add(CreateXElementSafe("Stichwort", operation.Keywords.Keyword));
            doc.Root.Add(CreateXElementSafe("Zeit", operation.Timestamp.TimeOfDay.ToString()));
            doc.Root.Add(CreateXElementSafe("Datum", operation.Timestamp.ToShortDateString()));

            doc.Save(_configuration.EvaDestinationFileName);
        }

        private XElement CreateXElementSafe(string name, string value)
        {
            return new XElement(name, value ?? string.Empty);
        }

        #endregion

        #region IJob Members

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            if (_configuration.CustomTextExportEnabled)
            {
                try
                {
                    ExportCustomTextFile(operation);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ExportToCustomFailed);
                    Logger.Instance.LogException(this, ex);
                }
            }

            if (_configuration.EvaExportEnabled)
            {
                try
                {
                    ExportEvaFile(operation);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.ExportToEVAFailed);
                    Logger.Instance.LogException(this, ex);
                }
            }
        }

        bool IJob.Initialize(IServiceProvider serviceProvider)
        {
            _configuration = new Configuration(serviceProvider);
            return true;
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_configuration != null)
            {
                _configuration.Dispose();
                _configuration = null;
            }
        }

        #endregion
    }
}