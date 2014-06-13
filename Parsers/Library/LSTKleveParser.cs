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
using System.Linq;
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("LSTKleveParser", typeof(IParser))]
    class LSTKleveParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Einsatznr", "EArt", "Stichwort",
                "Diagnose", "Meldender", "Prioritaet", "Ort ",
                "Ortsteil", "Strasse", "Kreuzung", "NRN", "ADAC", "Info", "Objektname", "Routenausgabe", "beteiligte Einsatzmittel:","Besonderh" , "Ausdruck", "BMA-Nummer"
            };

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
                if (GetKeyword(line.TrimStart(), out keyword))
                {
                    switch (keyword)
                    {
                        case "Einsatznr": { section = CurrentSection.BEinsatznr; break; }
                        case "EArt": { section = CurrentSection.CEArt; break; }
                        case "Stichwort": { section = CurrentSection.DStichwort; break; }
                        case "Diagnose": { section = CurrentSection.EDiagnose; break; }
                        case "Meldender": { section = CurrentSection.FMeldender; break; }
                        case "Prioritaet": { section = CurrentSection.GPriorität; break; }
                        case "Ort ": { section = CurrentSection.HOrt; break; }
                        case "Ortsteil": { section = CurrentSection.IOrtsteil; break; }
                        case "Strasse": { section = CurrentSection.JStraße; break; }
                        case "Kreuzung": { section = CurrentSection.KKreuzung; break; }
                        case "ADAC": { section = CurrentSection.MADAC; break; }
                        case "Info": { section = CurrentSection.NInfo; break; }
                        case "NRN": { section = CurrentSection.LNRN; break; }
                        case "Objektname": { section = CurrentSection.OObjektname; break; }
                        case "BMA-Nummer": { section = CurrentSection.SBMA; break; }
                        case "Routenausgabe": { section = CurrentSection.PRoutenausgabe; break; }
                        case "beteiligte Einsatzmittel:": { section = CurrentSection.QEinsatzmittel; break; }
                        case "Ausdruck": { section = CurrentSection.REnde; break; }
                        case "Besonderh": { section = CurrentSection.TBesonder; break; }
                    }
                }


                switch (section)
                {
                    case CurrentSection.AAnfang:
                        {
                            break;
                        }
                    case CurrentSection.BEinsatznr:
                        {
                            operation.OperationNumber = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.CEArt:
                        {
                            operation.Keywords.Keyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.DStichwort:
                        {
                            operation.Keywords.EmergencyKeyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.EDiagnose:
                        {
                            operation.Picture = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.FMeldender:
                        {
                            operation.Messenger = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.GPriorität:
                        {
                            operation.Priority = GetMessageText(line);
                            section = CurrentSection.AAnfang;
                            break;
                        }
                    case CurrentSection.HOrt:
                        {
                            operation.Einsatzort.City = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.IOrtsteil:
                        {
                            operation.Einsatzort.City += " - " + GetMessageText(line);
                            break;
                        }
                    case CurrentSection.JStraße:
                        {
                            string street, streetNumber, appendix;
                            ParserUtility.AnalyzeStreetLine(GetMessageText(line), out street, out streetNumber, out appendix);
                            operation.Einsatzort.Street = street;
                            operation.Einsatzort.StreetNumber = streetNumber;
                            operation.CustomData["Einsatzort Zusatz"] = appendix;
                            break;
                        }
                    case CurrentSection.KKreuzung:
                        {
                            operation.Einsatzort.Intersection = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.LNRN:
                        {
                            operation.CustomData.Add("NRN", GetMessageText(line));
                            break;
                        }
                    case CurrentSection.MADAC:
                        {
                            operation.CustomData.Add("ADAC", GetMessageText(line));
                            break;
                        }
                    case CurrentSection.NInfo:
                        {
                            operation.Comment = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.OObjektname:
                        {
                            operation.Einsatzort.Property += GetMessageText(line);
                            operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();
                            break;
                        }
                    case CurrentSection.SBMA:
                        {
                            operation.Einsatzort.Property += " BMA: " + GetMessageText(line);
                            operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();
                            break;
                        }
                    case CurrentSection.PRoutenausgabe:
                        {
                            //TODO: Auswerten wenn Format bekannt ist.
                            break;
                        }
                    case CurrentSection.QEinsatzmittel:
                        {
                            Match alarmtime = Regex.Match(line, @"(([01]?\d|2[0-3]):[0-5]\d:[0-5]\d)|(--:--:--)");
                            if (alarmtime.Success)
                            {
                                string time = alarmtime.Value;
                                string unit = line.Replace(time, "").Trim();
                                operation.Resources.Add(new OperationResource { FullName = unit, Timestamp = time });
                            }
                            break;
                        }
                    case CurrentSection.REnde:
                        {
                            Match datetime = Regex.Match(line, @"[123]\d\. \w* 20\d{2}, (([01]?\d|2[0-3]):[0-5]\d)");
                            if (datetime.Success)
                            {
                                CultureInfo ci = new CultureInfo("de");
                                DateTime timeStamp;
                                operation.Timestamp = DateTime.TryParse(datetime.Value, ci, DateTimeStyles.None, out timeStamp) ? timeStamp : DateTime.Now;

                            }
                            break;
                        }
                    case CurrentSection.TBesonder:
                        {
                            operation.Einsatzort.Property += " Besonderheiten: " + GetMessageText(line);
                            operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();

                            section = CurrentSection.AAnfang;
                            break;
                        }
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
        private string GetMessageText(string line, string prefix = null)
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
            BEinsatznr,
            CEArt,
            DStichwort,
            EDiagnose,
            FMeldender,
            GPriorität,
            HOrt,
            IOrtsteil,
            JStraße,
            KKreuzung,
            LNRN,
            MADAC,
            NInfo,
            OObjektname,
            PRoutenausgabe,
            QEinsatzmittel,
            REnde,
            SBMA,
            TBesonder
        }

        #endregion
    }
}