using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

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
            LoadKeywordsFile();
        }

        #endregion

        #region Methods

        private void LoadKeywordsFile()
        {
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

        private DateTime ReadFaxTimestamp(string line, DateTime fallback)
        {
            DateTime date = fallback;
            TimeSpan timestamp = date.TimeOfDay;

            Match dt = Regex.Match(line, @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d");
            Match ts = Regex.Match(line, @"([01]?[0-9]|2[0-3]):[0-5][0-9]");
            if (dt.Success)
            {
                DateTime.TryParse(dt.Value, out date);
            }
            if (ts.Success)
            {
                TimeSpan.TryParse(ts.Value, out timestamp);
            }

            return new DateTime(date.Year, date.Month, date.Day, timestamp.Hours, timestamp.Minutes, timestamp.Seconds, timestamp.Milliseconds, DateTimeKind.Local);
        }

        private bool StartsWithKeyword(string line, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in _keywords)
            {
                if (line.StartsWith(kwd))
                {
                    keyword = kwd;
                    return true;
                }
            }
            keyword = null;
            return false;
        }

        /// <summary>
        /// Returns the message text, which is the line text but excluding the keyword/prefix and a possible colon.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="prefix">The prefix that is to be removed (optional).</param>
        /// <returns></returns>
        private string GetMessageText(string line, string prefix)
        {
            if (prefix == null)
            {
                prefix = "";
            }

            if (prefix.Length > 0)
            {
                line = line.Remove(0, prefix.Length).Trim();
            }
            else
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    line = line.Remove(0, colonIndex + 1);
                }
            }

            if (line.StartsWith(":"))
            {
                line = line.Remove(0, 1).Trim();
            }

            return line;
        }

        /// <summary>
        /// Attempts to read the zip code from the city, if available.
        /// </summary>
        /// <param name="cityText"></param>
        /// <returns>The zip code of the city. -or- null, if there was no.</returns>
        private string ReadZipCodeFromCity(string cityText)
        {
            string zipCode = "";
            foreach (char c in cityText)
            {
                if (char.IsNumber(c))
                {
                    zipCode += c;
                    continue;
                }
                break;
            }
            return zipCode;
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            lines = Utilities.Trim(lines);

            CurrentSection section = CurrentSection.AHeader;
            bool keywordsOnly = true;
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    // Try to parse the header and extract date and time if possible
                    operation.Timestamp = ReadFaxTimestamp(line, operation.Timestamp);

                    // Switch sections. The parsing may differ in each section.
                    switch (line.Trim())
                    {
                        case "MITTEILER": { section = CurrentSection.BMitteiler; continue; }
                        case "EINSATZORT": { section = CurrentSection.CEinsatzort; continue; }
                        case "ZIELORT": { section = CurrentSection.DZielort; continue; }
                        case "EINSATZGRUND": { section = CurrentSection.EEinsatzgrund; continue; }
                        case "EINSATZMITTEL": { section = CurrentSection.FEinsatzmittel; continue; }
                        case "BEMERKUNG": { section = CurrentSection.GBemerkung; keywordsOnly = false; continue; }
                        case "ENDE FAX": { section = CurrentSection.HFooter; keywordsOnly = false; continue; }
                        default: break;
                    }

                    string msg = line;
                    string prefix = "";

                    // Make the keyword check - or not (depends on the section we are in; see above)
                    string keyword = null;
                    if (keywordsOnly)
                    {
                        if (!StartsWithKeyword(line, out keyword))
                        {
                            continue;
                        }

                        int x = line.IndexOf(':');
                        if (x == -1)
                        {
                            // If there is no colon found (may happen occasionally) then simply remove the length of the keyword from the beginning
                            prefix = keyword;
                            msg = line.Remove(0, prefix.Length).Trim();
                        }
                        else
                        {
                            prefix = line.Substring(0, x);
                            msg = line.Substring(x + 1).Trim();
                        }

                        prefix = prefix.Trim().ToUpperInvariant();
                    }

                    // Parse each section
                    switch (section)
                    {
                        case CurrentSection.AHeader:
                            {
                                switch (prefix)
                                {
                                    case "ABSENDER":
                                        operation.CustomData["Sender"] = msg;
                                        break;
                                    case "TERMIN":
                                        operation.CustomData["Termin"] = msg;
                                        break;
                                    case "EINSATZNUMMER":
                                        operation.OperationNumber = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.BMitteiler:
                            {
                                // This switch would not be necessary in this section (there is only "Name")...
                                switch (prefix)
                                {
                                    case "NAME":
                                        operation.Messenger = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.CEinsatzort:
                            {
                                switch (prefix)
                                {
                                    case "STRAßE":
                                        {
                                            // The street here is mangled together with the street number. Dissect them...
                                            int streetNumberColonIndex = msg.LastIndexOf(':');
                                            if (streetNumberColonIndex != -1)
                                            {
                                                // We need to check for occurrence of the colon, because it may have been omitted by the OCR-software
                                                string streetNumber = msg.Remove(0, streetNumberColonIndex + 1).Trim();
                                                operation.StreetNumber = streetNumber;
                                            }

                                            operation.Street = msg.Substring(0, msg.IndexOf("Haus-")).Trim();
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            operation.ZipCode = ReadZipCodeFromCity(msg);
                                            operation.City = msg.Remove(0, operation.ZipCode.Length).Trim();
                                        }
                                        break;
                                    case "OBJEKT":
                                        {
                                            operation.Property = msg;
                                        } break;
                                    case "PLANNUMMER":
                                        {
                                            // TODO
                                        } break;
                                    case "STATION":
                                        {
                                            // TODO
                                        } break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DZielort:
                            // TODO: In my faxes, the fields "Straße", "Haus-Nr", "Ort", "Objekt" and "Station" were always empty. Do we need to parse them?
                            break;
                        case CurrentSection.EEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        operation.Keyword = msg;
                                        break;
                                    case "STICHWORT B":
                                        operation.CustomData["StichwortB"] = msg;
                                        break;
                                    case "STICHWORT R":
                                        operation.CustomData["StichwortR"] = msg;
                                        break;
                                    case "STICHWORT S":
                                        operation.CustomData["StichwortS"] = msg;
                                        break;
                                    case "STICHWORT T":
                                        operation.CustomData["StichwortT"] = msg;
                                        break;
                                    case "PRIO.":
                                        operation.CustomData["Priority"] = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                // Create sub-parser for this section
                                List<Resource> resources = new List<Resource>();
                                Resource last = new Resource();

                                bool finished = false;
                                while (!finished)
                                {
                                    // Execute within security (no exceptions)
                                    Utilities.Swallow<object>(o =>
                                    {
                                        string fel = lines[i];
                                        if (fel.StartsWith("Einsatzmittel"))
                                        {
                                            msg = GetMessageText(fel, "Einsatzmittel");
                                            last.Einsatzmittel = msg;
                                        }
                                        else if (fel.StartsWith("Alarmiert") && !string.IsNullOrEmpty(msg))
                                        {
                                            msg = GetMessageText(fel, "Alarmiert");

                                            DateTime dt;
                                            // Most of the time the OCR-software reads the colon as a "1", so we check this case right here.
                                            if (!DateTime.TryParseExact(msg, "dd.MM.yyyy HH1mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                            {
                                                // If this is NOT the case and it was parsed correctly, try it here
                                                DateTime.TryParseExact(msg, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                                            }

                                            last.Alarmiert = dt.ToString();
                                        }
                                        else if (fel.StartsWith("Geforderte Ausstattung"))
                                        {
                                            msg = GetMessageText(fel, "Geforderte Ausstattung");

                                            last.GeforderteAusstattung = msg;

                                            // This line will end the construction of this resource. Add it to the list and go to the next.
                                            resources.Add(last);
                                            last = new Resource();
                                        }
                                        else if (fel.StartsWith("Bemerkung", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            operation.CustomData["Einsatzmittel"] = resources;
                                            finished = true;
                                            // Decrement index variable here because it gets incremented later
                                            i -= 2;
                                        }
                                    }, null);

                                    i++;
                                }
                            }
                            break;
                        case CurrentSection.GBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment += msg + "\n";
                            }
                            break;
                        case CurrentSection.HFooter:
                            // The footer can be ignored completely.
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
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
