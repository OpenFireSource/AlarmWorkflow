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
using AlarmWorkflow.Parser.Library.util;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsAmbergParser", typeof(IParser))]
    class IlsAmbergParser : IParser
    {
        #region Fields

        private readonly string[] _keywords =
        {
            "Einsatznummer", "Name", "Rufnummer", "Straße", "Haus-Nr.",
            "Ort", "Objekt","Station", "Schlagw.",
            "Stichwort", "Alarmiert", "Gerät", "X","Y"
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
            double geoX = 0, geoY = 0;
            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
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
                    string keyword = "";
                    if (keywordsOnly)
                    {
                        if (!ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
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
                                        operation.OperationNumber = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.CKoordinaten:
                            switch (prefix)
                            {
                                case "X":
                                    geoX = double.Parse(msg, nfi);
                                    break;
                                case "Y":
                                    geoY = double.Parse(msg, nfi);
                                    var geo = GeographicCoords.FromGaussKrueger(geoX, geoY);
                                    operation.Einsatzort.GeoLatitude = geo.Latitude;
                                    operation.Einsatzort.GeoLongitude = geo.Longitude;
                                    break;
                            }
                            break;
                        case CurrentSection.BMitteiler:
                            switch (prefix)
                            {
                                case "NAME":
                                    operation.Messenger = msg;
                                    break;
                                case "RUFNUMMER":
                                    operation.Messenger = operation.Messenger.AppendLine(string.Format("Nr.: {0}", msg));
                                    break;
                            }
                            break;
                        case CurrentSection.DEinsatzort:
                            {
                                switch (prefix)
                                {
                                    case "STRAßE":
                                        operation.Einsatzort.Street = msg;
                                        break;
                                    case "HAUS-NR.":
                                        operation.Einsatzort.StreetNumber = msg;
                                        break;
                                    case "ORT":
                                        {
                                            operation.Einsatzort.ZipCode = ParserUtility.ReadZipCodeFromCity(msg);
                                            if (!string.IsNullOrWhiteSpace(operation.Einsatzort.ZipCode))
                                            {
                                                operation.Einsatzort.City = msg.Replace(operation.Einsatzort.ZipCode, "").Trim();
                                            }
                                            else
                                            {
                                                operation.Einsatzort.City = msg;
                                            }
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
                                    case "STATION":
                                        operation.CustomData["Einsatzort Station"] = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.EZielort:
                            {
                                switch (prefix)
                                {
                                    case "STRAßE":
                                        operation.Zielort.Street = msg;
                                        break;
                                    case "HAUS-NR.":
                                        operation.Zielort.StreetNumber = msg;
                                        break;
                                    case "ORT":
                                        operation.Zielort.ZipCode = ParserUtility.ReadZipCodeFromCity(msg);
                                        if (!string.IsNullOrWhiteSpace(operation.Zielort.ZipCode))
                                        {
                                            operation.Zielort.City = msg.Replace(operation.Zielort.ZipCode, "").Trim();
                                        }
                                        else
                                        {
                                            operation.Zielort.City = msg;
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
                        case CurrentSection.FEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "STICHWORT":
                                        operation.Keywords.EmergencyKeyword = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.HEinsatzmittel:
                            {
                                switch (prefix)
                                {
                                    case "NAME":
                                        last.FullName = msg;
                                        break;
                                    case "GERÄT":
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
                        case CurrentSection.GBemerkungen:
                            {
                                operation.Picture = operation.Picture.AppendLine(msg);
                            }
                            break;
                        case CurrentSection.ZFooter:
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

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {
            if (line.Contains("MITTEILER"))
            {
                section = CurrentSection.BMitteiler;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("KOORDINATEN"))
            {
                section = CurrentSection.CKoordinaten;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.DEinsatzort;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("ZIELORT"))
            {
                section = CurrentSection.EZielort;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZGRUND"))
            {
                section = CurrentSection.FEinsatzgrund;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("BEMERKUNGEN"))
            {
                section = CurrentSection.GBemerkungen;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.HEinsatzmittel;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("ENDE ALARMFAX"))
            {
                section = CurrentSection.ZFooter;
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
            CKoordinaten,
            DEinsatzort,
            EZielort,
            FEinsatzgrund,
            GBemerkungen,
            HEinsatzmittel,
            ZFooter
        }

        #endregion
    }
}