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
using System.Text.RegularExpressions;
using System.Globalization;


namespace AlarmWorkflow.Parser.Library
{
    [Export("LFSOffenbachParser", typeof(IParser))]
    class LFSOffenbachParser : IParser
    {
        #region Static

        private static readonly Regex _coordinatenRegex = new Regex(@"POINT \((\d+.\d+) (\d+.\d+)\)");

        #endregion

        #region Fields

        private readonly string[] _keywords = new string[] {
            "Einsatznummer","Objekt","Ort:","Ortsteil","Straße","Koordinaten",
            "Bemerkung","Meldebild","Einsatzanlass","Zielort", "Zeiten", "EM"
            };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            lines = Utilities.Trim(lines);
            foreach (var line in lines)
            {
                if (ParserUtility.StartsWithKeyword(line, _keywords, out var keyword))
                {
                    var msg = ParserUtility.GetMessageText(line, keyword);
                    switch (keyword.ToUpperInvariant())
                    {
                        case "EINSATZNUMMER":
                            {
                                operation.OperationNumber = msg;
                                break;
                            }
                        case "ORT:":
                            {
                                operation.Einsatzort.City = msg;
                                break;
                            }
                        case "STRAßE":
                            {
                                string street;
                                string streetNumber;
                                string appendix;
                                ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                operation.CustomData["Einsatzort Zusatz"] = appendix;
                                operation.Einsatzort.Street = street;
                                operation.Einsatzort.StreetNumber = streetNumber;
                                break;
                            }
                        case "OBJEKT":
                            {
                                operation.Einsatzort.Property = msg;
                                break;
                            }
                        case "BEMERKUNG":
                            {
                                operation.Comment = msg; break;
                            }
                        case "KOORDINATEN":
                            {
                                if (_coordinatenRegex.IsMatch(msg))
                                {
                                    Match m = _coordinatenRegex.Match(msg);
                                    operation.Einsatzort.GeoLatitude = Convert.ToDouble(m.Groups[1].Value, CultureInfo.InvariantCulture);
                                    operation.Einsatzort.GeoLongitude = Convert.ToDouble(m.Groups[2].Value, CultureInfo.InvariantCulture);
                                }
                                break;
                            }
                        case "EINSATZANLASS":
                            {
                                operation.Keywords.EmergencyKeyword = msg; break;
                            }
                        case "MELDEBILD":
                            {
                                operation.Picture = msg; break;
                            }
                        case "ZIELORT":
                            {
                                break;
                            }
                        case "ZEITEN":
                            {
                                break;
                            }
                        case "EM":
                            {
                                Match alarm = Regex.Match(line, @"[1-9]{1,2}-[1-9]{2}-[1-9]{1}");
                                if (alarm.Success)
                                {
                                    operation.Resources.Add(new OperationResource { FullName = alarm.Groups[0].Value });
                                }
                                break;
                            }
                    }
                }
            }

            return operation;
        }

        #endregion
    }
}
