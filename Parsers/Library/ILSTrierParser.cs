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
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("ILSTrierParser", typeof(IParser))]
    class ILSTrierParser : IParser
    {
        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.AHeader;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                if (line.ToUpper().StartsWith("ALARMPLAN"))
                {
                    String temp = line.Substring(line.ToUpper().IndexOf("EINSATZNUMMER", StringComparison.Ordinal)).Trim();
                    temp = temp.Substring(temp.IndexOf(" ", StringComparison.Ordinal)).Trim();
                    operation.OperationNumber = temp.Substring(0, temp.IndexOf(" ", StringComparison.Ordinal)).Trim();
                    section = CurrentSection.BBody;
                }
                else if (line.ToUpper().StartsWith("PLZ ORT"))
                {
                    operation.Einsatzort.City = GetMessageText(line, "PLZ ORT");
                }
                else if (line.ToUpper().StartsWith("STRASSE"))
                {
                    operation.Einsatzort.Street = GetMessageText(line, "STRASSE");
                }
                else if (line.ToUpper().StartsWith("OBJEKT"))
                {
                    operation.Einsatzort.Property = GetMessageText(line, "OBJEKT");
                }
                else if (line.ToUpper().StartsWith("TELEFON"))
                {
                    operation.Messenger = GetMessageText(line, "TELEFON");
                }
                else if (line.ToUpper().StartsWith("MELDUNG INFO"))
                {
                    operation.Comment = GetMessageText(line, "MELDUNG INFO");
                }
                else if (line.ToUpper().StartsWith("EINSATZART"))
                {
                    operation.Keywords.Keyword = GetMessageText(line, "EINSATZART");
                }
                else if (line.ToUpper().StartsWith("STICHWORT"))
                {
                    operation.Keywords.EmergencyKeyword = GetMessageText(line, "STICHWORT");
                }
                else if (line.ToUpper().StartsWith("ANFAHRT"))
                {
                    section = CurrentSection.CAnfahrt;
                    continue;
                }
                else if (line.ToUpper().StartsWith("BETEILIGTE EINSATZMITTEL"))
                {
                    section = CurrentSection.DEinsatzMittel;
                    continue;
                }
                else if (line.ToUpper().StartsWith("AP"))
                {
                    section = CurrentSection.EFooter;
                }
                switch (section)
                {
                    case CurrentSection.CAnfahrt:
                        if (line.Contains("PLZ"))
                        {
                            operation.OperationPlan = line.Substring(0, line.ToUpper().IndexOf("PLZ", StringComparison.Ordinal));
                            String temp = line.Substring(line.ToUpper().IndexOf("PLZ", StringComparison.Ordinal));
                            if (temp.Contains(" "))
                            {
                                temp = temp.Substring(temp.IndexOf(" ", StringComparison.Ordinal));
                            }
                            operation.Einsatzort.ZipCode = temp.Trim();
                        }
                        else
                        {
                            operation.OperationPlan = line;
                        }
                        //Only the first line is interesting ;)
                        section = CurrentSection.BBody;
                        break;
                    case CurrentSection.DEinsatzMittel:
                        if (line.ToUpper().Contains("FAHRZEUG"))
                        {
                            continue;
                        }

                        Regex timeStamp = new Regex("\\d\\d:\\d\\d:\\d\\d");
                        line = timeStamp.Replace(line, "").Trim();
                        OperationResource resource = new OperationResource { FullName = line };
                        operation.Resources.Add(resource);
                        break;
                    case CurrentSection.EFooter:
                        return operation;
                }
            }
            return operation;
        }

        private string GetMessageText(string line, string prefix)
        {
            if (prefix == null)
            {
                prefix = "";
            }

            if (prefix.Length > 0)
            {
                line = line.Remove(0, prefix.Length).Trim();
            }
            else
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    line = line.Remove(0, colonIndex + 1);
                }
            }

            if (line.StartsWith(":"))
            {
                line = line.Remove(0, 1).Trim();
            }

            return line;
        }

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BBody,
            CAnfahrt,
            DEinsatzMittel,
            EFooter,
        }

        #endregion
    }
}