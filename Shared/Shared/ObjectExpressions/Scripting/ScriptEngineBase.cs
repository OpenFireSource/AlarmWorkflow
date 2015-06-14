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
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Shared.ObjectExpressions.Scripting
{
    abstract class ScriptEngineBase : DisposableObject, IScriptEngine
    {
        #region Properties

        /// <summary>
        /// Gets a dictionary containing all values that the client has set.
        /// </summary>
        protected IDictionary<string, object> SetValues { get; private set; }

        #endregion

        #region Constructors

        protected ScriptEngineBase()
        {
            SetValues = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, performs some initialization work before the script engine is used.
        /// </summary>
        /// <param name="serviceProvider">The global service provider that may be used to query services.</param>
        protected virtual void Initialize(IServiceProvider serviceProvider)
        {

        }

        /// <summary>
        /// When overridden in a derived class, performs some work when a variable is assigned to the script.
        /// The default implementation stores the value by its name in the <see cref="SetValues"/> dictionary.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual void Set(string name, object value)
        {
            this.SetValues[name] = value;
        }

        /// <summary>
        /// When overridden in a derived class, executes the script file contents as presented in the <paramref name="source"/> parameter,
        /// optionally making use of the provided arguments.
        /// </summary>
        /// <param name="source">The script file contents to execute.</param>
        /// <param name="args">An optional array containing arguments to use when executing. May be empty.</param>
        /// <returns>Any object representing the result returned from the script.</returns>
        /// <exception cref="CustomScriptExecutionException">An exception has occurred while executing the script.</exception>
        protected abstract object Execute(string source, object[] args);

        #endregion

        #region IScriptEngine Members

        void IScriptEngine.Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                Initialize(serviceProvider);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Initialize() failed! The script engine may not function properly. Please see log for further information.");
                Logger.Instance.LogException(this, ex);
            }
        }

        void IScriptEngine.Set(string name, object value)
        {
            try
            {
                Set(name, value);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Set() failed! The script engine may not function properly. Please see log for further information.");
                Logger.Instance.LogException(this, ex);
            }
        }

        object IScriptEngine.Execute(string source, object[] args)
        {
            /* A note on the exception handling:
             * - Rethrow CustomScriptExecutionException, because they are already the right exception type.
             * - However any other exception must be wrapped.
             */

            try
            {
                return Execute(source, args);
            }
            catch (CustomScriptExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomScriptExecutionException(ex, CustomScriptExecutionException.Reason.ScriptInvocationException);
            }
        }

        #endregion
    }
}
