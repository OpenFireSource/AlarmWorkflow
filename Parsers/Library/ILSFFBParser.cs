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

namespace AlarmWorkflow.Parser.Library
{
    [Export("ILSFFBParser", typeof(IParser))]
    class ILSFFBParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
                                                        {
                                                            "ALARM","E-Nr","EINSATZORT","STRAßE",
                                                            "ORTSTEIL/ORT","OBJEKT","EINSATZPLAN","MELDEBILD",
                                                            "EINSATZSTICHWORT","HINWEIS","EINSATZMITTEL","(ALARMSCHREIBEN ENDE)"
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
                if (GetKeyword(line, out keyword))
                {
                    switch (keyword.Trim())
                    {
                        case "E-Nr": { section = CurrentSection.BeNr; break; }
                        case "EINSATZORT": { section = CurrentSection.CEinsatzort; break; }
                        case "STRAßE": { section = CurrentSection.DStraße; break; }
                        case "ORTSTEIL/ORT": { section = CurrentSection.EOrt; break; }
                        case "OBJEKT": { section = CurrentSection.FObjekt; break; }
                        case "EINSATZPLAN": { section = CurrentSection.GEinsatzplan; break; }
                        case "MELDEBILD": { section = CurrentSection.HMeldebild; break; }
                        case "EINSATZSTICHWORT": { section = CurrentSection.JEinsatzstichwort; break; }
                        case "HINWEIS": { section = CurrentSection.KHinweis; break; }
                        case "EINSATZMITTEL": { section = CurrentSection.LEinsatzmittel; break; }
                        case "(ALARMSCHREIBEN ENDE)":
                            {
                                section = CurrentSection.MEnde; break;
                            }
                    }
                }


                switch (section)
                {
                    case CurrentSection.BeNr:
                        operation.OperationNumber = GetMessageText(line.Substring(0, line.IndexOf("ALARM", StringComparison.Ordinal)), keyword);
                        keyword = "ALARM";
                        try
                        {
                            operation.Timestamp = DateTime.Parse(GetMessageText(line.Substring(line.IndexOf("ALARM", StringComparison.Ordinal)), keyword));
                        }
                        catch (FormatException)
                        {
                            operation.Timestamp = DateTime.Now;
                        }
                        break;
                    case CurrentSection.CEinsatzort:
                        operation.Zielort.Location = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.DStraße:
                        string msg = GetMessageText(line, keyword);
                        string street, streetNumber, appendix;
                        ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                        operation.CustomData["Einsatzort Zusatz"] = appendix;
                        operation.Einsatzort.Street = street;
                        operation.Einsatzort.StreetNumber = streetNumber;
                        break;
                    case CurrentSection.EOrt:
                        operation.Einsatzort.City = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.FObjekt:
                        operation.Einsatzort.Property = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.GEinsatzplan:
                        operation.OperationPlan = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.HMeldebild:
                        if (operation.Picture != null)
                        {
                            operation.Picture += GetMessageText(line, keyword);
                        }
                        else
                        {
                            operation.Picture = GetMessageText(line, keyword);
                        }
                        break;
                    case CurrentSection.JEinsatzstichwort:
                        operation.Keywords.EmergencyKeyword = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.KHinweis:
                        operation.Comment += " " + GetMessageText(line, keyword);
                        break;
                    case CurrentSection.LEinsatzmittel:
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

                    case CurrentSection.MEnde:
                        break;

                }
            }

            return operation;
        }

        #endregion

        #region Methods
        private bool GetKeyword(string line, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in _keywords.Where(kwd => line.ToLowerInvariant().StartsWith(kwd.ToLowerInvariant())))
            {
                keyword = kwd;
                return true;
            }
            keyword = null;
            return false;
        }

        /// <summary>
        /// Returns the message text, which is the line text but excluding the keyword/prefix and a possible colon.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="prefix">The prefix that is to be removed (optional).</param>
        /// <returns></returns>
        private string GetMessageText(string line, string prefix)
        {
            if (prefix == null)
            {
                prefix = "";
            }

            if (prefix.Length > 0)
            {
                line = line.Remove(0, prefix.Length);
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
                line = line.Remove(0, 1);
            }
            line = line.Trim();
            return line;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AAnfang,
            BeNr,
            CEinsatzort,
            DStraße,
            EOrt,
            FObjekt,
            GEinsatzplan,
            HMeldebild,
            JEinsatzstichwort,
            KHinweis,
            LEinsatzmittel,
            MEnde
        }

        #endregion

    }
}
