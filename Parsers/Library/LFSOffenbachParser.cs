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
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("LFSOffenbachParser", typeof(IParser))]
    class LFSOffenbachParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new string[] {
            "ALARMAUSDRUCK","EINSATZNUMMER","ORT ","STRASSE"
            ,"OBJEKT ","EINSATZPLANNUMMER","DIAGNOSE",
            "EINSATZSTICHWORT","BEMERKUNGEN","DAS FAX WURDE", "AUSDRUCK VOM", "MELDENDE(R)"
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
                    switch (keyword)
                    {
                        case "EINSATZNUMMER": { section = CurrentSection.BeNr; break; }
                        case "ORT ": { section = CurrentSection.CEinsatzort; break; }
                        case "STRASSE": { section = CurrentSection.DStraße; break; }
                        case "OBJEKT ": { section = CurrentSection.FObjekt; break; }
                        case "EINSATZPLANNUMMER": { section = CurrentSection.GEinsatzplan; break; }
                        case "DIAGNOSE": { section = CurrentSection.HMeldebild; break; }
                        case "EINSATZSTICHWORT": { section = CurrentSection.JEinsatzstichwort; break; }
                        case "MELDENDE(R)": { section = CurrentSection.LMeldender; break; }
                        case "BEMERKUNGEN": { section = CurrentSection.KHinweis; break; }
                        case "DAS FAX WURDE": { section = CurrentSection.OFaxtime; break; }
                        case "AUSDRUCK VOM": { section = CurrentSection.MEnde; break; }
                    }
                }
                else
                {
                    section = CurrentSection.MEnde;
                }

                switch (section)
                {
                    case CurrentSection.BeNr:
                        operation.OperationNumber = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.CEinsatzort:
                        string txt = GetMessageText(line, keyword);
                        operation.Einsatzort.ZipCode = ReadZipCodeFromCity(txt);
                        operation.Einsatzort.City = txt.Remove(0, operation.Einsatzort.ZipCode.Length).Trim();
                        break;
                    case CurrentSection.DStraße:
                        operation.Einsatzort.Street = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.FObjekt:
                        operation.Einsatzort.Property = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.GEinsatzplan:
                        operation.OperationPlan = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.HMeldebild:
                        operation.Picture = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.JEinsatzstichwort:
                        operation.Keywords.EmergencyKeyword = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.KHinweis:
                        operation.Comment += " " + GetMessageText(line, keyword);
                        operation.Comment = operation.Comment.Trim();
                        break;
                    case CurrentSection.OFaxtime:
                        operation.Timestamp = ReadFaxTimestamp(line, DateTime.Now);
                        break;
                    case CurrentSection.MEnde:
                        break;
                }
            }

            return operation;
        }

        #endregion

        #region Methods

        private DateTime ReadFaxTimestamp(string line, DateTime fallback)
        {
            DateTime date = fallback;
            TimeSpan timestamp = date.TimeOfDay;

            Match dt = Regex.Match(line, @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d");
            Match ts = Regex.Match(line, @"([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]");
            if (dt.Success)
            {
                DateTime.TryParse(dt.Value, out date);
            }
            if (ts.Success)
            {
                TimeSpan.TryParse(ts.Value, out timestamp);
            }

            return new DateTime(date.Year, date.Month, date.Day, timestamp.Hours, timestamp.Minutes, timestamp.Seconds, timestamp.Milliseconds, DateTimeKind.Local);
        }

        /// <summary>
        /// Attempts to read the zip code from the city, if available.
        /// </summary>
        /// <param name="cityText"></param>
        /// <returns>The zip code of the city. -or- null, if there was no.</returns>
        private string ReadZipCodeFromCity(string cityText)
        {
            string zipCode = "";
            foreach (char c in cityText)
            {
                if (char.IsNumber(c))
                {
                    zipCode += c;
                    continue;
                }
                break;
            }
            return zipCode;
        }

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
            FObjekt,
            GEinsatzplan,
            HMeldebild,
            JEinsatzstichwort,
            KHinweis,
            LMeldender,
            MEnde,
            OFaxtime
        }

        #endregion
    }
}
