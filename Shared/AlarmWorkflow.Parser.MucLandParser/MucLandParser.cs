using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.MucLandParser
{
    /// <summary>
    /// Description of MucLandParser.
    /// </summary>
    public class MucLandParser : IParser
    {
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
            int tries = 0;
            while (fileNotFound)
            {
                fileNotFound = false;
                tries++;
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
                    if (tries < 10)
                    {
                        fileNotFound = true;
                        Thread.Sleep(1000);
                        Logger.Instance.LogFormat(LogType.Warning, this, "Ocr file not found. Try {0} of 10!", tries);
                    }
                    else
                    {
                        Logger.Instance.LogFormat(LogType.Warning, this, "Ocr File not found! " + ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, ex.ToString());
                }
            }

            return einsatz;
        }

        #endregion

    }
}
