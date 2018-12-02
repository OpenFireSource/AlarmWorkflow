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
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsLudwigsburgParser", typeof(IParser))]
    class IlsLudwigsburgParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = { "Einsatznummer", "Meldungseingang", "Stichwort", "Sondersignal", "Hinweis", "Ortsteil", "Ort", "Strasse", "Objekt", "Kategorie", "Information", "Einsatzplan", "BMA-Nr.", "Objektplan", "Zugeteilte Fahrzeuge" };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.Anfang;
            lines = Utilities.Trim(lines);
            foreach (var line in lines)
            {
                string keyword;
                if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
                {
                    string msg = ParserUtility.GetMessageText(line, keyword);
                    switch (keyword.Trim())
                    {
                        case "Einsatznummer":
                            operation.OperationNumber = msg;
                            break;
                        case "Meldungseingang":
                            operation.Timestamp = ParserUtility.ReadFaxTimestamp(msg, DateTime.Now);
                            break;
                        case "Stichwort":
                            operation.Keywords.Keyword = msg;
                            break;
                        case "Sondersignal":
                            operation.CustomData["Sondersignal"] = msg;
                            break;
                        case "Hinweis":
                            operation.Comment = msg;
                            break;
                        case "Ort":
                            string zip = ParserUtility.ReadZipCodeFromCity(msg);
                            operation.Einsatzort.ZipCode = zip;
                            operation.Einsatzort.City = msg.Replace(zip, "").Trim();
                            break;
                        case "Ortsteil":
                            operation.CustomData["Einsatzortz Ortsteil"] = msg;
                            break;
                        case "Strasse":
                            string street, streetNumber, appendix;
                            ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                            operation.CustomData["Einsatzort Zusatz"] = appendix;
                            operation.Einsatzort.Street = street;
                            operation.Einsatzort.StreetNumber = streetNumber;
                            break;
                        case "Objekt":
                            operation.Einsatzort.Property = msg;
                            break;
                        case "Kategorie":
                            operation.CustomData["Einsatzobjekt Kategorie"] = msg;
                            break;
                        case "Information":
                            operation.CustomData["Einsatzobjekt Information"] = msg;
                            break;
                        case "Einsatzplan":
                            operation.OperationPlan = msg;
                            break;
                        case "BMA-Nr.":
                            operation.CustomData["Einsatzobjekt BMA-Nr."] = msg;
                            break;
                        case "Objektplan":
                            operation.CustomData["Einsatzobjekt Objektplan"] = msg;
                            break;
                        case "Zugeteilte Fahrzeuge":
                            section = CurrentSection.Einsatzmittel;
                            break;

                    }
                }


                switch (section)
                {
                    case CurrentSection.Einsatzmittel:
                        if (line.Equals("Zugeteilte Fahrzeuge:", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        OperationResource resource = new OperationResource();
                        resource.FullName = line;
                        operation.Resources.Add(resource);
                        break;
                }
            }

            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            Anfang,
            Einsatzmittel
        }

        #endregion

    }
}
