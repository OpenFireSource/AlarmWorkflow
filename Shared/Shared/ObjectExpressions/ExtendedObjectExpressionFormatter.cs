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
using System.IO;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.ObjectExpressions.Scripting;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents an <see cref="ObjectExpressionFormatter&lt;TInput&gt;"/> that can use script files to format an object.
    /// </summary>
    /// <typeparam name="TInput">The type of the object to format.</typeparam>
    public class ExtendedObjectExpressionFormatter<TInput> : ObjectExpressionFormatter<TInput>
    {
        #region Constants

        private const string CustomScriptIntroductoryString = "$";

        #endregion

        #region Fields

        private static IScriptEngineFactory _scriptEngineFactory;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the last error that has occurred during compilation.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This property will only have a value if compilation was performed and it failed.
        /// In any other case this property will have no value. It is also resetted when ToString() is called again.</remarks>
        public CustomScriptExecutionException Error { get; private set; }

        /// <summary>
        /// Determines whether or not an error has occurred so far.
        /// </summary>
        public bool HasError
        {
            get { return Error != null; }
        }

        #endregion

        #region Constructors

        static ExtendedObjectExpressionFormatter()
        {
            _scriptEngineFactory = new ScriptEngineFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedObjectExpressionFormatter&lt;TInput&gt;"/> class.
        /// </summary>
        public ExtendedObjectExpressionFormatter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedObjectExpressionFormatter&lt;TInput&gt;"/> class
        /// and optionally uses a custom callback to resolve expressions that could not be resolved automatically.
        /// </summary>
        /// <param name="resolver">The resolver that shall be used when an expression could not be resolved.</param>
        public ExtendedObjectExpressionFormatter(ExpressionResolver<TInput> resolver)
            : base(resolver)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to perform two passes: 1st pass invokes the custom scripts (if any), 2nd pass processes the results from the first pass.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="format">The format string, using the property values in curly braces (expressions), like {Property}. Must not be empty.</param>
        /// <returns>The formatted string.</returns>
        public override string ToString(TInput graph, string format)
        {
            // Do it twice:
            // - First pass goes over all macros and processes them (including script references)
            // - Second pass does it again and replaces macros potentially created by custom scripts.
            for (int i = 0; i < 2; i++)
            {
                format = base.ToString(graph, format);
            }
            return format;
        }

        /// <summary>
        /// Overridden to process custom script expressions.
        /// </summary>
        /// <param name="graph">The object graph to use.</param>
        /// <param name="macro">The macro, hopefully representing a custom script statement. If this is not the case, then the base method does the processing.</param>
        /// <param name="expression">See base class.</param>
        /// <returns>See base class.</returns>
        protected override string ProcessMacro(TInput graph, string macro, string expression)
        {
            if (expression.StartsWith(CustomScriptIntroductoryString))
            {
                expression = expression.Remove(0, CustomScriptIntroductoryString.Length);

                string filePath = null;
                IScriptEngine engine = null;

                if (TrySplitExpression(expression, out filePath, out engine))
                {
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Utilities.GetWorkingDirectory(), filePath);
                    }

                    using (engine)
                    {
                        return GetResultFromScriptEngine(graph, engine, filePath);
                    }
                }
            }

            // Otherwise let the base class' method do its job.
            return base.ProcessMacro(graph, macro, expression);
        }

        private bool TrySplitExpression(string expression, out string filePath, out IScriptEngine engine)
        {
            engine = null;
            filePath = null;

            int iEqualsSign = expression.IndexOf('=');
            if (iEqualsSign > 0)
            {
                IScriptEngine engineTmp = _scriptEngineFactory.CreateEngine(expression.Substring(0, iEqualsSign));

                if (engineTmp != null)
                {
                    filePath = expression.Remove(0, iEqualsSign + 1);
                    engine = engineTmp;

                    return true;
                }
            }

            return false;
        }

        private string GetResultFromScriptEngine(object graph, IScriptEngine engine, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.ScriptFileNotFound);
            }

            engine.Initialize(ServiceProvider.Instance);
            engine.Set("graph", graph);

            try
            {
                object[] args = new object[0];

                object result = engine.Execute(File.ReadAllText(filePath), args);

                if (result != null)
                {
                    return result.ToString();
                }
            }
            catch (CustomScriptExecutionException ex)
            {
                this.Error = ex;

                Logger.Instance.LogException(this, ex);
                return Properties.Resources.CustomScriptInvocationFailed;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }

            return string.Empty;
        }

        #endregion
    }
}
