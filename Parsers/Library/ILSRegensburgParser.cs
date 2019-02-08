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
using AlarmWorkflow.Parser.Library.util;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("ILSRegensburgParser", typeof(IParser))]
    class ILSRegensburgParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Einsatznummer", "Name", "Straße", "Abschnitt", "Ort",
                "Gemeinde", "Kreuzung", "Objekt", "Koordinate", "Schlagw.",
                "Stichwort", "Prio.", "Alarmiert", "gef. Gerät"
            };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.AHeader;
            bool keywordsOnly = true;
            bool multiLineProperties = false;
            string keyword = "";
            string prefix = "";
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    if (GetSection(line.Trim(), ref section, ref keywordsOnly, ref multiLineProperties))
                    {
                        continue;
                    }

                    string msg = line;

                    // Make the keyword check - or not (depends on the section we are in; see above)
                    if (!multiLineProperties)
                    {
                        prefix = "";
                    }
                    if (keywordsOnly)
                    {
                        bool foundKeyword = ParserUtility.StartsWithKeyword(line, _keywords, out keyword);
                        if (!foundKeyword && !multiLineProperties)
                        {
                            continue;
                        }
                        if (foundKeyword)
                        {
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
                            operation.Messenger = msg;
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
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex).Trim();
                                            }
                                            break;
                                        }
                                    case "GEMEINDE":
                                        {
                                            operation.CustomData.Add("GEMEINDE", msg);
                                        }
                                        break;
                                    case "OBJEKT":
                                        if (msg.Contains("EPN:"))
                                        {
                                            operation.Einsatzort.Property = ParserUtility.GetTextBetween(msg, null, "EPN");
                                            operation.OperationPlan = ParserUtility.GetTextBetween(msg, "EPN");
                                        }
                                        else
                                        {
                                            operation.Einsatzort.Property = msg;
                                        }
                                        break;
                                    case "ABSCHNITT":
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection += msg;
                                        break;
                                    case "KOORDINATE":
                                        Regex r = new Regex(@"\d+");
                                        var matches = r.Matches(line);
                                        if (matches.Count == 2)
                                        {
                                            int geoRechts = Convert.ToInt32(matches[0].Value);
                                            int geoHoch = Convert.ToInt32(matches[1].Value);
                                            var geo = GeographicCoords.FromGaussKrueger(geoRechts, geoHoch);
                                            operation.Einsatzort.GeoLatitude = geo.Latitude;
                                            operation.Einsatzort.GeoLongitude = geo.Longitude;
                                        }
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
                                    case "PRIO.":
                                        operation.Priority = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzmittel:
                            {
                                switch (prefix)
                                {
                                    case "NAME":
                                        last.FullName = msg;
                                        break;
                                    case "GEF. GERÄT":
                                        // Only add to requested equipment if there is some text,
                                        // otherwise the whole vehicle is the requested equipment
                                        if (!string.IsNullOrWhiteSpace(msg))
                                        {
                                            last.RequestedEquipment.Add(msg);
                                        }
                                        break;
                                    case "ALARMIERT":
                                        last.Timestamp = ParserUtility.TryGetTimestampFromMessage(msg, DateTime.Now).ToString();

                                        operation.Resources.Add(last);
                                        last = new OperationResource();
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.FBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment.AppendLine(msg);
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

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly, ref bool multiLineProperties)
        {
            if (line.Contains("MITTEILER"))
            {
                section = CurrentSection.BMitteiler;
                multiLineProperties = false;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.CEinsatzort;
                keywordsOnly = true;
                multiLineProperties = true;
                return true;
            }
            if (line.Contains("EINSATZGRUND"))
            {
                section = CurrentSection.DEinsatzgrund;
                multiLineProperties = false;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.EEinsatzmittel;

                multiLineProperties = false;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.FBemerkung;
                multiLineProperties = false;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("ENDE ALARMFAX — V2.0"))
            {
                section = CurrentSection.GFooter;
                multiLineProperties = false;
                keywordsOnly = false;
                return true;
            }
            return false;
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
