// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsAnsbachParser", typeof(IParser))]
    sealed class IlsAnsbachParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[] { 
            "ABSENDER", "FAX", "TERMIN", "EINSATZNUMMER", "NAME", "STRAßE", "ORT", "OBJEKT", "PLANNUMMER", 
            "STATION", "STRAßE", "ORT", "OBJEKT", "STATION", "SCHLAGW", "STICHWORT", "PRIO", 
            "EINSATZMITTEL", "ALARMIERT", "GEFORDERTE AUSSTATTUNG" };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
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

                    // Try to parse the header and extract date and time if possible
                    operation.Timestamp = ParserUtility.ReadFaxTimestamp(line, operation.Timestamp);

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
                        if (!ParserUtility.StartsWithKeyword(line, Keywords, out keyword))
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
                                        operation.CustomData["Absender"] = msg;
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
                                                operation.Einsatzort.StreetNumber = streetNumber;
                                            }

                                            operation.Einsatzort.Street = msg.Substring(0, msg.IndexOf("Haus-")).Trim();
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            operation.Einsatzort.ZipCode = ParserUtility.ReadZipCodeFromCity(msg);
                                            if (string.IsNullOrWhiteSpace(operation.Einsatzort.ZipCode))
                                            {
                                                Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", operation.Einsatzort.City);
                                            }

                                            operation.Einsatzort.City = msg.Remove(0, operation.Einsatzort.ZipCode.Length).Trim();

                                            // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                            // However we can (at least with google maps) omit this information without problems!
                                            int dashIndex = operation.Einsatzort.City.IndexOf('-');
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
                                    case "PLANNUMMER":
                                        operation.CustomData["Einsatzort Plannummer"] = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Einsatzort Station"] = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DZielort:
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
                                                operation.Zielort.StreetNumber = msg.Remove(0, streetNumberColonIndex + 1).Trim();
                                            }

                                            operation.Zielort.StreetNumber = msg.Substring(0, msg.IndexOf("Haus-")).Trim();
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            string plz = ParserUtility.ReadZipCodeFromCity(msg);
                                            operation.Zielort.ZipCode = plz;
                                            operation.Zielort.City = msg.Remove(0, plz.Length).Trim();
                                        }
                                        break;
                                    case "OBJEKT":
                                        operation.Zielort.Property = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Zielort Station"] = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "STICHWORT B":
                                        operation.Keywords.B = msg;
                                        break;
                                    case "STICHWORT R":
                                        operation.Keywords.R = msg;
                                        break;
                                    case "STICHWORT S":
                                        operation.Keywords.S = msg;
                                        break;
                                    case "STICHWORT T":
                                        operation.Keywords.T = msg;
                                        break;
                                    case "PRIO.":
                                        operation.Priority = msg;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                if (line.StartsWith("EINSATZMITTEL", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    msg = ParserUtility.GetMessageText(line, "EINSATZMITTEL");
                                    last.FullName = msg;
                                }
                                else if (line.StartsWith("ALARMIERT", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(msg))
                                {
                                    msg = ParserUtility.GetMessageText(line, "Alarmiert");

                                    DateTime dt = ParserUtility.TryGetTimestampFromMessage(msg, operation.Timestamp);
                                    last.Timestamp = dt.ToString();
                                }
                                else if (line.StartsWith("GEFORDERTE AUSSTATTUNG", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    msg = ParserUtility.GetMessageText(line, "Geforderte Ausstattung");

                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        last.RequestedEquipment.Add(msg);
                                    }

                                    operation.Resources.Add(last);
                                    last = new OperationResource();
                                }
                            }
                            break;
                        case CurrentSection.GBemerkung:
                            {
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

            operation.Comment = ParserUtility.RemoveTrailingNewline(operation.Comment);

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
