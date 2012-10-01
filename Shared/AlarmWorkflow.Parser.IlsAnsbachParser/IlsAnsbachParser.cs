using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.IlsAnsbachParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILS Ansbach.
    /// </summary>
    public class IlsAnsbachParser : IParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the IlsAnsbachParser class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public IlsAnsbachParser()
        {
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] file)
        {
            Operation einsatz = new Operation();

            // TODO

            return einsatz;
        }

        #endregion

    }
}
