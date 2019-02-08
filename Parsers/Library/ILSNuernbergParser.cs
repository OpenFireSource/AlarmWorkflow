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
    [Export("ILSNuernbergParser", typeof(IParser))]
    sealed class ILSNuernbergParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[]
            {
                "ABSENDER", "FAX", "TERMIN", "EINSATZNUMMER", "NAME", "STRAßE","KOORDINATE",
                "ORT", "OBJEKT", "ABSCHNITT", "ZUSTÄNDIGE ILS", "ABTEILUNG", "KREUZUNG",
                "STATION", "SCHLAGWORT", "PRIO"
            };

        #endregion

        #region Methods

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
            foreach (string kwd in Keywords)
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

        private bool GetSection(string line, ref CurrentSection section, ref bool keywordsOnly)
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
            if (line.Contains("OBJEKTINFO"))
            {
                section = CurrentSection.Objektinfo;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.GBemerkung;
                keywordsOnly = false;
                return true;
            }

            if (line.Contains("ENDE ALARMFAX"))
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
                                        operation.CustomData["Absender"] = msg.Substring(0, msg.IndexOf("ALARMZEIT", StringComparison.InvariantCultureIgnoreCase));
                                        String dateString = msg.Substring(msg.IndexOf("ALARMZEIT", StringComparison.InvariantCultureIgnoreCase)).Trim().Remove(0, "ALARMZEIT".Length + 1).Trim();
                                        DateTime time = DateTime.Now;
                                        DateTime.TryParse(dateString, out time);
                                        operation.Timestamp = time;
                                        break;
                                    case "TERMIN":
                                        operation.CustomData["Termin"] = msg;
                                        break;
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
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex).Trim();
                                            }
                                        }
                                        break;
                                    case "ABSCHNITT":
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection += msg;
                                        break;
                                    case "OBJEKT":
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Einsatzort Station"] = msg;
                                        break;
                                    case "KOORDINATE":
                                        Regex r = new Regex(@"[\d\.]+");
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
                                    case "ABTEILUNG":
                                    case "ZUSTÄNDIGE ILS":
                                    
                                        //These fields are currently unassigned. If required this can be done.
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
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGWORT":
                                        operation.Keywords.Keyword = msg.Substring(0, msg.IndexOf("STICHWORT", StringComparison.InvariantCultureIgnoreCase));
                                        operation.Keywords.EmergencyKeyword = msg.Substring(msg.IndexOf("STICHWORT", StringComparison.InvariantCultureIgnoreCase)).Trim().Remove(0, "STICHWORT".Length + 1).Trim();
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
                                    //Thats the line "Name : Alarmiert : Aus : AN". Should be ignored.
                                }
                                else
                                {
                                    operation.Resources.Add(new OperationResource { FullName = msg.Replace(':',' ').Trim() });
                                }

                            }
                            break;
                        case CurrentSection.Objektinfo:
                            operation.CustomData["Objektinfo"] += line;
                            break;
                        case CurrentSection.GBemerkung:
                            {
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
            DZielort,
            EEinsatzgrund,
            FEinsatzmittel,
            Objektinfo,
            GBemerkung,

            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            HFooter,
        }

        #endregion
    }
}