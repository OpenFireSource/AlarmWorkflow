﻿// This file is part of AlarmWorkflow.
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
    [Export("ILSNordoberpfalzParser", typeof(IParser))]
    sealed class ILSNordoberpfalzParser : IParser
    {

        #region Constants

        private static readonly string[] Keywords = new[] { 
              "EINSATZNUMMER", "NAME", "STRAßE","ORT", "OBJEKT", 
            "STATION", "SCHLAGW.","STICHWORT","PRIO.","ALARMIERT", "RUFNUMMER" };

        #endregion

        #region Methods

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {
            //MI TTE I LER must be considered when using tesseract because of recognition problems.
            if (line.Contains("MITTEILER") || line.Contains("M I TTE I LER"))
            {
                section = CurrentSection.BMitteiler;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.CEinsatzort;
                //Is not true but only works that way
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZGRUND"))
            {
                section = CurrentSection.EEinsatzgrund;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.FEinsatzmittel;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("BEMERKUNG") || line.Contains("OBJEKTINFORMATION"))
            {
                section = CurrentSection.GBemerkung;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("ENDE FAX"))
            {
                section = CurrentSection.HFooter;
                keywordsOnly = false;
                return true;
            }
            return false;
        }
        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);

            CurrentSection section = CurrentSection.AHeader;
            bool keywordsOnly = true;

            InnerSection innerSection = InnerSection.AStraße;
            for (int i = 0; i < lines.Length - 5; i++)
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


                    if (GetSection(line.Trim(), ref section, ref keywordsOnly))
                    {
                        continue;
                    }

                    string msg = line;
                    string prefix = "";

                    // Make the keyword check - or not (depends on the section we are in; see above)
                    if (keywordsOnly)
                    {
                        string keyword;
                        if (!ParserUtility.StartsWithKeyword(line, Keywords, out keyword))
                        {
                            continue;
                        }

                        int x = line.IndexOf(':');
                        int y = line.IndexOf('=');
                        if (x == -1 && y == -1)
                        {
                            // If there is no colon found (may happen occasionally) then simply remove the length of the keyword from the beginning
                            prefix = keyword;
                            msg = line.Remove(0, prefix.Length).Trim();
                        }
                        else if (y != -1 && x == -1 || (y != -1 && x != -1 && y < x))
                        {
                            prefix = line.Substring(0, y);
                            msg = line.Substring(y + 1).Trim();
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
                                    case "EINSATZNUMMER":
                                        operation.OperationNumber = msg;
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
                                    case "RUFNUMMER":
                                        operation.Messenger = operation.Messenger.AppendLine(string.Format("Nr.: {0}", msg));
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
                                            innerSection = InnerSection.AStraße;
                                            string street, streetNumber, appendix;
                                            ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                            operation.CustomData["Einsatzort Zusatz"] = appendix;
                                            operation.Einsatzort.Street = street;
                                            operation.Einsatzort.StreetNumber = streetNumber;
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            innerSection = InnerSection.BOrt;
                                            operation.Einsatzort.ZipCode = ParserUtility.ReadZipCodeFromCity(msg);
                                            if (string.IsNullOrWhiteSpace(operation.Einsatzort.ZipCode))
                                            {
                                                Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", operation.Einsatzort.City);
                                            }

                                            operation.Einsatzort.City = msg.Remove(0, operation.Einsatzort.ZipCode.Length).Trim();

                                            // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                            // However we can (at least with google maps) omit this information without problems!
                                            int dashIndex = operation.Einsatzort.City.IndexOf(" - ");
                                            if (dashIndex != -1)
                                            {
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex).Trim();
                                            }
                                        }
                                        break;
                                    case "OBJEKT":
                                        innerSection = InnerSection.CObjekt;
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "STATION":
                                        innerSection = InnerSection.DStation;
                                        operation.CustomData["Einsatzort Station"] = msg;
                                        break;
                                    default:
                                        switch (innerSection)
                                        {
                                            case InnerSection.AStraße:
                                                //Quite dirty because of Streetnumber. Looking for better solution
                                                operation.Einsatzort.Street += msg;
                                                break;
                                            case InnerSection.BOrt:
                                                operation.Einsatzort.City += msg;
                                                break;
                                            case InnerSection.CObjekt:
                                                operation.Einsatzort.Property += msg;
                                                break;
                                            case InnerSection.DStation:
                                                operation.CustomData["Einsatzort Station"] += msg;
                                                break;
                                        }
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
                                    case "STICHWORT":
                                        operation.Keywords.EmergencyKeyword = msg;
                                        break;
                                    case "PRIO.":
                                        operation.Priority = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                switch (prefix)
                                {
                                    case "NAME":
                                        last.FullName = msg.Trim();
                                        break;
                                    case "ALARMIERT":
                                        // Only add to requested equipment if there is some text,
                                        // otherwise the whole vehicle is the requested equipment
                                        if (!string.IsNullOrWhiteSpace(msg))
                                        {
                                            last.Timestamp = ParserUtility.TryGetTimestampFromMessage(msg, DateTime.Now).ToString();
                                        }
                                        operation.Resources.Add(last);

                                        last = new OperationResource();
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.GBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment.AppendLine(msg);
                            }
                            break;
                        case CurrentSection.HFooter:
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

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BMitteiler,
            CEinsatzort,
            EEinsatzgrund,
            FEinsatzmittel,
            GBemerkung,
            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            HFooter,
        }
        private enum InnerSection
        {
            AStraße,
            BOrt,
            CObjekt,
            DStation,
        }
        #endregion


    }
}
