using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using System;
using System.Collections.Generic;

namespace AlarmWorkflow.Parser.IlsAnsbachParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILS Ansbach.
    /// </summary>
    public class IlsAnsbachParser : IParser
    {
        #region Fields

        /// <summary>
        /// Dictionary that contains text parsers which differ in each section. This is due to the fact that the ILS-Ansbach-faxes are a little complex
        /// and in order to maintain readability.
        /// </summary>
        private Dictionary<CurrentSection, Func<string, bool>> _sectionParsers;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the IlsAnsbachParser class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public IlsAnsbachParser()
        {
            // Set up section parsers
            _sectionParsers = new Dictionary<CurrentSection, Func<string, bool>>();
            _sectionParsers[CurrentSection.AHeader] = HeaderParser;
        }

        #endregion

        #region Methods

        private bool HeaderParser(string line)
        {
            return true;
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            // TODO
            try
            {
                for (int i = 0; i < lines.Length; i++)
                {


                }
                CurrentSection section = CurrentSection.AHeader;
                foreach (string line in lines)
                {
                    string msg;
                    string prefix;
                    int x = line.IndexOf(':');
                    if (x == -1)
                    {
                        continue;
                    }

                    prefix = line.Substring(0, x);
                    msg = line.Substring(x + 1).Trim();

                    prefix = prefix.Trim().ToUpperInvariant();

                }
            }
            catch (System.Exception ex)
            {

            }

            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BMitteiler,
            CEinsatzort,
            DZielort,
            EEinsatzgrund,
            FEinsatzmittel,
            GBemerkung,
            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            HFooter,
        }

        #endregion

    }
}
