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
    [Export("FEZMuenchenLandParser", typeof(IParser))]
    sealed class FEZMuenchenLandParser : IParser
    {

        #region IFaxParser Members

        Operation IParser.Parse(string[] lines)
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
