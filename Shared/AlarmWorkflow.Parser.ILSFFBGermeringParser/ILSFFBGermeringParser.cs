using System;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSFFBGermeringParser
{
    [Export("ILSFFBGermeringParser", typeof(IFaxParser))]
    sealed class ILSFFBGermeringParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MucLandParser class.
        /// </summary>
        public ILSFFBGermeringParser()
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
                bool Other_FD_Upf = false;
                bool Other_FD_Ort = false;
                bool Other_FD_Bhf = false;
                bool Other_FD_Eic = false;
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
                            // Auslesen der Standardinformation Alarmfax ILS FFB
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

                    //TODO
                    //if ((line.StartsWith("EINSATZSTICHWORT")) && (getEinsatzort == false))
                    //{
                    //    operation.EmergencyKeyword = line.Substring(4);
                    //    getEinsatzort = true;
                    //}

                    //Auslesen der Fahrzeuge FF Germering und teile FF Unterpfaffenhofen
                    int x0 = line.IndexOf("FF Germering");
                    if (x0 != -1)
                    {

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + "FF Germering ";

                    }

                    int x1 = line.IndexOf("mering 40/1");
                    if (x1 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 40/1 " + geraet;

                    }

                    int x2 = line.IndexOf("mering 40/2");
                    if (x2 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 40/2 " + geraet;

                    }

                    int x3 = line.IndexOf("ring 30/1");
                    if (x3 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 30/1 " + geraet;

                    }

                    int x4 = line.IndexOf("mering 61/1");
                    if (x4 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 61/1 " + geraet;

                    }

                    int x5 = line.IndexOf("mering 81/1");
                    if (x5 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 81/1 " + geraet;

                    }

                    int x6 = line.IndexOf("mering 11/1");
                    if (x6 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 11/1 " + geraet;

                    }

                    int x8 = line.IndexOf("ering A-ÖSA");
                    if (x8 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | ÖSA " + geraet;

                    }

                    int x9 = line.IndexOf("ring A-P 250");
                    if (x9 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | P250 " + geraet;

                    }

                    int x10 = line.IndexOf("enhofen A-VSA");
                    if (x10 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | VSA Upf " + geraet;

                    }

                    int x11 = line.IndexOf("mering A-VSA");
                    if (x11 != -1)
                    {

                        int anfang = line.IndexOf('(');

                        string geraet = line.Substring(anfang + 1);       // Einführen einer Hilfsvariable "geraet"
                        geraet = geraet.Substring(0, geraet.Length - 2);  // schneidet  ')' ab
                        geraet = geraet.Trim();                                 // entfernt  ggf. vorhandene Leerzeichen am Anfang und Ende

                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | 81/1 + VSA " + geraet;

                    }

                    //Auswerten zusätzlich alarmierter Nachbarfeuerwehren
                    int x12 = line.IndexOf("FF Unterpf");
                    if (x12 != -1)
                    {

                        operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + " | Unterpfaffenhofen ";

                    }

                    int x13 = line.IndexOf("FL Unterpf");
                    if (x13 != -1)
                    {

                        operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + "| Unterpfaffenhofen ";

                    }

                    int x14 = line.IndexOf("FL Puchheim-Bahnhof");
                    if (x14 != -1)
                    {

                        operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + "| PuchheimBHF ";

                    }


                    int x15 = line.IndexOf("FL Eichen");
                    if (x15 != -1)
                    {

                        operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + "| Eichenau ";

                    }

                    int x16 = line.IndexOf("FL Puchheim-Ort");
                    if (x16 != -1)
                    {

                        operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + "| PuchheimOrt ";

                    }

                    //Ersetzen der anderen alarmierten Feuerwehren
                    operation.CustomData["OtherFD"] = operation.CustomData["OtherFD"] + " ";

                    if ((operation.GetCustomData<string>("OtherFD").Contains("Unterpfaffenhofen") == true) && (Other_FD_Upf == false))
                    {
                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | FF Unterpfaffenhofen ";
                        Other_FD_Upf = true;
                    }

                    if ((operation.GetCustomData<string>("OtherFD").Contains("PuchheimOrt") == true) && (Other_FD_Ort == false))
                    {
                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | FF Puchheim Ort ";
                        Other_FD_Ort = true;
                    }

                    if ((operation.GetCustomData<string>("OtherFD").Contains("PuchheimBHF") == true) && (Other_FD_Bhf == false))
                    {
                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | FF Puchheim Bahnhof ";
                        Other_FD_Bhf = true;
                    }

                    if ((operation.GetCustomData<string>("OtherFD").Contains("Eichenau") == true) && (Other_FD_Eic == false))
                    {
                        operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"] + " | FF Eichenau ";
                        Other_FD_Eic = true;
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

                    // Nur zu verwendne wenn auch in der Fahrzeuge ersetzt werden soll

                    //if (operation.CustomData["Vehicles"].Contains("ß") == true)
                    //{
                    //    operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"].Replace("ß", "ss");
                    //}

                    //if (operation.CustomData["Vehicles"].Contains("ä") == true)
                    //{
                    //    operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"].Replace("ä", "ae");
                    //}

                    //if (operation.CustomData["Vehicles"].Contains("ö") == true)
                    //{
                    //    operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"].Replace("ö", "oe");
                    //}

                    //if (operation.CustomData["Vehicles"].Contains("ü") == true)
                    //{
                    //    operation.CustomData["Vehicles"] = operation.CustomData["Vehicles"].Replace("ü", "ue");
                    //}

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

                    //Prüfen ob noch notwendig TODO!!!
                    //if (operation.Comment.Contains("EINSATZMITTEL") == true)
                    //{
                    //    operation.Comment = operation.Comment.Replace("EINSATZMITTEL:", " ");
                    //}


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