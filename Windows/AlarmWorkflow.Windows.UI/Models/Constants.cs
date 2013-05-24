using System;

namespace AlarmWorkflow.Windows.UI.Models
{
    static class Constants
    {
        internal const int OfpInterval = 2000;
        internal const int OfpMaxAge = (7 * 24 * 60);
        internal const bool OfpOnlyNonAcknowledged = true;
        internal const int OfpLimitAmount = 50;
    }
}
