using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Website.Reports.Areas.Display.Models;
using AlarmWorkflow.Website.Reports.Models;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Controllers
{
    public class AlarmController : Controller
    {
        /// <summary>
        /// GET: /Display/Alarm/Index
        /// GET: /Display/Alarm/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Display/Alarm/GetLatestOperation
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLatestOperation()
        {
            GetLastOperationData data = new GetLastOperationData();
            try
            {
                using (var service = InternalServiceProxy.GetServiceInstance())
                {
                    IList<int> ids = service.Instance.GetOperationIds(0, true, 1);
                    if (ids.Count == 1)
                    {
                        OperationItem item = service.Instance.GetOperationById(ids[0]);
                        data.success = true;
                        data.op = item;
                    }
                    else if (ids.Count == 0)
                    {
                        data.success = true;
                        data.op = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // It's ok when an exception is thrown here. We catch it, log it, and the View considers it as an error (success is false).
                Logger.Instance.LogException(this, ex);
            }

            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = data;
            return result;
        }
    }
}
