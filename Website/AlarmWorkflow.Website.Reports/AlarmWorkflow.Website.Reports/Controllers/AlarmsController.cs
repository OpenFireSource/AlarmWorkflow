using System.Collections.Generic;
using System.Web.Mvc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Website.Reports.Models;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Website.Reports.Controllers
{
    public class AlarmsController : Controller
    {
        /// <summary>
        /// GET: /Alarms/
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
        /// GET: /Alarms/Details/
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
    }
}
