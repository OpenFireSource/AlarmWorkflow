using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.NoParser
{
    [Export("NoParser", typeof(IFaxParser))]
    sealed class NoParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParser"/> class.
        /// Calling this constructor will log a warning stating that usage is discouraged.
        /// </summary>
        public NoParser()
        {
            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.UsageWarning);
        }

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            return operation;
        }

        #endregion

    }
}
