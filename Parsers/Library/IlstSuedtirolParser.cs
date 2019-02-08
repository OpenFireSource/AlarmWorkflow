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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlstSuedtirolParser", typeof(IParser))]
    class IlstSuedtirolParser : IParser
    {
        private readonly string[] _keywords = new[]
            {
                "Datum","Zeit","Feuerwehr","Alarmstufe","Bezeichnung","Strasse Nr.","Strasse","PLZ","Ort","Pager Meldung","Einsatz ID","Lat/Long","Beschreibung"
            };

        #region Implementation of IParser

        public Operation Parse(string[] lines)
        {
            Operation operation = new Operation();
            string date = "";
            foreach (string line in lines)
            {
                string keyword;
                ParserUtility.StartsWithKeyword(line, _keywords, out keyword);
                string msg = ParserUtility.GetMessageText(line, keyword);

                switch (keyword)
                {
                    case "Datum":
                        date = msg;
                        break;
                    case "Zeit":
                        string time = msg;
                        operation.Timestamp = ParserUtility.ReadFaxTimestamp(string.Format("{0} {1}", date, time), DateTime.Now);
                        break;
                    case "Feuerwehr":
                        operation.CustomData["Feuerwehr"] = msg;
                        break;
                    case "Alarmstufe":
                        operation.Keywords.EmergencyKeyword = msg;
                        break;
                    case "Bezeichnung":
                        operation.Keywords.Keyword = msg;
                        break;
                    case "Strasse":
                        operation.Einsatzort.Street = msg;
                        break;
                    case "Strasse Nr.":
                        operation.Einsatzort.StreetNumber = msg;
                        break;
                    case "PLZ":
                        operation.Einsatzort.ZipCode = msg;
                        break;
                    case "Ort":
                        operation.Einsatzort.City = msg;
                        break;
                    case "Pager Meldung":
                        operation.CustomData["Pager Meldung"] = msg;
                        break;
                    case "Einsatz ID":
                        operation.OperationNumber = msg;
                        break;
                    case "Lat/Long":
                        string[] values = msg.Split(';');
                        NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
                        operation.Einsatzort.GeoLatitude = Convert.ToDouble(values[0].Trim().Replace(',', '.'), nfi);
                        operation.Einsatzort.GeoLongitude = Convert.ToDouble(values[1].Trim().Replace(',', '.'), nfi);
                        break;
                    case "Beschreibung":
                        operation.Comment = msg;
                        break;

                }
            }
            return operation;
        }

        #endregion
    }
}
