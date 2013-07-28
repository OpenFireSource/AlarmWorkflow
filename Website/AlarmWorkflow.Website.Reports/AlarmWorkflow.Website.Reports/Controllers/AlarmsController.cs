using System.Collections.Generic;
using System.Web.Mvc;
using AlarmWorkflow.Website.Reports.Models;

namespace AlarmWorkflow.Website.Reports.Controllers
{
    public class AlarmsController : Controller
    {
        //
        // GET: /Alarms/

        public ActionResult Index()
        {


            return View(GetAlarms());
        }

        private IEnumerable<Alarm> GetAlarms()
        {
            yield break;
        }

    }
}
