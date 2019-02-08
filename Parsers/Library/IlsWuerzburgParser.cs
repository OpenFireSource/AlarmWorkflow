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
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsWuerzburgParser", typeof(IParser))]
    sealed class IlsWuerzburgParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[]
            {
                "ABSENDER", "FAX", "TERMIN", "EINSATZ", "NAME", "STRAßE", "ORT", "ABSCHNITT", "OBJEKT", "PLANNUMMER",
                "STATION", "ZONE", "STRAßE", "ORT", "OBJEKT", "STATION", "ZONE", "EINSATZGRUND", "SCHLAGW", "PRIO",
                "EINSATZMITTEL", "NAME", "GERÄT", "ALARMIERT"
            };

        #endregion

        #region Methods

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {
            if (line.Contains("MITTEILER"))
            {
                section = CurrentSection.BMitteiler;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.CEinsatzort;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("ZIELORT"))
            {
                section = CurrentSection.DZielort;
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
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.GBemerkung;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("TEXTBAUSTEINE"))
            {
                section = CurrentSection.HTextbausteine;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("ENDE FAX"))
            {
                section = CurrentSection.IFooter;
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
                                    case "EINSATZ":
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
                                }
                            }
                            break;
                        case CurrentSection.CEinsatzort:
                            {
                                switch (prefix)
                                {
                                    case "STRAßE":
                                        {
                                            string street, streetNumber, appendix;
                                            ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                            operation.CustomData["Einsatzort Zusatz"] = appendix;
                                            operation.Einsatzort.Street = street;
                                            operation.Einsatzort.StreetNumber = streetNumber;
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
                                            int dashIndex = operation.Einsatzort.City.IndexOf(" - ");
                                            if (dashIndex != -1)
                                            {
                                                operation.CustomData["Einsatzort Kommune"] = operation.Einsatzort.City.Substring(dashIndex).Trim();
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex).Trim();
                                            }
                                        }
                                        break;
                                    case "ABSCHNITT":
                                        operation.Zielort.Intersection = msg;
                                        break;
                                    case "OBJEKT":
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "PLANNUMMER":
                                        operation.OperationPlan = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Einsatzort Station"] = msg;
                                        break;
                                    case "ZONE":
                                        operation.Einsatzort.Location = msg;
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
                                            string street, streetNumber, appendix;
                                            ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                            operation.CustomData["Zielort Zusatz"] = appendix;
                                            operation.Zielort.Street = street;
                                            operation.Zielort.StreetNumber = streetNumber;
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            string zipcode = ParserUtility.ReadZipCodeFromCity(msg);
                                            operation.Zielort.ZipCode = zipcode;
                                            operation.Zielort.City = msg.Remove(0, zipcode.Length).Trim();
                                        }
                                        break;
                                    case "OBJEKT":
                                        operation.Zielort.Property = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Zielort Station"] = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        Regex regex = new Regex("\\[(.*)]");

                                        if (regex.IsMatch(msg))
                                        {
                                            Match match = regex.Match(msg);
                                            operation.Keywords.EmergencyKeyword = match.Groups[1].Value;
                                            operation.Keywords.Keyword = msg.Replace(match.Value, "");
                                        }
                                        else
                                        {
                                            operation.Keywords.Keyword = msg;
                                        }

                                        break;
                                    case "STICHWORT B":
                                        operation.Keywords.B = msg;
                                        break;
                                    case "STICHWORT T":
                                        operation.Keywords.T = msg;
                                        break;
                                    case "STICHWORT S":
                                        operation.Keywords.S = msg;
                                        break;
                                    case "STICHWORT I":
                                        operation.CustomData["Stichwort I"] = msg;
                                        break;
                                    case "STICHWORT R":
                                        operation.Keywords.R = msg;
                                        break;
                                    case "PRIO.":
                                        operation.Priority = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                if (line.StartsWith("NAME", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    continue;
                                }

                                int indexEquip = line.IndexOf(":", StringComparison.Ordinal);
                                int indexTime = line.IndexOf(":", indexEquip + 1, StringComparison.Ordinal);
                                if (line.Contains("Fehlt:"))
                                {
                                    indexTime = line.IndexOf(":", indexTime + 1, StringComparison.Ordinal);
                                }

                                OperationResource resource = new OperationResource();
                                resource.FullName = line.Substring(0, indexEquip).Trim();
                                resource.RequestedEquipment.Add(line.Substring(indexEquip + 1, indexTime - indexEquip - 1).Trim());
                                resource.Timestamp = line.Substring(indexTime + 1).Trim();
                                operation.Resources.Add(resource);
                            }
                            break;
                        case CurrentSection.GBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment.AppendLine(msg);
                            }
                            break;
                        case CurrentSection.HTextbausteine:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Picture = operation.Picture.AppendLine(msg);
                            }
                            break;
                        case CurrentSection.IFooter:
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
            DZielort,
            EEinsatzgrund,
            FEinsatzmittel,
            GBemerkung,
            HTextbausteine,
            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            IFooter,
        }

        #endregion
    }
}