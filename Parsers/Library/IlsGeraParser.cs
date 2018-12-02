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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsGeraParser", typeof(IParser))]
    class IlsGeraParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[] { "Gemeinde", "Ortsteil", "Strasse", 
            "Nr.","Objekthinweis", "Objekt", "Einsatzplan-Nr.", 
            "Hinweis", "Meldender", "Alter", "Einsatzart", "Stichwort", "Meldungen",
            "Bemerkung", "Anfahrtsvorschlag von Hauptwache", 
            "Beteiligte Einsatzmittel","Hydrant vor Hausnummer" };

        #endregion

        #region Implementation of IParser

        public Operation Parse(string[] lines)
        {
            Operation operation = new Operation();
            operation.CustomData["Meldungen"] = string.Empty;
            operation.CustomData["Anfahrt"] = string.Empty;
            operation.CustomData["Hydranten"] = string.Empty;
            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.Header;
            string line;
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    string msg = line;
                    string keyword;
                    if (ParserUtility.StartsWithKeyword(line, Keywords, out keyword))
                    {
                        msg = ParserUtility.GetMessageText(line, keyword);
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        switch (keyword.ToUpperInvariant())
                        {
                            case "GEMEINDE":
                                section = CurrentSection.Einsatzinfos;
                                Match match = Regex.Match(msg, @"\d{5}");
                                if (match.Success)
                                {
                                    string zip = match.Value;
                                    operation.Einsatzort.City = msg.Replace(zip, "").Trim();
                                    operation.Einsatzort.ZipCode = zip;
                                }
                                else
                                {
                                    operation.Einsatzort.City = msg;
                                }
                                break;
                            case "ORTSTEIL":
                                operation.CustomData["Ortsteil"] = msg;
                                break;
                            case "STRASSE":
                                operation.Einsatzort.Street = msg;
                                break;
                            case "NR.":
                                operation.Einsatzort.StreetNumber = msg;
                                break;
                            case "OBJEKT":
                                operation.Einsatzort.Property = msg;
                                break;
                            case "OBJEKTHINWEIS":
                                operation.Comment = msg;
                                break;
                            case "EINATZPLAN-NR.":
                                operation.OperationPlan = msg;
                                break;
                            case "HINWEIS":
                                operation.Comment = operation.Comment.AppendLine(msg);
                                break;
                            case "MELDENDER":
                                operation.Messenger = msg;
                                break;
                            case "ALTER":
                                operation.CustomData["Alter"] = msg;
                                break;
                            case "EINSATZART":
                                operation.Keywords.Keyword = msg;
                                break;
                            case "STICHWORT":
                                operation.Keywords.EmergencyKeyword = msg;
                                break;
                            case "MELDUNGEN":
                                section = CurrentSection.Meldungen;
                                break;
                            case "BEMERKUNG":
                                section = CurrentSection.Bemerkung;
                                operation.Comment = operation.Comment.AppendLine(msg);
                                break;
                            case "ANFAHRTSVORSCHLAG VON HAUPTWACHE":
                                section = CurrentSection.Anfahrt;
                                break;
                            case "BETEILIGTE EINSATZMITTEL":
                                section = CurrentSection.Einsatzmittel;
                                break;
                            case "HYDRANT VOR HAUSNUMMER":
                                section = CurrentSection.Hydranten;
                                break;
                        }
                    }
                    else
                    {
                        switch (section)
                        {
                            case CurrentSection.Meldungen:
                                operation.CustomData["Meldungen"] = (operation.CustomData["Meldungen"] as string).AppendLine(msg);
                                break;
                            case CurrentSection.Bemerkung:
                                operation.Comment = operation.Comment.AppendLine(msg);
                                break;
                            case CurrentSection.Anfahrt:
                                operation.CustomData["Anfahrt"] = (operation.CustomData["Anfahrt"] as string).AppendLine(msg);
                                break;
                            case CurrentSection.Einsatzmittel:
                                Match result = Regex.Match(msg, @"\d{3}(\.\d{2}){2}");
                                if (result.Success)
                                {
                                    operation.Resources.Add(new OperationResource
                                    {
                                        FullName = result.Value
                                    });
                                }
                                break;
                            case CurrentSection.Hydranten:
                                operation.CustomData["Hydranten"] = (operation.CustomData["Hydranten"] as string).AppendLine(msg);
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }

            //The last line contains sometimes informations about the alarmtime, operationnumber,... here I try(!) to get them.
            line = lines[lines.Length - 1];
            try
            {
                Match result = Regex.Match(line, @"(0[1-9]|[12][0-9]|3[01])[- /.] ?(0[1-9]|1[012])[- /.] ?(19|20)\d\d");
                DateTime date = DateTime.Now;
                if (result.Success)
                {
                    DateTime.TryParse(result.Value.Replace(" ", ""), out date);
                }
                operation.Timestamp = ParserUtility.ReadFaxTimestamp(line, date);
                result = Regex.Match(line, @"\d{10}");
                if (result.Success)
                {
                    operation.OperationNumber = result.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", lines.Length - 1, ex.Message);
            }
            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            Header,
            Einsatzinfos,
            Meldungen,
            Bemerkung,
            Anfahrt,
            Einsatzmittel,
            Hydranten,
            Footer
        }

        #endregion
    }
}
