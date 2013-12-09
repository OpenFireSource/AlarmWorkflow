using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlarmWorkflow.Windows.ServiceContracts;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Models
{
    public class GetLastOperationData
    {
        public bool success { get; set; }
        public OperationItem op { get; set; }
    }
}