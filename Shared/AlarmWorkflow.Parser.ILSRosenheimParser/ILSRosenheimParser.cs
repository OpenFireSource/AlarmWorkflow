using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Parser.ILSRosenheimParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILS Rosenheim.
    /// </summary>
    [Export("ILSRosenheimParser", typeof(IFaxParser))]
    public class ILSRosenheimParser : IFaxParser
    {
        #region Fields
        private readonly string[] _keywords = new[]
                                                        {
                                                            "Einsatz-Nr.","Name","Straße","Abschnitt",
                                                            "Ortsteil","Kreuzung","Objekt","Schlagw.",
                                                            "Stichwort","Priorität","Alarmiert","gef. Gerät"
                                                        };

        private readonly Dictionary<String,String> _fdUnits;

        #endregion

        #region Constructor

        public ILSRosenheimParser()
        {
            _fdUnits = new Dictionary<string, string>();
            string[] units = SettingsManager.Instance.GetSetting("Shared", "FD.Units").GetStringArray();
            foreach (string unit in units)
            {
                string[] result = unit.Split(new[] {"=;="}, StringSplitOptions.None);
                if (result.Length == 2)
                {
                    _fdUnits.Add(result[0],result[1]);
                }
                else
                {
                    _fdUnits.Add(unit,unit);
                }
            }
        }

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

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
                    // Switch sections. The parsing may differ in each section.
                    switch (line.Trim())
                    {
                        case "MITTEILER": { section = CurrentSection.BMitteiler; continue; }
                        case "EINSATZORT": { section = CurrentSection.CEinsatzort; continue; }
                        case "EINSATZGRUND": { section = CurrentSection.DEinsatzgrund; continue; }
                        case "EINSATZMITTEL": { section = CurrentSection.EEinsatzmittel; continue; }
                        case "BEMERKUNG": { section = CurrentSection.FBemerkung; keywordsOnly = false; continue; }
                        case "ENDE ALARMFAX — V2.0": { section = CurrentSection.GFooter; keywordsOnly = false; continue; }
                    }

                    string msg = line;
                    string prefix = "";

                    // Make the keyword check - or not (depends on the section we are in; see above)
                    string keyword = "";
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
                                    case "EINSATZ-NR.":
                                        operation.OperationNumber = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.BMitteiler:
                            operation.Messenger = line.Remove(0, keyword.Length).Trim();
                            break;
                        case CurrentSection.CEinsatzort:
                            {
                                switch (prefix)
                                {
                                    case "STRAßE":
                                        {
                                            operation.Einsatzort.Street = msg;
                                            int empty = msg.LastIndexOf(" ", StringComparison.Ordinal);
                                            if (empty != -1 && empty != msg.Length)
                                            {
                                                operation.Einsatzort.Street = msg.Substring(0, empty).Trim();
                                                operation.Einsatzort.StreetNumber = msg.Substring(empty).Trim();
                                            }

                                        }
                                        break;
                                    case "ORTSTEIL":
                                        {
                                            operation.Einsatzort.City = msg;
                                            // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                            // However we can (at least with google maps) omit this information without problems!
                                            int dashIndex = msg.IndexOf('-');
                                            if (dashIndex != -1)
                                            {
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex);
                                            }
                                        }
                                        break;
                                    case "OBJEKT":
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "STICHWORT":
                                        operation.Keywords.EmergencyKeyword = msg;
                                        break;
                                    case "PRIORITÄT":
                                        operation.Priority = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzmittel:
                            {
                                if (line.StartsWith("NAME", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    msg = GetMessageText(line, "NAME");
                                    last.FullName = msg;
                                }
                                else if (line.StartsWith("ALARMIERT", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(msg))
                                {
                                    msg = GetMessageText(line, "ALARMIERT");

                                    // In case that parsing the time failed, we just assume that the resource got requested right away.
                                    DateTime dt;
                                    // Most of the time the OCR-software reads the colon as a "1", so we check this case right here.
                                    if (!DateTime.TryParseExact(msg, "dd.MM.yyyy HH1mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                    {
                                        // If this is NOT the case and it was parsed correctly, try it here
                                        DateTime.TryParseExact(msg, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                                    }

                                    last.Timestamp = dt.ToString(CultureInfo.InvariantCulture);
                                }
                                else if (line.StartsWith("GEF. GERÄT", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    msg = GetMessageText(line, "GEF. GERÄT");

                                    // Only add to requested equipment if there is some text,
                                    // otherwise the whole vehicle is the requested equipment
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        last.RequestedEquipment.Add(msg);
                                    }
                                    
                                    foreach (KeyValuePair<string, string> fdUnit in _fdUnits)
                                    {
                                        if (last.FullName.ToLower().Contains(fdUnit.Key.ToLower()))
                                        {
                                            operation.OperationPlan += " - " + fdUnit.Value;
                                            break;
                                        }
                                    }
                                    operation.Resources.Add(last);
                                    last = new OperationResource();
                                }
                            }
                            break;
                        case CurrentSection.FBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment += msg + Environment.NewLine;
                            }
                            break;
                        case CurrentSection.GFooter:
                            // The footer can be ignored completely.
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

        #region Methods
        private bool StartsWithKeyword(string line, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in _keywords.Where(kwd => line.ToLowerInvariant().StartsWith(kwd.ToLowerInvariant())))
            {
                keyword = kwd;
                return true;
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
                line = line.Remove(0, prefix.Length);
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
                line = line.Remove(0, 1);
            }
            line = line.Trim();
            return line;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BMitteiler,
            CEinsatzort,
            DEinsatzgrund,
            EEinsatzmittel,
            FBemerkung,
            GFooter
        }

        #endregion
    }

}
