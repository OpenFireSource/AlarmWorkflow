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
using System.IO;
using System.Net;
using System.Net.Mime;
using System.ServiceModel;
using System.Web.Mvc;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Website.Reports.Areas.Reporting.Models;
using AlarmWorkflow.Website.Reports.Filters;

namespace AlarmWorkflow.Website.Reports.Areas.Reporting.Controllers
{
    public class AlarmsController : Controller
    {
        /// <summary>
        /// GET: /Reporting/Alarms/
        /// </summary>
        /// <returns></returns>
        [CustomHandleError()]
        public ActionResult Index()
        {
            return View(GetAlarms());
        }

        private IEnumerable<Operation> GetAlarms()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
            {
                foreach (int id in service.Instance.GetOperationIds(0, false, 0))
                {
                    yield return service.Instance.GetOperationById(id);
                }
                yield break;
            }
        }

        /// <summary>
        /// GET: /Reporting/Alarms/Details/
        /// </summary>
        /// <param name="id">Id of the operation.</param>
        /// <returns></returns>
        [CustomHandleError()]
        public ActionResult Details(int id)
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
            {
                return View(service.Instance.GetOperationById(id));
            }
        }

        /// <summary>
        /// GET: /Reporting/Alarms/Export/
        /// </summary>
        /// <param name="id">Id of the operation.</param>
        /// <returns></returns>
        public ActionResult Export(int id)
        {
            try
            {
                using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
                {
                    Operation operation = service.Instance.GetOperationById(id);
                    Stream stream = ExportUtilities.ExportOperation(operation);

                    FileStreamResult result = new FileStreamResult(stream, MediaTypeNames.Text.Xml);
                    result.FileDownloadName = string.Format("{0}.xml", id);
                    return result;
                }
            }
            catch (EndpointNotFoundException)
            {
                // Silently ignore this exception.
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }
    }
}
