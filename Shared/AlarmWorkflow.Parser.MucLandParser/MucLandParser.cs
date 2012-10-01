using System;
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

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            try
            {
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
                            case "EINSATZNR":
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
                                operation.Intersection = msg;
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
                                operation.Hint = msg;
                                break;
                            case "EINSATZPLAN":
                                operation.PlanOfAction = msg;
                                break;
                        }
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
