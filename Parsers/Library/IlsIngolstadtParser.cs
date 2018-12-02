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
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AlarmWorkflow.Parser.Library.util;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsIngolstadtParser", typeof(IParser))]
    sealed class IlsIngolstadtParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Einsatz-Nr","Objekt","Station", "Strasse", "Abschnitt","Planumer",
                "Kreuzung", "Ort", "Koordinate","Schlagwort","Stichwort", "Priorität", "Prioritat"
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
                                    case "ABSENDER":
                                        operation.OperationNumber = ParserUtility.GetTextBetween(msg, "Einsatznummer:");
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.BEinsatzort:
                            {
                                switch (prefix)
                                {
                                    case "STRASSE":
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
                                            string zipCode = ParserUtility.ReadZipCodeFromCity(msg);
                                            operation.Einsatzort.ZipCode = zipCode;
                                            operation.Einsatzort.City = msg.Replace(zipCode, "").Trim();
                                            // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                            // However we can (at least with google maps) omit this information without problems!
                                            int dashIndex = operation.Einsatzort.City.IndexOf(" - ");
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
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection = msg;
                                        break;
                                    case "PLANUMER":
                                        operation.OperationPlan = msg;
                                        break;
                                    case "KOORDINATE":
                                        Regex r = new Regex(@"(\d+\.\d+)");
                                        var matches = r.Matches(line);
                                        if (matches.Count == 2)
                                        {
                                            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
                                            double geoRechts = Convert.ToDouble(matches[0].Value, nfi);
                                            double geoHoch = Convert.ToDouble(matches[1].Value, nfi);
                                            var geo = GeographicCoords.FromGaussKrueger(geoRechts, geoHoch);
                                            operation.Einsatzort.GeoLatitude = geo.Latitude;
                                            operation.Einsatzort.GeoLongitude = geo.Longitude;
                                        }
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.CEreignis:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGWORT":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "STICHWORT":
                                        operation.Keywords.EmergencyKeyword = msg;
                                        break;
                                    case "PRIORITAT":
                                    case "PRIORITÄT":
                                        operation.Priority = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DZielort:
                            switch (prefix)
                            {
                                case "STRASSE":
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
                                        // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                        // However we can (at least with google maps) omit this information without problems!
                                        int dashIndex = msg.IndexOf('-');
                                        if (dashIndex != -1)
                                        {
                                            // Ignore everything after the dash
                                            operation.Zielort.City = operation.Einsatzort.City.Substring(0, dashIndex);
                                        }
                                    }
                                    break;
                                case "OBJEKT":
                                    operation.Zielort.Property = msg;
                                    break;
                                case "STATION":
                                    operation.CustomData["Zielort Station"] = msg;
                                    break;
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                string name, equip;
                                name = ParserUtility.GetTextBetween(msg, null, ">> gefordert:");
                                equip = ParserUtility.GetTextBetween(msg, ">> gefordert:");
                                OperationResource resource = new OperationResource
                                {
                                    FullName = name,
                                    RequestedEquipment = new List<string>() { equip }
                                };
                                operation.Resources.Add(resource);
                            }
                            break;
                        case CurrentSection.EBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Picture = operation.Picture.AppendLine(msg);
                            }
                            break;
                        case CurrentSection.GFooter:
                            // The footer can be ignored completely.
                            break;
                    }
                }
                catch (Exception ex)
                {
                    string l = lines[i];
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }
            return operation;
        }

        #endregion

        #region Methods

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {

            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.BEinsatzort;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EREIGNIS"))
            {
                section = CurrentSection.CEreignis;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("ZIELORT"))
            {
                section = CurrentSection.DZielort;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.EBemerkung;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.FEinsatzmittel;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("******************") || line.Contains("xxxxxxxxxxxxxxxxxx") || line.Contains("XXXXXXXXXXXXXXXXXX"))
            {
                section = CurrentSection.GFooter;
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
            DZielort,
            BEinsatzort,
            CEreignis,
            FEinsatzmittel,
            EBemerkung,
            GFooter
        }

        #endregion
    }
}