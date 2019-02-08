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
using System.Globalization;
using System.Text.RegularExpressions;
using AlarmWorkflow.Parser.Library.util;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("ILSPassauParser", typeof(IParser))]
    class ILSPassauParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = {"", "KOORDINATE", "EINSATZNUMMER", "ABSENDER", "NAME", "STRA�E", "ORT", "OBJEKT", "KREUZUNG", "STICHWORT B", "STICHWORT SO", "PRIO.", "SCHLAGW", "EINSATZMITTELNAME"};

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
                //Is not true but only works that way
                keywordsOnly = true;
                return true;
            }

            if (line.Contains("EINSATZGRUND"))
            {
                section = CurrentSection.DEinsatzgrund;
                keywordsOnly = true;
                return true;
            }

            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.EEinsatzmittel;
                keywordsOnly = true;
                return true;
            }

            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.FBemerkung;
                keywordsOnly = false;
                return true;
            }

            if (line.Contains("ENDE FAX"))
            {
                section = CurrentSection.GFooter;
                keywordsOnly = false;
                return true;
            }

            return false;
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            var operation = new Operation();
            var operationResource = new OperationResource();

            lines = Utilities.Trim(lines);

            CurrentSection section = CurrentSection.AHeader;
            bool keywordsOnly = true;

            InnerSection innerSection = InnerSection.AStra�e;
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
                        if (!StartsWithKeyword(line, out var keyword))
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
                                case "EINSATZNUMMER":
                                    operation.OperationNumber = ParserUtility.GetTextBetween(msg, null, "ALARMZEIT");
                                    operation.Timestamp = ReadFaxTimestamp(ParserUtility.GetTextBetween(msg, "ALARMZEIT", null), DateTime.Now);
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
                                case "STRA�E":
                                {
                                    innerSection = InnerSection.AStra�e;
                                    ParserUtility.AnalyzeStreetLine(msg, out var street, out var streetNumber, out var appendix);
                                    operation.CustomData["Einsatzort Zusatz"] = appendix;
                                    operation.Einsatzort.Street = street;
                                    operation.Einsatzort.StreetNumber = streetNumber;
                                }
                                    break;
                                case "ABSCHNITT":
                                    break;
                                case "ORT":
                                {
                                    innerSection = InnerSection.BOrt;
                                    operation.Einsatzort.ZipCode = ReadZipCodeFromCity(msg);
                                    if (string.IsNullOrWhiteSpace(operation.Einsatzort.ZipCode))
                                    {
                                        Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", operation.Einsatzort.City);
                                    }

                                    operation.Einsatzort.City = msg.Remove(0, operation.Einsatzort.ZipCode.Length).Trim();

                                    // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                    // However we can (at least with google maps) omit this information without problems!
                                    int dashIndex = operation.Einsatzort.City.IndexOf(" - ", StringComparison.Ordinal);
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
                                case "KREUZUNG":
                                    operation.Einsatzort.Intersection = msg;
                                    break;
                                case "STATION":
                                    operation.CustomData.Add("Einsatzort Station:", msg);
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
                                default:
                                    switch (innerSection)
                                    {
                                        case InnerSection.AStra�e:
                                            //Quite dirty because of Streetnumber. Looking for better solution
                                            operation.Einsatzort.Street += msg;
                                            break;
                                        case InnerSection.BOrt:
                                            operation.Einsatzort.City += msg;
                                            break;
                                        case InnerSection.CObjekt:
                                            operation.Einsatzort.Property += msg;
                                            break;
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
                                case "STICHWORT B":
                                    operation.Keywords.B = ParserUtility.GetTextBetween(msg, null, "STICHWORT RD:");
                                    operation.Keywords.R = ParserUtility.GetTextBetween(msg, "STICHWORT RD:", null);
                                    break;
                                case "STICHWORT SO":
                                    operation.Keywords.S = ParserUtility.GetTextBetween(msg, null, "STICHWORT TH:");
                                    operation.Keywords.T = ParserUtility.GetTextBetween(msg, "STICHWORT TH:", "STICHWORT IN:");
                                    operation.CustomData.Add("Stichwort IN:", ParserUtility.GetTextBetween(msg, "STICHWORT IN:", null));
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
                                case "EINSATZMITTELNAME":
                                    operationResource.FullName = msg.Trim();
                                    break;

                                case "GEF. GER�TE":
                                    // Only add to requested equipment if there is some text,
                                    // otherwise the whole vehicle is the requested equipment
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        operationResource.RequestedEquipment.Add(msg);
                                    }

                                    operation.Resources.Add(operationResource);
                                    operationResource = new OperationResource();
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

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BMitteiler,
            CEinsatzort,
            DEinsatzgrund,
            EEinsatzmittel,
            FBemerkung,

            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            GFooter
        }

        private enum InnerSection
        {
            AStra�e,
            BOrt,
            CObjekt,
        }

        #endregion
    }
}