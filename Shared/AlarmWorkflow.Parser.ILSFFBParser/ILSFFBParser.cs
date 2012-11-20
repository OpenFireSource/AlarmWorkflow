using System;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSFFBParser
{
    [Export("ILSFFBParser", typeof(IFaxParser))]
    sealed class ILSFFBParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MucLandParser class.
        /// </summary>
        public ILSFFBParser()
        {
        }

        #endregion

        #region IFaxParser Members


        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            try
            {

                //Definition der bool Variablen
                //bool nextIsOrt = false;
                bool ReplStreet = false;
                bool ReplVehicle = false;
                bool ReplCity = false;
                bool ReplComment = false;
                bool ReplPicture = false;
                bool Alarmtime = false;
                bool Faxtime = false;
                //bool getEinsatzort = false;

                foreach (string line in lines)
                {

                    string msg;
                    string prefix;
                    int x = line.IndexOf(':');
                    if (x != -1)
                    {
                        prefix = line.Substring(0, x);
                        msg = line.Substring(x + 1).Trim();

                        prefix = prefix.Trim().ToUpperInvariant();
                        switch (prefix)
                        {

                            //Füllen der Standardinformatione Alarmfax ILS FFB
                            case "EINSATZNR":
                            case "E — NR":
                            case "E-NR":
                            case "E-Nr":
                                operation.OperationNumber = msg;
                                break;
                            case "MITTEILER":
                                operation.Messenger = msg;
                                break;
                            case "EINSATZORT":
                                operation.Location = msg;
                                break;
                            case "STRAßE":
                            case "STRABE":
                                operation.Street = msg;
                                break;
                            case "KREUZUNG":
                                operation.CustomData["Intersection"] = msg;
                                break;
                            case "ORTSTEIL/ORT":
                                operation.City = msg;
                                break;
                            case "OBJEKT":
                            case "9BJEKT":
                                operation.Property = msg;
                                break;
                            case "MELDEBILD":
                                operation.Picture = msg;
                                break;
                            case "HINWEIS":
                                operation.Comment = msg;
                                break;
                            case "EINSATZPLAN":
                                operation.OperationPlan = msg;
                                break;
                            case "EINSATZSTICHWORT":
                                operation.EmergencyKeyword = msg;
                                break;
                        }
                    }

                    // TODO: ist noch mit der ILS FFB zu klären ob auf dem Fax die Alarmzeit wieder kommt. Daher aktuell Alarzeit noch mit Faxeingang gleich

                    // Anzeige des Zeitpunkts des Alarmeingangs
                    if (Alarmtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Alarmtime"] = "Alarmzeit: " + uhrzeit.ToString("HH:mm:ss ");
                        Alarmtime = true;
                    }

                    // Anzeige des Zeitpunkts des Faxeingangs
                    if (Faxtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Faxtime"] = "Faxeingang: " + uhrzeit.ToString("HH:mm:ss ");
                        Faxtime = true;
                    }

                    // Fahrzeug füllen wenn leer
                    if (ReplVehicle == false)
                    {
                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " ";
                        ReplVehicle = true;
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
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }

            return operation;
        }

        #endregion

    }
}
