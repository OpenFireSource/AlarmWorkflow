using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.AlarmSource.Sms
{
    [Export("DefaultSMSParser", typeof(IParser))]
    class DefaultSMSParser : IParser
    {
        #region ISmsParser Members

        Operation IParser.Parse(string[] lines)
        {
            string text = string.Join("", lines);
            Operation operation = new Operation {Timestamp = DateTime.Now, Comment = text};
            return operation;
        }

        #endregion
    }
}
