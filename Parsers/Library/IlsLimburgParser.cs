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

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsLimburgParser", typeof(IParser))]
    class IlsLimburgParser : IParser
    {
        #region Constants

        private static readonly string[] Keywords = new[] { "Alarmstichwort", "Einsatzort", "Ortsteil", "Ort", "Objekt" };

        #endregion

        #region Implementation of IParser

        public Operation Parse(string[] lines)
        {
            Operation operation = new Operation();

            lines = Utilities.Trim(lines);
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    var line = lines[i];
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
                            case "ALARMSTICHWORT":
                                operation.Keywords.Keyword = msg;
                                break;
                            case "EINSATZORT":
                                string street, streetNumber, appendix;
                                ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                operation.CustomData["Einsatzort Zusatz"] = appendix;
                                operation.Einsatzort.Street = street;
                                operation.Einsatzort.StreetNumber = streetNumber;
                                break;
                            case "ORT":
                                operation.Einsatzort.City = msg;
                                break;
                            case "ORTSTEIL":
                                operation.CustomData["Einsatzort Ortsteil"] = msg;
                                break;
                            case "OBJEKT":
                                operation.Einsatzort.Property = msg;
                                break;

                        }
                    }
                    else
                    {
                        operation.Comment = operation.Comment.AppendLine(line);
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
    }
}
