using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Website.Reports.Areas.Reporting.Models;
using AlarmWorkflow.Website.Reports.Models;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Website.Reports.Areas.Reporting.Controllers
{
    public class AlarmsController : Controller
    {
        /// <summary>
        /// GET: /Reporting/Alarms/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(GetAlarms());
        }

        private IEnumerable<Operation> GetAlarms()
        {
            using (var service = InternalServiceProxy.GetServiceInstance())
            {
                foreach (int id in service.Instance.GetOperationIds(0, false, 0))
                {
                    OperationItem oi = service.Instance.GetOperationById(id);
                    yield return oi.ToOperation();
                }
                yield break;
            }
        }

        /// <summary>
        /// GET: /Reporting/Alarms/Details/
        /// </summary>
        /// <param name="id">Id of the operation.</param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            using (var service = InternalServiceProxy.GetServiceInstance())
            {
                return View(service.Instance.GetOperationById(id).ToOperation());
            }
        }

        /// <summary>
        /// GET: /Reporting/Alarms/Export/
        /// </summary>
        /// <param name="id">Id of the operation.</param>
        /// <returns></returns>
        public ActionResult Export(int id)
        {
            using (var service = InternalServiceProxy.GetServiceInstance())
            {
                Operation operation = service.Instance.GetOperationById(id).ToOperation();
                Stream stream = ExportUtilities.ExportOperation(operation);

                FileStreamResult result = new FileStreamResult(stream, "text/xml");
                result.FileDownloadName = string.Format("{0}.xml", id);
                return result;
            }
        }
    }
}
