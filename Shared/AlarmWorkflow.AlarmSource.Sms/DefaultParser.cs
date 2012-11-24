using System;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("DefaultParser", typeof(ISmsParser))]
    class DefaultParser : ISmsParser
    {
        #region ISmsParser Members

        Operation ISmsParser.Parse(string smsText)
        {
            Operation operation = new Operation();
            operation.Timestamp = DateTime.Now;
            operation.Comment = smsText;

            return operation;
        }

        #endregion
    }
}
