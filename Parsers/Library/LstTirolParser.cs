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
    [Export("LstTirolParser", typeof(IParser))]
    class LstTirolParser : IParser
    {
        #region Static

        private static readonly Regex PlzRegex = new Regex(@"(\d{4}) (.*)", RegexOptions.Compiled);

        #endregion

        #region Fields

        private readonly string[] _keywords = new[]
                                                        {
                                                            "EINSATZBEGINN Uhrzeit/Datum","PLZ - ORT","STRASSE HNR.","OBJEKTBEZEICHNUNG",
                                                            "INFO ZU OBJEKT","INFO AUS DEN ÖRTLICHEN EINSATZINFORMATIONEN",
                                                            "MDL-NUMMER","MELDERNAME","MELDERTELEFONNUMMER","MELDER PLZ - ORT","MEDER STRASSE HNR.",
                                                            "MELDER OBJEKTBEZEICHNUNG","MELDER INFO ZUM OBJEKT",
                                                            "EINSATZCODE","EINSATZTEXT","ÖRTLICHE EINSATZINFORMATIONEN"
                                                        };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.ADaten;
            lines = Utilities.Trim(lines);
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
                    string keyword = "";
                    if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword) && section == CurrentSection.BEinsatzmittel)
                    {
                        section = CurrentSection.ADaten;
                        keywordsOnly = true;
                    }
                    if (GetSection(line.Trim(), ref section, ref keywordsOnly))
                    {
                        continue;
                    }

                    string msg = line;
                    string prefix = "";

                    // Make the keyword check - or not (depends on the section we are in; see above)
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
                                case "EINSATZBEGINN UHRZEIT/DATUM":
                                    operation.Timestamp = ParserUtility.ReadFaxTimestamp(ParserUtility.GetTextBetween(msg, null, "/"), DateTime.Now);
                                    operation.OperationNumber = ParserUtility.GetTextBetween(msg, "Einsatznr.:");
                                    break;
                                case "PLZ - ORT":
                                    if (PlzRegex.IsMatch(msg))
                                    {
                                        Match result = PlzRegex.Match(msg);
                                        operation.Einsatzort.ZipCode = result.Groups[1].Value;
                                        operation.Einsatzort.City = result.Groups[2].Value;
                                    }
                                    break;
                                case "STRASSE HNR.":
                                    operation.Einsatzort.Street = msg;
                                    break;
                                case "OBJEKTBEZEICHNUNG":
                                    operation.Einsatzort.Property = msg;
                                    break;
                                case "INFO ZUM OBJEKT":
                                    operation.Comment = operation.Comment.AppendLine(msg);
                                    break;
                                case "MELDERNAME":
                                    operation.Messenger = msg;
                                    break;
                                case "EINSATZCODE":
                                    operation.Keywords.Keyword = msg;
                                    break;
                                case "EINSATZTEXT":
                                    operation.Picture = operation.Picture.AppendLine(msg);
                                    break;

                            }
                            break;
                        case CurrentSection.BEinsatzmittel:
                            operation.Resources.Add(new OperationResource() { FullName = msg });
                            break;
                        case CurrentSection.CLink:
                            section = CurrentSection.DEnde;
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
            if (line.ToUpper().Contains("FUNKRUFNAME"))
            {
                section = CurrentSection.BEinsatzmittel;
                keywordsOnly = false;
                return true;
            }
            if (line.ToUpper().Contains("ÖRTLICHE EINSATZINFORMATIONEN"))
            {
                section = CurrentSection.CLink;
                keywordsOnly = false;
                return true;
            }
            return false;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            ADaten,
            BEinsatzmittel,
            CLink,
            DEnde
        }

        #endregion

    }
}
