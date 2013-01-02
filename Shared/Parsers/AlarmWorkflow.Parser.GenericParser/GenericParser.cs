using System.IO;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Parser.GenericParser.Control;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Parser.GenericParser.Parsing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser
{
    /// <summary>
    /// Represents the logic of a parser that parses faxes based on simple rules for simple faxes.
    /// </summary>
    [Export("GenericParser", typeof(IFaxParser))]
    class GenericParser : IFaxParser
    {
        #region Fields

        private Configuration _configuration;
        private ControlInformation _controlInformation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParser"/> class.
        /// </summary>
        public GenericParser()
        {
            // Print warning
            Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.GenericParserBetaWarningText);

            _configuration = new Configuration();
            LoadControlInformationFile();
        }

        #endregion

        #region Methods

        private void LoadControlInformationFile()
        {
            string fileName = _configuration.ControlFile;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(Utilities.GetWorkingDirectory(), fileName);
            }

            _controlInformation = ControlInformation.Load(fileName);
        }

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            lines = Utilities.Trim(lines);

            ParserInstance parser = new ParserInstance(_controlInformation);
            return parser.Parse(lines);
        }

        #endregion
    }
}
