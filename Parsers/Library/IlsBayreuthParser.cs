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
using System.Globalization;
using AlarmWorkflow.Parser.Library.util;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsBayreuthParser", typeof(IParser))]
    sealed class IlsBayreuthParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[]
            {
                "Absender","Einsatznummer","Name","Telefon","Straße","Einsatzort Position X", "Einsatzort Position Y",
                "Abschnitt","Ort","Objekt","Kreuzung","Station","Schlagwort.",
                "Stichwort","- Brand","- Rettungsdienst","- Sonstiges",
                "- THL","- Info","Einsatzmittelname","gef. Geräte"
            };
        private double geoX;
        private double geoY;

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
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);
            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
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
                                    case "EINSATZNUMMER":
                                        operation.OperationNumber = ParserUtility.GetTextBetween(msg, null, "Alarmzeit:");
                                        string timestamp = ParserUtility.GetTextBetween(msg, "Alarmzeit:");
                                        operation.Timestamp = ParserUtility.ReadFaxTimestamp(timestamp, DateTime.Now);
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
                                    case "TELEFON":
                                        operation.Messenger = string.Format("{0} Telefon: {1}", operation.Messenger, msg);
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
                                    case "ABSCHNITT":
                                        operation.CustomData["Einsatzort Abschnitt"] = msg;
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
                                    case "OBJEKT":
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection = msg;
                                        break;
                                    case "STATION":
                                        operation.CustomData["Einsatzort Station"] = ParserUtility.GetTextBetween(msg, null, "Objektnummer");
                                        operation.OperationPlan = ParserUtility.GetMessageText(ParserUtility.GetTextBetween(msg, "Objektnummer"), "");
                                        break;
                                    case "Einsatzort Position X":
                                        geoX = double.Parse(msg, nfi);
                                        break;
                                    case "Einsatzort Position Y":
                                        geoY = double.Parse(msg, nfi);
                                        var geo = GeographicCoords.FromGaussKrueger(geoX, geoY);
                                        operation.Einsatzort.GeoLatitude = geo.Latitude;
                                        operation.Einsatzort.GeoLongitude = geo.Longitude;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGWORT.":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "- BRAND":
                                        operation.Keywords.B = msg;
                                        break;
                                    case "- RETTUNGSDIENST":
                                        operation.Keywords.R = msg;
                                        break;
                                    case "- SONSTIGES":
                                        operation.Keywords.S = msg;
                                        break;
                                    case "- THL":
                                        operation.Keywords.T = msg;
                                        break;
                                    case "- INFO":
                                        operation.CustomData["Stichwort I"] = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EEinsatzmittel:
                            {
                                switch (prefix)
                                {
                                    case "EINSATZMITTELNAME":
                                        last.FullName = msg;
                                        break;
                                    case "GEF. GERÄTE":
                                        last.RequestedEquipment.Add(msg);
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
            GFooter,
        }

        #endregion
    }
}