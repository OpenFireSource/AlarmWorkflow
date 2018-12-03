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
    [Export("IlsKreuznachParser", typeof(IParser))]
    class IlsKreuznachParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Einsatznummer", "Einsatzort", "Ort", "Strasse","Objekt","BMA-Nummer/Linie","BMA-Info",
                "Ortsteil", "Besonderheiten", "Einsatzart", "Alarmstichwort",
                "Meldender"
            };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.ADaten;
            bool keywordsOnly = true;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                try
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    GetSection(line.Trim(), ref section, ref keywordsOnly);
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
                        if (x == -1)
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
                    switch (section)
                    {
                        case CurrentSection.ADaten:
                            switch (prefix)
                            {
                                case "EINSATZNUMMER":
                                    operation.OperationNumber = msg;
                                    break;
                                case "EINSATZORT":
                                    operation.Einsatzort.Location = msg;
                                    break;
                                case "OBJEKT":
                                    operation.Einsatzort.Property = msg;
                                    break;
                                case "ORT":
                                    operation.Einsatzort.City = msg;
                                    break;
                                case "BMA-NUMMER/LINIE":
                                    operation.OperationPlan = msg;
                                    break;
                                case "ORTSTEIL":
                                    operation.CustomData["Einsatzort Ortsteil"] = msg;
                                    break;
                                case "STRASSE":
                                    string street, streetNumber, appendix;
                                    ParserUtility.AnalyzeStreetLine(msg, out street, out streetNumber, out appendix);
                                    operation.CustomData["Einsatzort Zusatz"] = appendix;
                                    operation.Einsatzort.Street = street;
                                    operation.Einsatzort.StreetNumber = streetNumber;
                                    break;
                                case "BESONDERHEITEN":
                                    operation.Comment = msg;
                                    break;
                                case "EINSATZART":
                                    operation.Keywords.Keyword = msg;
                                    break;
                                case "ALARMSTICHWORT":
                                    operation.Keywords.EmergencyKeyword = msg;
                                    break;
                                case "MELDENDER":
                                    operation.Messenger = msg;
                                    break;
                            }
                            break;
                        case CurrentSection.CEinsatzmittel:
                            if (line.Contains("Alarmierte Einheiten"))
                            {
                                continue;
                            }
                            OperationResource resource = new OperationResource {FullName = msg};
                            operation.Resources.Add(resource);
                            break;
                        case CurrentSection.EFooter:
                            break;
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

        #region Methods

        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {
            if (line.Contains("Alarmierte Einheiten"))
            {
                section = CurrentSection.CEinsatzmittel;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("Alarmgruppen"))
            {
                section = CurrentSection.ZTemp;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("Ausdruck vom"))
            {
                section = CurrentSection.EFooter;
                keywordsOnly = true;
                return true;
            }
            return false;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            ADaten,
            CEinsatzmittel,
            EFooter,
            ZTemp
        }

        #endregion
    }
}