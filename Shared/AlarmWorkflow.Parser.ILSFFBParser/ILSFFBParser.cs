using System;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSFFBParser
{
    /// <summary>
    /// Description of ILSFFBParser.
    /// </summary>
    [Export("ILSFFBParser", typeof(IFaxParser))]
    sealed class ILSFFBParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ILSFFBParser class.
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
                bool ReplCity = false;
                bool ReplComment = false;
                bool ReplPicture = false;
                //bool Alarmtime = false;
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

                            //F�llen der Standardinformatione Alarmfax ILS FFB
                            case "EINSATZNR":
                            case "E � NR":
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
                            case "STRA�E":
                            case "STRABE":
                                operation.Street = msg;
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

                    // TODO: ist noch mit der ILS FFB zu kl�ren ob auf dem Fax die Alarmzeit wieder kommt. Daher aktuell Alarzeit noch mit Faxeingang gleich

                    // Anzeige des Zeitpunkts des Alarmeingangs
                    //if (Alarmtime == false)
                    //{
                    //    DateTime uhrzeit = DateTime.Now;
                    //    operation.CustomData["Alarmtime"] = "Alarmzeit: " + uhrzeit.ToString("HH:mm:ss ");
                    //    Alarmtime = true;
                    //}

                    // Anzeige des Zeitpunkts des Faxeingangs
                    if (Faxtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Faxtime"] = "Faxeingang: " + uhrzeit.ToString("HH:mm:ss ");
                        Faxtime = true;
                    }

                    // Fahrzeug f�llen TODO Check if needed in further cases
                    //operation.CustomData["Vehicles"] = "";
                    

                    // Sonderzeichenersetzung im Meldebild

                    if (ReplPicture == false)
                    {
                        operation.Picture = operation.Picture + " ";
                        ReplPicture = true;
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ss");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ae");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "oe");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung im Ort

                    if (ReplCity == false)
                    {
                        operation.City = operation.City + " ";
                        ReplCity = true;
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ss");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ae");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "oe");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung in der Strasse

                    if (ReplStreet == false)
                    {
                        operation.Street = operation.Street + " ";
                        ReplStreet = true;
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ss");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ae");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "oe");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung im Hinweis

                    if (ReplComment == false)
                    {
                        operation.Comment = operation.Comment + " ";
                        ReplComment = true;
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ss");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ae");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "oe");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ue");
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
