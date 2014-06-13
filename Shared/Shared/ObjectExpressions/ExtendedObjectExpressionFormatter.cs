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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using Microsoft.CSharp;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents an <see cref="ObjectExpressionFormatter&lt;TInput&gt;"/> that additionally allows using a custom C#-script to provide the formatting.
    /// See documentation for further information.
    /// </summary>
    /// <remarks><para>This class allows the user to write some code in C#, which is then compiled by this formatter and invoked when being used.
    /// This has the advantage of letting the user define much complexer and more advanced statements instead of what the basic formatter offers.</para>
    /// <para>The disadvantage of this however is a slight performance impact and writing complexer code is required.</para>
    /// <para>See below for a simplified example showing the needed types and methods. Naming and visibility as shown in the example code is mandatory.</para>
    /// <example><code>
    /// // Class name may be anything. Important is that there is exactly one public, non-static type defined.
    /// public class Script
    /// {
    ///     // Method name has to be 'Function'. Other than that, you may define any additional (non-public) types or methods.
    ///     public static string Function(object graph)
    ///     {
    ///         // Cast 'graph' to the type given to the formatter and perform some work.
    ///         // Then, return the string like below.
    ///         // The outcome of this function is then again processed by the underlying formatter,
    ///         // so you may as well return: 'My value = {MyProperty}'.
    ///         return "42";
    ///     }
    /// }
    /// </code></example>
    /// <para>You may need to declare some using-statements. In principal these are allowed, however only a small subset of assemblies
    /// is referenced by the in-memory compiler. A list of these include:
    /// <list type="bullet">
    /// <item>mscorlib.dll</item>
    /// <item>System.dll</item>
    /// <item>System.Core.dll</item>
    /// <item>System.Xml.dll</item>
    /// <item>System.Xml.Linq.dll</item>
    /// <item>Additionally: The assembly in which the type <typeparamref name="TInput"/> is declared</item>
    /// </list></para></remarks>
    /// <typeparam name="TInput">The type of the object to format.</typeparam>
    public class ExtendedObjectExpressionFormatter<TInput> : ObjectExpressionFormatter<TInput>
    {
        #region Constants

        /// <summary>
        /// Defines the token with which a format string shall start in order to represent
        /// a reference to a script that will be invoked to yield the contents of the format string.
        /// </summary>
        public const string ExecuteCSharpCodeExpressionHead = "$cs=";
        /// <summary>
        /// Defines the name of the static method that represents the custom script method that is invoked.
        /// </summary>
        public const string StaticFunctionName = "Function";

        #endregion

        #region Fields

        private static readonly Dictionary<string, WeakReference> CachedCompiledAssemblies;

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
            CachedCompiledAssemblies = new Dictionary<string, WeakReference>();
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
            if (expression.StartsWith(ExecuteCSharpCodeExpressionHead))
            {
                string filePath = expression.Remove(0, ExecuteCSharpCodeExpressionHead.Length);
                if (!Path.IsPathRooted(filePath))
                {
                    filePath = Path.Combine(Utilities.GetWorkingDirectory(), filePath);
                }

                return GetResultFromCSharpCode(graph, filePath);
            }

            // Otherwise let the base class' method do its job.
            return base.ProcessMacro(graph, macro, expression);
        }

        private string GetResultFromCSharpCode(TInput graph, string path)
        {
            try
            {
                Assembly cachedAssembly = GetCachedGeneratedAssemblyOrCompile(path);

                if (cachedAssembly.GetExportedTypes().Length != 1)
                {
                    throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.NotExactlyOneExportedTypeFound);
                }

                Type execType = cachedAssembly.GetExportedTypes()[0];

                MethodInfo execFunction = execType.GetMethod(StaticFunctionName, BindingFlags.Static | BindingFlags.Public);
                if (execFunction == null)
                {
                    throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.ScriptFunctionNotFound);
                }

                if (execFunction.ReturnType != typeof(string) || execFunction.GetParameters().Length != 1)
                {
                    throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.ScriptFunctionMethodHasWrongSignature);
                }

                ParameterInfo graphParameter = execFunction.GetParameters()[0];
                if (graphParameter.ParameterType != typeof(object))
                {
                    throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.ScriptFunctionMethodHasWrongSignature);
                }

                try
                {
                    string result = (string)execFunction.Invoke(null, new object[] { graph });

                    this.Error = null;

                    return result;
                }
                catch (TargetInvocationException ex)
                {
                    Logger.Instance.LogException(this, ex.InnerException);
                    throw new CustomScriptExecutionException(ex, CustomScriptExecutionException.Reason.ScriptInvocationException);
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

            return "";
        }

        private static Assembly GetCachedGeneratedAssemblyOrCompile(string path)
        {
            Assembly cachedAssembly = null;
            if (CachedCompiledAssemblies.ContainsKey(path))
            {
                WeakReference cachedAssemblyRef = CachedCompiledAssemblies[path];
                if (cachedAssemblyRef.IsAlive)
                {
                    cachedAssembly = (Assembly)cachedAssemblyRef.Target;
                }
                else
                {
                    CachedCompiledAssemblies.Remove(path);
                }
            }

            if (cachedAssembly == null)
            {
                cachedAssembly = CompileAssemblyFromFile(path);
                CachedCompiledAssemblies[path] = new WeakReference(cachedAssembly);
            }
            return cachedAssembly;
        }

        private static Assembly CompileAssemblyFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.ScriptFileNotFound);
            }

            CodeDomProvider provider = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.CompilerOptions = "/optimize";

            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");

            // Reference the AlarmWorkflow.Shared.dll as well.
            parameters.ReferencedAssemblies.Add(typeof(ExtendedObjectExpressionFormatter<>).Assembly.Location);
            // Also reference the assembly which contains the object to be formatted.
            parameters.ReferencedAssemblies.Add(typeof(TInput).Assembly.Location);

            Stopwatch sw = Stopwatch.StartNew();
            CompilerResults results = provider.CompileAssemblyFromFile(parameters, path);
            sw.Stop();
            Logger.Instance.LogFormat(LogType.Info, null, Properties.Resources.CustomScriptCompilationFinished, sw.ElapsedMilliseconds);

            if (results.Errors.Count > 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, null, Properties.Resources.CustomScriptCompilationWithErrorsWarnings, results.Errors.Count, path);
                foreach (CompilerError item in results.Errors)
                {
                    Logger.Instance.LogFormat(LogType.Warning, null, "{0}", item);
                }

                if (results.Errors.HasErrors)
                {
                    throw new CustomScriptExecutionException(CustomScriptExecutionException.Reason.CompilationFailed);
                }
            }

            return results.CompiledAssembly;
        }

        #endregion
    }
}
