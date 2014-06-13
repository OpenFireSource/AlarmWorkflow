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
    [Export("LstMansfeldSuedharz", typeof(IParser))]
    class LstMansfeldSuedharz : IParser
    {
        #region Fields

        private readonly string[] _keywords = new string[] { "Einsatzdepeche", "AAO", "Einsatzort", "Strasse", "Ort", "Objekt", "Wer", "Was", "Wo", "Einsatzplan", "Hinweistext", "Einheiten" };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.AAnfang;
            lines = Utilities.Trim(lines);
            foreach (string line in lines)
            {
                string keyword;
                string messageText = line;
                if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
                {

                    switch (keyword)
                    {
                        case "Einsatzdepeche":
                            {
                                section = CurrentSection.AAnfang;
                                break;
                            }
                        case "AAO":
                            {
                                section = CurrentSection.BAao;
                                break;
                            }
                        case "Einsatzort":
                            {
                                section = CurrentSection.CEinsatzort;
                                break;
                            }
                        case "Strasse":
                            {
                                section = CurrentSection.DStrasse;
                                break;
                            }
                        case "Ort":
                            {
                                section = CurrentSection.EOrt;
                                break;
                            }
                        case "Objekt":
                            {
                                section = CurrentSection.FObjekt;
                                break;
                            }
                        case "Wer":
                            {
                                section = CurrentSection.GMeldender;
                                break;
                            }
                        case "Was":
                            {
                                section = CurrentSection.HSchlagwort;
                                break;
                            }
                        case "Wo":
                            {
                                section = CurrentSection.JZusatzinfo;
                                break;
                            }
                        case "Einsatzplan":
                            {
                                section = CurrentSection.KEinsatzplan;
                                break;
                            }
                        case "Hinweistext":
                            {
                                section = CurrentSection.LHinweis;
                                break;
                            }
                        case "Einheiten":
                            {
                                section = CurrentSection.MEinheiten;
                                break;
                            }

                    }
                    if (section == CurrentSection.GMeldender || section == CurrentSection.HSchlagwort || section == CurrentSection.JZusatzinfo || section == CurrentSection.MEinheiten)
                    {
                        section = CurrentSection.ZEnde;
                    }
                    messageText = ParserUtility.GetMessageText(line, keyword);
                }

                switch (section)
                {
                    case CurrentSection.AAnfang:
                        {
                            operation.OperationNumber = ParserUtility.GetTextBetween(messageText, null, "am:", StringComparison.InvariantCulture);
                            string textBeteween = ParserUtility.GetTextBetween(messageText, "am:", "um", StringComparison.InvariantCulture);
                            operation.Timestamp = ParserUtility.ReadFaxTimestamp(textBeteween, DateTime.Now);
                            break;
                        }
                    case CurrentSection.BAao:
                        {
                            operation.Keywords.Keyword = messageText;
                            break;
                        }
                    case CurrentSection.CEinsatzort:
                        {
                            operation.Einsatzort.Location = messageText;
                            break;
                        }
                    case CurrentSection.DStrasse:
                        {
                            string street, streetNumber, appendix;
                            ParserUtility.AnalyzeStreetLine(messageText, out street, out streetNumber, out appendix);
                            operation.Einsatzort.Street = street;
                            operation.Einsatzort.StreetNumber = streetNumber;
                            operation.CustomData["Einsatzort Zusatz"] = appendix;
                            break;
                        }
                    case CurrentSection.EOrt:
                        {
                            operation.Einsatzort.City = messageText;
                            break;
                        }
                    case CurrentSection.FObjekt:
                        {
                            operation.Einsatzort.Property = messageText;
                            break;
                        }
                    case CurrentSection.GMeldender:
                        {
                            operation.Messenger = messageText + Environment.NewLine;
                            break;
                        }
                    case CurrentSection.HSchlagwort:
                        {
                            operation.Keywords.EmergencyKeyword = messageText;
                            break;
                        }
                    case CurrentSection.JZusatzinfo:
                        {
                            operation.Picture = messageText + Environment.NewLine;
                            break;
                        }
                    case CurrentSection.KEinsatzplan:
                        {
                            operation.OperationPlan = messageText;
                            break;
                        }
                    case CurrentSection.LHinweis:
                        {
                            operation.Comment = messageText + Environment.NewLine;
                            break;
                        }
                    case CurrentSection.MEinheiten:
                        {
                            Match match = Regex.Match(messageText, "<(.*)>");
                            if (match.Success)
                            {
                                string value = match.Groups[1].Value;
                                OperationResource operationResource = new OperationResource();
                                operationResource.FullName = value;
                                operation.Resources.Add(operationResource);
                            }
                            break;
                        }
                }
            }
            operation.Comment = ParserUtility.RemoveTrailingNewline(operation.Comment);
            operation.Picture = ParserUtility.RemoveTrailingNewline(operation.Picture);
            operation.Messenger = ParserUtility.RemoveTrailingNewline(operation.Messenger);
            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AAnfang,
            BAao,
            CEinsatzort,
            DStrasse,
            EOrt,
            FObjekt,
            GMeldender,
            HSchlagwort,
            JZusatzinfo,
            KEinsatzplan,
            LHinweis,
            MEinheiten,
            ZEnde
        }

        #endregion
    }
}
