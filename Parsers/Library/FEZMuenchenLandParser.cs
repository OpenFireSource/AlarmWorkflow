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
using System.Text.RegularExpressions;
using AlarmWorkflow.Parser.Library.util;

namespace AlarmWorkflow.Parser.Library
{
    [Export("FezMuenchenLandParser", typeof(IParser))]
    class FezMuenchenLandParser : IParser
    {
        #region Fields

        private readonly string[] _keywords =
        {
            "EINSATZNR","MITTEILER","EINSATZORT","STRAßE","ABSCHNITT","KREUZUNG",
            "ORTSTEIL/ORT","OBJEKT","EINSATZPLAN","MELDEBILD"
            ,"HINWEIS","GEFORDERTE EINSATZMITTEL","(ALARMSCHREIBEN ENDE)","KOORDINATEN"

        };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.AAnfang;
            lines = Utilities.Trim(lines);
            foreach (var line in lines)
            {
                string keyword;
                if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
                {
                    switch (keyword.Trim())
                    {
                        case "EINSATZNR": { section = CurrentSection.BeNr; break; }
                        case "MITTEILER": { section = CurrentSection.CMitteiler; break; }
                        case "EINSATZORT": { section = CurrentSection.DEinsatzort; break; }
                        case "STRAßE": { section = CurrentSection.EStraße; break; }
                        case "KOORDINATEN": { section = CurrentSection.Koordinaten; break; }
                        case "ABSCHNITT": { section = CurrentSection.FAbschnitt; break; }
                        case "KREUZUNG": { section = CurrentSection.GKreuzung; break; }
                        case "ORTSTEIL/ORT": { section = CurrentSection.HOrt; break; }
                        case "OBJEKT": { section = CurrentSection.JObjekt; break; }
                        case "EINSATZPLAN": { section = CurrentSection.KEinsatzplan; break; }
                        case "MELDEBILD": { section = CurrentSection.LMeldebild; break; }
                        case "HINWEIS": { section = CurrentSection.MHinweis; break; }
                        case "GEFORDERTE EINSATZMITTEL": { section = CurrentSection.NEinsatzmittel; break; }
                        case "(ALARMSCHREIBEN ENDE)": { section = CurrentSection.OEnde; break; }
                    }
                }


                switch (section)
                {
                    case CurrentSection.BeNr:
                        int indexOf = line.IndexOf("ALARM", StringComparison.InvariantCultureIgnoreCase);
                        if (indexOf == -1)
                        {
                            operation.OperationNumber = ParserUtility.GetMessageText(line, keyword);
                            break;
                        }
                        operation.OperationNumber = ParserUtility.GetMessageText(line.Substring(0, indexOf), keyword);
                        keyword = "ALARM";
                        try
                        {
                            operation.Timestamp = DateTime.Parse(ParserUtility.GetMessageText(line.Substring(indexOf), keyword));
                        }
                        catch (FormatException)
                        {
                            operation.Timestamp = DateTime.Now;
                        }
                        break;
                    case CurrentSection.CMitteiler:
                        operation.Messenger = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.DEinsatzort:
                        operation.Einsatzort.Location = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.EStraße:
                        string msg = ParserUtility.GetMessageText(line, keyword);
                        string street, streetNumber, appendix;
                        ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                        operation.CustomData["Einsatzort Zusatz"] = appendix;
                        operation.Einsatzort.Street = street;
                        operation.Einsatzort.StreetNumber = streetNumber;
                        break;
                    case CurrentSection.FAbschnitt:
                        operation.CustomData["Einsatzort Abschnitt"] = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.GKreuzung:
                        operation.Einsatzort.Intersection = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.HOrt:
                        operation.Einsatzort.City = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.JObjekt:
                        operation.Einsatzort.Property = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.KEinsatzplan:
                        operation.OperationPlan = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.LMeldebild:
                        operation.Picture = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.MHinweis:
                        operation.Comment = ParserUtility.GetMessageText(line, keyword);
                        break;
                    case CurrentSection.NEinsatzmittel:
                        if (line.StartsWith("Geforderte Einsatzmittel", StringComparison.InvariantCultureIgnoreCase))
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
                        else
                        {
                            operation.Resources.Add(new OperationResource() { FullName = line });
                        }
                        break;
                    case CurrentSection.Koordinaten:
                        string coords = ParserUtility.GetMessageText(line, keyword).Replace("GK4", "").Replace(",", ".");
                        Regex r = new Regex(@"[\d.]+");
                        var matches = r.Matches(coords);
                        if (matches.Count == 2)
                        {
                            var geo = GeographicCoords.FromGaussKrueger(Convert.ToDouble(matches[0].Value), Convert.ToDouble(matches[1].Value));
                            operation.Einsatzort.GeoLatitude = geo.Latitude;
                            operation.Einsatzort.GeoLongitude = geo.Longitude;
                        }
                        break;

                    case CurrentSection.OEnde:
                        break;

                }
            }

            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AAnfang,
            BeNr,
            CMitteiler,
            DEinsatzort,
            EStraße,
            FAbschnitt,
            GKreuzung,
            HOrt,
            JObjekt,
            KEinsatzplan,
            LMeldebild,
            MHinweis,
            NEinsatzmittel,
            OEnde,
            Koordinaten
        }

        #endregion

    }
}
