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
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents an exception that occurred while executing a custom C# script.
    /// </summary>
    [Serializable()]
    public class CustomScriptExecutionException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the reason why compilation has failed.
        /// </summary>
        public Reason FailureReason { get; private set; }

        #endregion

        #region Constructors

        private CustomScriptExecutionException()
            : base(Properties.Resources.CustomScriptExecutionExceptionMessage)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomScriptExecutionException"/> class.
        /// </summary>
        /// <param name="reason">The reason why execution has failed.</param>
        public CustomScriptExecutionException(Reason reason)
            : this(null, reason)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomScriptExecutionException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception, if any.</param>
        /// <param name="reason">The reason why execution has failed.</param>
        public CustomScriptExecutionException(Exception innerException, Reason reason)
            : base(GetTranslatedErrorMessage(reason), innerException)
        {
            FailureReason = reason;
        }

        #endregion

        #region Methods

        private static string GetTranslatedErrorMessage(Reason reason)
        {
            switch (reason)
            {
                case Reason.ScriptFileNotFound:
                    return Resources.CustomScriptExecutionExceptionScriptFileNotFound;
                case Reason.NotExactlyOneExportedTypeFound:
                    return Resources.CustomScriptExecutionExceptionNotExactlyOneExportedTypeFound;
                case Reason.CompilationFailed:
                    return Resources.CustomScriptExecutionExceptionCompilationFailed;
                case Reason.ScriptFunctionNotFound:
                    return Resources.CustomScriptExecutionExceptionScriptFunctionNotFound;
                case Reason.ScriptFunctionMethodHasWrongSignature:
                    return Resources.CustomScriptExecutionExceptionScriptFunctionMethodHasWrongSignature;
                case Reason.ScriptInvocationException:
                    return Resources.CustomScriptExecutionExceptionScriptInvocationException;
                case Reason.Undefined:
                default:
                    return Resources.CustomScriptExecutionExceptionUndefined;
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Defines the reason type of this exception.
        /// </summary>
        public enum Reason
        {
            /// <summary>
            /// An undefined error has occurred.
            /// </summary>
            Undefined = 0,
            /// <summary>
            /// The script file wasn't found on disk.
            /// </summary>
            ScriptFileNotFound,
            /// <summary>
            /// Either zero or more than one exported (public) type was found in the script.
            /// </summary>
            NotExactlyOneExportedTypeFound,
            /// <summary>
            /// Compilation of the script file has failed.
            /// </summary>
            CompilationFailed,
            /// <summary>
            /// There was no method found, whose name matches the one looked after.
            /// </summary>
            ScriptFunctionNotFound,
            /// <summary>
            /// The script function method has the wrong signature.
            /// </summary>
            ScriptFunctionMethodHasWrongSignature,
            /// <summary>
            /// The script was successfully compiled, but an exception has occurred while invoking it.
            /// </summary>
            ScriptInvocationException,
        }

        #endregion
    }
}
