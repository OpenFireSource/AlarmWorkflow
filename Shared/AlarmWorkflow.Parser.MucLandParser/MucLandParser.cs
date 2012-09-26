using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using AlarmWorkflow.Shared.Alarmfax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Logging;

namespace AlarmWorkflow.Shared.AlarmfaxParser
{
    /// <summary>
    /// Description of MucLandParser.
    /// </summary>
    [Export("MucLandParser", typeof(IParser))]
    public class MucLandParser : IParser
    {
        #region Fields

        /// <summary>
        /// The Logger object.
        /// </summary>
        private ILogger logger = new Logging.NoLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MucLandParser class.
        /// </summary>
        /// <param name="logger">The logger object.</param>
        /// <param name="replaceList">The RreplaceList object.</param>
        public MucLandParser()
        {
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(IList<ReplaceString> replaceList, string file)
        {
            Operation einsatz = new Operation();
            string line = string.Empty;
            bool fileNotFound = true;
            int trys = 0;
            while (fileNotFound)
            {
                fileNotFound = false;
                trys++;
                try
                {
                    StreamReader reader = new StreamReader(file);
                    while ((line = reader.ReadLine()) != null)
                    {
                        string msg;
                        string prefix;
                        int x = line.IndexOf(':');
                        if (x != -1)
                        {
                            prefix = line.Substring(0, x);
                            msg = line.Substring(x + 1).Trim();
                            if (replaceList != null)
                            {
                                foreach (ReplaceString rps in replaceList)
                                {
                                    msg = msg.Replace(rps.OldString, rps.NewString);
                                }
                            }

                            prefix = prefix.Trim().ToUpperInvariant();
                            switch (prefix)
                            {
                                case "EINSATZNR":
                                    einsatz.Einsatznr = msg;
                                    break;
                                case "MITTEILER":
                                    einsatz.Mitteiler = msg;
                                    break;
                                case "EINSATZORT":
                                    einsatz.Einsatzort = msg;
                                    break;
                                case "STRAßE":
                                case "STRABE":
                                    einsatz.Strasse = msg;
                                    break;
                                case "KREUZUNG":
                                    einsatz.Kreuzung = msg;
                                    break;
                                case "ORTSTEIL/ORT":
                                    einsatz.Ort = msg;
                                    break;
                                case "OBJEKT":
                                case "9BJEKT":
                                    einsatz.Objekt = msg;
                                    break;
                                case "MELDEBILD":
                                    einsatz.Meldebild = msg;
                                    break;
                                case "HINWEIS":
                                    einsatz.Hinweis = msg;
                                    break;
                                case "EINSATZPLAN":
                                    einsatz.Einsatzplan = msg;
                                    break;
                            }
                        }
                    }

                    reader.Close();
                }
                catch (FileNotFoundException ex)
                {
                    if (trys < 10)
                    {
                        fileNotFound = true;
                        Thread.Sleep(1000);
                        this.logger.WriteWarning("Ocr file not found. Try " + trys.ToString(CultureInfo.InvariantCulture) + " of 10!");
                    }
                    else
                    {
                        this.logger.WriteError("Ocr File not found! " + ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.WriteError(ex.ToString());
                }
            }

            return einsatz;
        }

        #endregion

    }
}
