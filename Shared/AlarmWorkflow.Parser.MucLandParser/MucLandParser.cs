using System;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.MucLandParser
{
    /// <summary>
    /// Description of MucLandParser.
    /// </summary>
    [Export("MucLandParser", typeof(IFaxParser))]
    sealed class MucLandParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MucLandParser class.
        /// </summary>
        public MucLandParser()
        {
        }

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            lines = Utilities.Trim(lines);

            foreach (string line in lines)
            {
                try
                {
                    string msg;
                    string prefix;
                    int x = line.IndexOf(':');
                    if (x == -1)
                    {
                        continue;
                    }

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
                            operation.Einsatzort.Location = msg;
                            break;
                        case "STRAßE":
                        case "STRABE":
                            operation.Einsatzort.Street = msg;
                            break;
                        case "KREUZUNG":
                            operation.Einsatzort.Intersection = msg;
                            break;
                        case "ORTSTEIL/ORT":
                            operation.Einsatzort.City = msg;
                            break;
                        case "OBJEKT":
                        case "9BJEKT":
                            operation.Einsatzort.Property = msg;
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
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(this, ex);
                }
            }

            return operation;
        }

        #endregion

    }
}
