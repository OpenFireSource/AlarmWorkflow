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
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using System.Globalization;
using AlarmWorkflow.Parser.Library.util;

namespace AlarmWorkflow.Parser.Library
{
    [Export("ILSFFBParser", typeof(IParser))]
    class ILSFFBParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
                                                        {
                                                            "ALARM","E-Nr","EINSATZORT","STRAßE","KOORDINATEN", "ABSCHNITT",
                                                            "ORTSTEIL / ORT","ORTSTEIL/ORT","OBJEKT","EINSATZPLAN","MELDEBILD",
                                                            "EINSATZSTICHWORT","HINWEIS","EINSATZMITTEL","(ALARMSCHREIBEN ENDE)"
                                                        };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.Anfang;
            lines = Utilities.Trim(lines);
            string streetData = string.Empty;
            string sectionData = string.Empty;

            foreach (var line in lines)
            {
                string keyword;

                if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
                {
                    switch (keyword.Trim())
                    {
                        case "E-Nr": { section = CurrentSection.ENr; break; }
                        case "EINSATZORT": { section = CurrentSection.Einsatzort; break; }
                        case "STRAßE": { section = CurrentSection.Straße; break; }
                        case "ABSCHNITT": { section = CurrentSection.Abschnitt; break; }
                        case "KOORDINATEN": { section = CurrentSection.Koordinaten; break; }
                        case "ORTSTEIL/ORT":
                        case "ORTSTEIL / ORT": { section = CurrentSection.Ort; break; }
                        case "OBJEKT": { section = CurrentSection.Objekt; break; }
                        case "EINSATZPLAN": { section = CurrentSection.Einsatzplan; break; }
                        case "MELDEBILD": { section = CurrentSection.Meldebild; break; }
                        case "EINSATZSTICHWORT": { section = CurrentSection.Einsatzstichwort; break; }
                        case "HINWEIS": { section = CurrentSection.Hinweis; break; }
                        case "EINSATZMITTEL": { section = CurrentSection.Einsatzmittel; break; }
                        case "(ALARMSCHREIBEN ENDE)":
                            {
                                section = CurrentSection.Ende; break;
                            }
                    }
                }


                switch (section)
                {
                    case CurrentSection.ENr:
                        string opnummer = ParserUtility.GetTextBetween(line, null, "ALARM");
                        string optime = ParserUtility.GetTextBetween(line, "ALARM");
                        operation.OperationNumber = ParserUtility.GetMessageText(opnummer, keyword);
                        operation.Timestamp = ParserUtility.ReadFaxTimestamp(optime, DateTime.Now);
                        break;
                    case CurrentSection.Einsatzort:
                        operation.Zielort.Location = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.Straße:
                        string msg = ParserUtility.GetMessageText(line, keyword);
                        streetData += msg;
                        break;
                    case CurrentSection.Abschnitt:
                        sectionData += ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.Ort:
                        operation.Einsatzort.City = ParserUtility.GetMessageText(line, keyword);
                        if (operation.Einsatzort.City.Contains(" - "))
                        {
                            int i = operation.Einsatzort.City.IndexOf(" - ");
                            operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, i).Trim();
                        }
                        break;
                    case CurrentSection.Objekt:
                        operation.Einsatzort.Property += ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.Einsatzplan:
                        operation.OperationPlan = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.Meldebild:
                        operation.Picture = operation.Picture.AppendLine(ParserUtility.GetMessageText(line, keyword));
                        break;
                    case CurrentSection.Einsatzstichwort:
                        operation.Keywords.EmergencyKeyword = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.Hinweis:
                        operation.Comment = operation.Comment.AppendLine(ParserUtility.GetMessageText(line, keyword));
                        break;
                    case CurrentSection.Einsatzmittel:
                        if (line.Equals("EINSATZMITTEL: ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        OperationResource resource = new OperationResource();
                        if (line.Contains('('))
                        {
                            string tool = line.Substring(line.IndexOf("(", StringComparison.Ordinal) + 1);
                            tool = tool.Length >= 2 ? tool.Substring(0, tool.Length - 2).Trim() : String.Empty;
                            string unit = line.Substring(0, line.IndexOf("(", StringComparison.Ordinal));
                            resource.FullName = unit;
                            resource.RequestedEquipment.Add(tool);
                            operation.Resources.Add(resource);

                        }
                        break;
                    case CurrentSection.Koordinaten:
                        string coords = ParserUtility.GetMessageText(line, keyword);
                        if (string.IsNullOrWhiteSpace(coords))
                        {
                            break;
                        }
                        double east = double.Parse(coords.Split('/')[0],CultureInfo.InvariantCulture);
                        double north = double.Parse(coords.Split('/')[1], CultureInfo.InvariantCulture);
                        var geo = GeographicCoords.FromGaussKrueger(east, north);
                        operation.Einsatzort.GeoLatitude = geo.Latitude;
                        operation.Einsatzort.GeoLongitude = geo.Longitude;
                        break;
                    case CurrentSection.Ende:
                        break;

                }
            }
            string street, streetNumber, appendix;
            ParserUtility.AnalyzeStreetLine(streetData.Replace("1.2", ""), out street, out streetNumber, out appendix);
            operation.CustomData["Einsatzort Zusatz"] = appendix;
            operation.Einsatzort.Street = street.Trim();
            operation.Einsatzort.StreetNumber = streetNumber;
            operation.Einsatzort.Intersection = sectionData;
            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            Anfang,
            ENr,
            Einsatzort,
            Straße,
            Ort,
            Objekt,
            Einsatzplan,
            Meldebild,
            Einsatzstichwort,
            Hinweis,
            Einsatzmittel,
            Ende,
            Koordinaten,
            Abschnitt
        }

        #endregion

    }
}
