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
    [Export("ILSDarmstadtDieburgParser", typeof(IParser))]
    class ILSDarmstadtDieburgParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Ort ", "Ortsteil", "Straße", "Hausnummer", "Koordinaten ", "Zusatzinfos", "Betroffene",
                "Einsatzart", "Stichwort", "Sondersignal", "Zusatzinformationen", "Alarmierungen",
                "Meldende", "Telefon", "Ausdruck", "Referenznummer"
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
                if (GetKeyword(line, out keyword))
                {
                    switch (keyword)
                    {
                        case "Ort ": { section = CurrentSection.BOrt; break; }
                        case "Ortsteil": { section = CurrentSection.COrtsteil; break; }
                        case "Straße": { section = CurrentSection.DStraße; break; }
                        case "Hausnummer": { section = CurrentSection.EHausnummer; break; }
                        case "Koordinaten ": { section = CurrentSection.FKoordinaten; break; }
                        case "Zusatzinfos": { section = CurrentSection.GZusatzinfos; break; }
                        case "Betroffene": { section = CurrentSection.HBetroffene; break; }
                        case "Einsatzart": { section = CurrentSection.IEinsatzart; break; }
                        case "Stichwort": { section = CurrentSection.JStichwort; break; }
                        case "Sondersignal": { section = CurrentSection.KSondersignal; break; }
                        case "Zusatzinformationen": { section = CurrentSection.LZusatzinformationen; break; }
                        case "Alarmierungen": { section = CurrentSection.MAlarmierungen; break; }
                        case "Meldende": { section = CurrentSection.NMeldende; break; }
                        case "Telefon": { section = CurrentSection.OTelefon; break; }
                        case "Ausdruck": { section = CurrentSection.PAusdruck; break; }
                        case "Referenznummer": { section = CurrentSection.QReferenznummer; break; }
                    }
                }


                switch (section)
                {
                    case CurrentSection.AAnfang:
                        {
                            break;
                        }
                    case CurrentSection.BOrt:
                        {
                            operation.Einsatzort.City = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.COrtsteil:
                        {
                            operation.Einsatzort.City += " " + GetMessageText(line);
                            break;
                        }
                    case CurrentSection.DStraße:
                        {
                            operation.Einsatzort.Street = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.EHausnummer:
                        {
                            operation.Einsatzort.StreetNumber = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.FKoordinaten:
                        {
                            break;
                        }
                    case CurrentSection.GZusatzinfos:
                        {
                            operation.Comment = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.HBetroffene:
                        {
                            operation.Comment += " " + GetMessageText(line);
                            section = CurrentSection.AAnfang;
                            break;
                        }
                    case CurrentSection.IEinsatzart:
                        {
                            operation.Keywords.EmergencyKeyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.JStichwort:
                        {
                            operation.Keywords.Keyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.KSondersignal:
                        {
                            break;
                        }
                    case CurrentSection.LZusatzinformationen:
                        {
                            operation.Picture = GetMessageText(line);
                            section = CurrentSection.AAnfang;
                            break;
                        }
                    case CurrentSection.MAlarmierungen:
                        {
                            Match alarm = Regex.Match(line, @"((0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d ([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]) (\d{5})");
                            if (alarm.Success)
                            {
                                operation.Resources.Add(new OperationResource { FullName = alarm.Groups[6].Value, Timestamp = alarm.Groups[1].Value });
                            }
                            break;
                        }
                    case CurrentSection.NMeldende:
                        {
                            operation.Messenger = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.OTelefon:
                        {
                            operation.Messenger += string.Format(@" Tel.:{0}", GetMessageText(line));
                            break;
                        }
                    case CurrentSection.PAusdruck:
                        {
                            operation.Timestamp = ReadFaxTimestamp(line, DateTime.Now);
                            break;
                        }
                    case CurrentSection.QReferenznummer:
                        {
                            operation.OperationNumber = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.REnde:
                        {
                            break;
                        }
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
            Match ts = Regex.Match(line, @"([01]?[0-9]|2[0-3]):[0-5][0-9]");
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

        private enum CurrentSection
        {
            AAnfang,
            BOrt,
            COrtsteil,
            DStraße,
            EHausnummer,
            FKoordinaten,
            GZusatzinfos,
            HBetroffene,
            IEinsatzart,
            JStichwort,
            KSondersignal,
            LZusatzinformationen,
            MAlarmierungen,
            NMeldende,
            OTelefon,
            PAusdruck,
            QReferenznummer,
            REnde
        }
    }
}