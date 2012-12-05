using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSStraubingParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILSStraubingParser.
    /// </summary>
    [Export("ILSStraubingParser", typeof(IFaxParser))]
    sealed class ILSStraubingParser : IFaxParser
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ILSStraubingParser class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public ILSStraubingParser()
        {

        }

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
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
                bool getAlarmTime = false;

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
                                operation.Location = msgx;
                                break;
                            case "STRAßE":
                            case "STRABE":
                                operation.Street = msgx;
                                break;                           
                            case "EINSATZPLAN":
                                operation.OperationPlan = msgx;
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
                    int x0 = line.IndexOf("DEG FF");
                    if (x0 != -1)
                    {

                        int anfang = line.IndexOf(':');

                        string altime = line.Substring(anfang + 15);       
                        altime = altime.Substring(0, altime.Length - 1); 
                        altime = altime.Trim();                                        
                        operation.CustomData["Alarmtime"] = "Alarmzeit: " + altime;
                        getAlarmTime = true;

                    }

                    // Auslesen des Zeitpunkts des Faxeingangs
                    if (Faxtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Faxtime"] = "Faxeingang: " + uhrzeit.ToString("HH:mm:ss ");
                        Faxtime = true;
                    }

                    // Weitere Standardinfos auslesen, ohne ":"
                    if (line.StartsWith("Einsatznummer"))
                    {
                        operation.OperationNumber = line.Substring(14);
                    }

                    if (line.StartsWith("Objekt"))
                    {
                        operation.Property = line.Substring(7);
                        operation.Property = operation.Property.Trim();
                    }

                    if (line.StartsWith("Name"))
                    {
                        operation.Messenger = operation.Messenger + line.Substring(5);
                    }

                    operation.Messenger = operation.Messenger + " ";

                    if (operation.Messenger.Contains("Ausgerückt") == true)
                    {
                        operation.Messenger = operation.Messenger.Replace(": Alarmiert : Ausgerückt", "");
                        operation.Messenger = operation.Messenger.Trim();
                    }

                    if (line.StartsWith("Schlagw."))
                    {
                        operation.Picture = operation.Picture + line.Substring(11);
                        operation.Picture = operation.Picture.Trim();
                    }

                    if (line.StartsWith("Stichw. B"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. T"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. S"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. I"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. R"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    //Ort Einlesen
                    if ((line.StartsWith("Ort")) && (nextIsOrt == false))
                    {
                        operation.City = operation.City + line.Substring(4);
                        operation.City = operation.City.Trim();
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
                        operation.City = operation.City + " ";
                        ReplCity = true;
                    }

                    if (operation.City.Contains("ß") == true)
                    {
                        operation.City = operation.City.Replace("ß", "ss");
                    }

                    if (operation.City.Contains("ä") == true)
                    {
                        operation.City = operation.City.Replace("ä", "ae");
                    }

                    if (operation.City.Contains("ö") == true)
                    {
                        operation.City = operation.City.Replace("ö", "oe");
                    }

                    if (operation.City.Contains("ü") == true)
                    {
                        operation.City = operation.City.Replace("ü", "ue");
                    }

                    // Sonderzeichenersetzung in der Strasse

                    if (ReplStreet == false)
                    {
                        operation.Street = operation.Street + " ";
                        ReplStreet = true;
                    }

                    if (operation.Street.Contains("Haus-Nr.:") == true)
                    {
                        operation.Street = operation.Street.Replace("Haus-Nr.:", "");
                    }

                    if (operation.Street.Contains("ß") == true)
                    {
                        operation.Street = operation.Street.Replace("ß", "ss");
                    }

                    if (operation.Street.Contains("ä") == true)
                    {
                        operation.Street = operation.Street.Replace("ä", "ae");
                    }

                    if (operation.Street.Contains("ö") == true)
                    {
                        operation.Street = operation.Street.Replace("ö", "oe");
                    }

                    if (operation.Street.Contains("ü") == true)
                    {
                        operation.Street = operation.Street.Replace("ü", "ue");
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
