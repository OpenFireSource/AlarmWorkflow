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
using System.Configuration;
using System.Linq;
using System.Text;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("IlsMagdeburgParser", typeof(IParser))]
    internal class IlsMagdeburgParser : IParser
    {
        #region Fields

        private readonly string[] _keywords =
        {
            "Einsatz-Nr.", "Str./Hausnr.", "Ortst./Gem.",
            "sonst. Ortsangabe.", "Objekt-Nummer","Objekt",
            "Stichwort", "Bemerkungen", "Telefonnummer", "Meldender"
        };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.AHeader;
            InnerSection inner = InnerSection.None;
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
                        if (x == -1 || x > keyword.Length + 1)
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
                                if (inner == InnerSection.ENr)
                                {
                                    operation.OperationNumber = msg;
                                }
                                switch (prefix)
                                {
                                    case "EINSATZ-NR.":
                                        inner = InnerSection.ENr;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.Einsatzmeldung:
                            switch (prefix)
                            {
                                case "STR./HAUSNR.":
                                    string street, streetNumber, appendix;

                                    ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                    operation.CustomData["Einsatzort Zusatz"] = appendix;
                                    operation.Einsatzort.Street = street;
                                    operation.Einsatzort.StreetNumber = streetNumber;
                                    break;
                                case "SONST. ORTSANGABE.":
                                    operation.CustomData["Einsatzort Zusatz"] = (operation.CustomData["Einsatzort Zusatz"] as string).AppendLine(msg);
                                    break;
                                case "OBJEKT":
                                    operation.Einsatzort.Property = msg;
                                    break;
                                case "OBJEKT-NUMMER":
                                    operation.Einsatzort.Property = operation.Einsatzort.Property.AppendLine(msg);
                                    break;
                                case "ORTST./GEM.":
                                    operation.Einsatzort.City = msg;
                                    break;
                                case "STICHWORT":
                                    operation.Keywords.Keyword = msg;
                                    break;
                                case "MELDENDER":
                                    operation.Messenger = msg;
                                    break;
                                case "TELEFONNUMMER":
                                    operation.Messenger = operation.Messenger.AppendLine(msg);
                                    break;
                                case "BEMERKUNGEN":
                                    operation.Picture = msg;
                                    break;
                            }
                            break;
                        case CurrentSection.Hinweise:
                            {
                                operation.Comment = operation.Comment.AppendLine(msg);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this,
                        "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }
            return operation;
        }

        #endregion

        #region Methods

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {
            if (line.Contains("Einsatzmeldung"))
            {
                section = CurrentSection.Einsatzmeldung;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("Hinweise"))
            {
                section = CurrentSection.Hinweise;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("Alarmierungen"))
            {
                section = CurrentSection.Alarmierungen;
                keywordsOnly = false;
                return true;
            }
            return false;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            Einsatzmeldung,
            Hinweise,
            Alarmierungen,
            AHeader
        }

        private enum InnerSection
        {
            ENr,
            None
        }

        #endregion
    }
}