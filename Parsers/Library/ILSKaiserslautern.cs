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
    [Export("ILSKaiserslautern", typeof(IParser))]
    sealed class ILSKaiserslautern : IParser
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ILSKaiserslautern class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public ILSKaiserslautern()
        {

        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);

            CurrentSection section = CurrentSection.AHeader;

            for (int i = 0; i < lines.Length; i++)
            {
                try
                {

                    //Definition der bool Variablen
                    //bool nextIsOrt = false;
                    bool ReplStreet = false;
                    bool ReplCity = false;
                    bool ReplComment = false;
                    bool ReplPicture = false;
                    bool Faxtime = false;
                    bool nextIsOrt = false;

                    foreach (string linex in lines)
                    {

                        string msgx;
                        string prefix;
                        int x = linex.IndexOf(':');
                        if (x != -1)
                        {
                            prefix = linex.Substring(0, x);
                            msgx = linex.Substring(x + 1).Trim();

                            prefix = prefix.Trim().ToUpperInvariant();
                            switch (prefix)
                            {

                                //Füllen der Standardinformatione Alarmfax Cases mit  ":"
                                case "EINSATZORT":
                                    operation.Einsatzort.Location = msgx;
                                    break;
                                case "EINSATZPLAN":
                                    operation.OperationPlan = msgx;
                                    break;
                                case "Hinweis":
                                    operation.Comment = msgx;
                                    break;
                            }
                        }
                    }

                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    // Einlesen mehrzeilige Bemerkung
                    switch (line.Trim())
                    {

                        case "BEMERKUNG": { section = CurrentSection.GBemerkung; continue; }
                        case "TEXTBAUSTEINE": { section = CurrentSection.HFooter; continue; }
                        default: break;
                    }

                    string msg = line;

                    // Bemerkung Section
                    switch (section)
                    {

                        case CurrentSection.GBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment += msg + "\n";
                                operation.Comment = operation.Comment.Substring(0, operation.Comment.Length - 1).Trim();
                            }
                            break;
                        case CurrentSection.HFooter:
                            // The footer can be ignored completely.
                            break;
                        default:
                            break;
                    }

                    //Auslesen der Alarmierungszeit
                    //TODO INFO ILS Ausstehend

                    // Auslesen des Zeitpunkts des Faxeingangs
                    if (Faxtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Faxtime"] = "Faxeingang: " + uhrzeit.ToString("HH:mm:ss ");
                        Faxtime = true;
                    }

                    // Weitere Standardinfos auslesen, ohne ":"

                    int x1 = line.IndexOf("Einsatznummer");
                    if (x1 != -1)
                    {
                        operation.OperationNumber = line.Substring(46);
                    }

                    if (line.StartsWith("Objekt"))
                    {
                        operation.Einsatzort.Property = line.Substring(7);
                        operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();
                    }

                    if (line.StartsWith("Meldender"))
                    {
                        operation.Messenger = operation.Messenger + line.Substring(10);
                    }

                    if (line.StartsWith("Straße"))
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street + line.Substring(7);
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Trim();
                    }

                    if (line.StartsWith("Diagnose"))
                    {
                        operation.Picture = operation.Picture + line.Substring(9);
                        operation.Picture = operation.Picture.Trim();
                    }

                    if (line.StartsWith("Stichwort"))
                    {
                        operation.Keywords.EmergencyKeyword = operation.Keywords.EmergencyKeyword + line.Substring(10);
                        operation.Keywords.EmergencyKeyword = operation.Keywords.EmergencyKeyword.Trim();
                    }

                    //Ort Einlesen
                    if ((line.StartsWith("PLZ / Ortsteil")) && (nextIsOrt == false))
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City + line.Substring(17);
                        operation.Einsatzort.City = operation.Einsatzort.City.Trim();
                        nextIsOrt = true;
                    }

                    // Sonderzeichenersetzung im Meldebild

                    if (ReplPicture == false)
                    {
                        operation.Picture = operation.Picture + " ";
                        ReplPicture = true;
                    }

                    if (operation.Picture.Contains("ß") == true)
                    {
                        operation.Picture = operation.Picture.Replace("ß", "ss");
                    }

                    if (operation.Picture.Contains("ä") == true)
                    {
                        operation.Picture = operation.Picture.Replace("ä", "ae");
                    }

                    if (operation.Picture.Contains("ö") == true)
                    {
                        operation.Picture = operation.Picture.Replace("ö", "oe");
                    }

                    if (operation.Picture.Contains("ü") == true)
                    {
                        operation.Picture = operation.Picture.Replace("ü", "ue");
                    }

                    // Sonderzeichenersetzung im Ort

                    if (ReplCity == false)
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City + " ";
                        ReplCity = true;
                    }

                    if (operation.Einsatzort.City.Contains("ß") == true)
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City.Replace("ß", "ss");
                    }

                    if (operation.Einsatzort.City.Contains("ä") == true)
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City.Replace("ä", "ae");
                    }

                    if (operation.Einsatzort.City.Contains("ö") == true)
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City.Replace("ö", "oe");
                    }

                    if (operation.Einsatzort.City.Contains("ü") == true)
                    {
                        operation.Einsatzort.City = operation.Einsatzort.City.Replace("ü", "ue");
                    }

                    // Sonderzeichenersetzung in der Strasse

                    if (ReplStreet == false)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street + " ";
                        ReplStreet = true;
                    }

                    if (operation.Einsatzort.Street.Contains("Haus-Nr.:") == true)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Replace("Haus-Nr.:", "");
                    }

                    if (operation.Einsatzort.Street.Contains("ß") == true)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Replace("ß", "ss");
                    }

                    if (operation.Einsatzort.Street.Contains("ä") == true)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Replace("ä", "ae");
                    }

                    if (operation.Einsatzort.Street.Contains("ö") == true)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Replace("ö", "oe");
                    }

                    if (operation.Einsatzort.Street.Contains("ü") == true)
                    {
                        operation.Einsatzort.Street = operation.Einsatzort.Street.Replace("ü", "ue");
                    }

                    // Sonderzeichenersetzung im Hinweis

                    if (ReplComment == false)
                    {
                        operation.Comment = operation.Comment + " ";
                        ReplComment = true;
                    }

                    if (operation.Comment.Contains("ß") == true)
                    {
                        operation.Comment = operation.Comment.Replace("ß", "ss");
                    }

                    if (operation.Comment.Contains("ä") == true)
                    {
                        operation.Comment = operation.Comment.Replace("ä", "ae");
                    }

                    if (operation.Comment.Contains("ö") == true)
                    {
                        operation.Comment = operation.Comment.Replace("ö", "oe");
                    }

                    if (operation.Comment.Contains("ü") == true)
                    {
                        operation.Comment = operation.Comment.Replace("ü", "ue");
                    }




                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }

            // Post-processing the operation if needed
            if (!string.IsNullOrWhiteSpace(operation.Comment) && operation.Comment.EndsWith("\n"))
            {
                operation.Comment = operation.Comment.Substring(0, operation.Comment.Length - 1).Trim();
            }

            return operation;
        }



        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            GBemerkung,
            /// <summary>
            /// Footer text. Can be ignored completely.
            /// </summary>
            HFooter,
        }

        #endregion

    }
}
