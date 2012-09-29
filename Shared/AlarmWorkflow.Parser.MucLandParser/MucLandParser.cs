using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Logging;

namespace AlarmWorkflow.Parser.MucLandParser
{
    /// <summary>
    /// Description of MucLandParser.
    /// </summary>
    public class MucLandParser : IParser
    {
        #region Fields

        /// <summary>
        /// The Logger object.
        /// </summary>
        private ILogger logger = new NoLogger();

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
                                    einsatz.OperationNumber = msg;
                                    break;
                                case "MITTEILER":
                                    einsatz.Messenger = msg;
                                    break;
                                case "EINSATZORT":
                                    einsatz.Location = msg;
                                    break;
                                case "STRAßE":
                                case "STRABE":
                                    einsatz.Street = msg;
                                    break;
                                case "KREUZUNG":
                                    einsatz.Intersection = msg;
                                    break;
                                case "ORTSTEIL/ORT":
                                    einsatz.City = msg;
                                    break;
                                case "OBJEKT":
                                case "9BJEKT":
                                    einsatz.Property = msg;
                                    break;
                                case "MELDEBILD":
                                    einsatz.Picture = msg;
                                    break;
                                case "HINWEIS":
                                    einsatz.Hint = msg;
                                    break;
                                case "EINSATZPLAN":
                                    einsatz.PlanOfAction = msg;
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
