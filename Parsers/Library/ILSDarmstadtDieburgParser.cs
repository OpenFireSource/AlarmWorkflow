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
                if (ParserUtility.StartsWithKeyword(line, _keywords, out keyword))
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
                            operation.Einsatzort.City = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.COrtsteil:
                        {
                            operation.Einsatzort.City += " " + ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.DStraße:
                        {
                            operation.Einsatzort.Street = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.EHausnummer:
                        {
                            operation.Einsatzort.StreetNumber = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.FKoordinaten:
                        {
                            break;
                        }
                    case CurrentSection.GZusatzinfos:
                        {
                            operation.Comment = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.HBetroffene:
                        {
                            operation.Comment += " " + ParserUtility.GetMessageText(line);
                            section = CurrentSection.AAnfang;
                            break;
                        }
                    case CurrentSection.IEinsatzart:
                        {
                            operation.Keywords.EmergencyKeyword = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.JStichwort:
                        {
                            operation.Keywords.Keyword = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.KSondersignal:
                        {
                            break;
                        }
                    case CurrentSection.LZusatzinformationen:
                        {
                            operation.Picture = ParserUtility.GetMessageText(line);
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
                            operation.Messenger = ParserUtility.GetMessageText(line);
                            break;
                        }
                    case CurrentSection.OTelefon:
                        {
                            operation.Messenger += string.Format(@" Tel.:{0}", ParserUtility.GetMessageText(line));
                            break;
                        }
                    case CurrentSection.PAusdruck:
                        {
                            operation.Timestamp = ParserUtility.ReadFaxTimestamp(line, DateTime.Now);
                            break;
                        }
                    case CurrentSection.QReferenznummer:
                        {
                            operation.OperationNumber = ParserUtility.GetMessageText(line);
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