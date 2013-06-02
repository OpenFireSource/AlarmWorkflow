using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("NoParser", typeof(IParser))]
    sealed class NoParser : IParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParser"/> class.
        /// Calling this constructor will log a warning stating that usage is discouraged.
        /// </summary>
        public NoParser()
        {
            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.NoParserUsageWarning);
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            return operation;
        }

        #endregion

    }
}
