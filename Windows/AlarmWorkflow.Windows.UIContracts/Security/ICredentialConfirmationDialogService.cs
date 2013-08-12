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

namespace AlarmWorkflow.Windows.UIContracts.Security
{
    /// <summary>
    /// Defines a means for a window that asks the user for confirmation (including credentials) in order to execute a function.
    /// </summary>
    public interface ICredentialConfirmationDialogService
    {
        /// <summary>
        /// Invokes the credentials-dialog which asks the user for credentials input, and returns the success of that operation.
        /// </summary>
        /// <param name="functionName">The name of the function that is about being invoked.</param>
        /// <param name="authorizationMode">The type of authorization to use.</param>
        /// <returns>The result of the credentials-dialog. This is true if the user is granted the rights, and false if not.</returns>
        bool Invoke(string functionName, AuthorizationMode authorizationMode);
    }
}