using System;
using System.Collections.Generic;
using System.IO;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using System.Reflection;

namespace AlarmWorkflow.Parser.IlsAnsbachParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILS Ansbach.
    /// </summary>
    public class IlsAnsbachParser : IParser
    {
        #region Fields

        private List<string> _keywords;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the IlsAnsbachParser class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public IlsAnsbachParser()
        {
            //LoadKeywordsFile();
        }

        #endregion

        #region Methods

        private void LoadKeywordsFile()
        {
            System.Diagnostics.Debugger.Break();

            string kwdFile = Path.Combine(Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly()), "Config\\IlsAnsbachFaxKeywords.lst");
            if (!File.Exists(kwdFile))
            {
                throw new FileNotFoundException("Could not load keywords file!", kwdFile);
            }

            _keywords = new List<string>();
            foreach (string line in File.ReadAllLines(kwdFile))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string keyword = line.ToUpperInvariant().Trim();
                if (!_keywords.Contains(keyword))
                {
                    _keywords.Add(keyword);
                }
            }
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            // TODO
            try
            {
                CurrentSection section = CurrentSection.AHeader;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    // Switch sections. The parsing may differ in each section.
                    switch (line.Trim())
                    {
                        case "MITTEILER": { section = CurrentSection.BMitteiler; continue; }
                        case "EINSATZORT": { section = CurrentSection.CEinsatzort; continue; }
                        case "ZIELORT": { section = CurrentSection.DZielort; continue; }
                        case "EINSATZGRUND": { section = CurrentSection.EEinsatzgrund; continue; }
                        case "EINSATZMITTEL": { section = CurrentSection.FEinsatzmittel; continue; }
                        case "BEMERKUNG": { section = CurrentSection.GBemerkung; continue; }
                        case "ENDE FAX": { section = CurrentSection.HFooter; continue; }
                        default: break;
                    }

                    string msg;
                    string prefix;
                    int x = line.IndexOf(':');
                    if (x == -1)
                    {
                        // Sometimes the OCR-software will miss a colon and instead replace it with nothing... in this case check our keywords!

                        // If even the keyword-check fails, bail out
                        continue;
                    }

                    prefix = line.Substring(0, x);
                    msg = line.Substring(x + 1).Trim();

                    prefix = prefix.Trim().ToUpperInvariant();

                    switch (section)
                    {
                        case CurrentSection.AHeader:
                            {
                                switch (prefix)
                                {
                                    case "EINSATZNUMMER":
                                        operation.OperationNumber = msg;
                                        break;
                                    default:
                                        continue;
                                }
                            }
                            break;
                        case CurrentSection.BMitteiler:
                            break;
                        case CurrentSection.CEinsatzort:
                            break;
                        case CurrentSection.DZielort:
                            break;
                        case CurrentSection.EEinsatzgrund:
                            break;
                        case CurrentSection.FEinsatzmittel:
                            break;
                        case CurrentSection.GBemerkung:
                            break;
                        case CurrentSection.HFooter:
                            break;
                        default:
                            break;
                    }


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
