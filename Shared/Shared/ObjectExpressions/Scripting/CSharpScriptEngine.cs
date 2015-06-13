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
using System.Reflection;
using AlarmWorkflow.Shared.Diagnostics;
using Microsoft.CSharp;

namespace AlarmWorkflow.Shared.ObjectExpressions.Scripting
{
    /// <summary>
    /// Represents a script engine that executes user-written C# code.
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
    /// <item>Additionally: The assembly in which the source object is declared</item>
    /// </list></para></remarks>
    class CSharpScriptEngine : ScriptEngineBase
    {
        #region Constants

        /// <summary>
        /// Defines the name of the static method that represents the custom script method that is invoked.
        /// </summary>
        public const string StaticFunctionName = "Function";

        #endregion

        #region Fields

        private static readonly Dictionary<int, WeakReference> CachedCompiledAssemblies;

        #endregion

        #region Constructors

        static CSharpScriptEngine()
        {
            CachedCompiledAssemblies = new Dictionary<int, WeakReference>();
        }

        internal CSharpScriptEngine()
            : base()
        {
        }

        #endregion

        #region Methods

        protected override object Execute(string source, object[] args)
        {
            object graph = SetValues["graph"];

            return GetResultFromCSharpCode(graph, source);
        }

        private static string GetResultFromCSharpCode(object graph, string source)
        {
            Assembly cachedAssembly = GetCachedGeneratedAssemblyOrCompile(graph, source);

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
                return (string)execFunction.Invoke(null, new object[] { graph });
            }
            catch (TargetInvocationException ex)
            {
                Logger.Instance.LogException(typeof(CSharpScriptEngine), ex.InnerException);
                throw new CustomScriptExecutionException(ex, CustomScriptExecutionException.Reason.ScriptInvocationException);
            }
        }

        private static Assembly GetCachedGeneratedAssemblyOrCompile(object graph, string source)
        {
            Assembly cachedAssembly = null;
            int sourceHash = source.GetHashCode();
            if (CachedCompiledAssemblies.ContainsKey(sourceHash))
            {
                WeakReference cachedAssemblyRef = CachedCompiledAssemblies[sourceHash];
                if (cachedAssemblyRef.IsAlive)
                {
                    cachedAssembly = (Assembly)cachedAssemblyRef.Target;
                }
                else
                {
                    CachedCompiledAssemblies.Remove(sourceHash);
                }
            }

            if (cachedAssembly == null)
            {
                cachedAssembly = CompileAssemblyFromFile(graph, source);
                CachedCompiledAssemblies[sourceHash] = new WeakReference(cachedAssembly);
            }

            return cachedAssembly;
        }

        private static Assembly CompileAssemblyFromFile(object graph, string source)
        {
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
            parameters.ReferencedAssemblies.Add(graph.GetType().Assembly.Location);

            Stopwatch sw = Stopwatch.StartNew();
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);
            sw.Stop();
            Logger.Instance.LogFormat(LogType.Info, null, Properties.Resources.CustomScriptCompilationFinished, sw.ElapsedMilliseconds);

            if (results.Errors.Count > 0)
            {
                Logger.Instance.LogFormat(LogType.Warning, null, Properties.Resources.CustomScriptCompilationWithErrorsWarnings, results.Errors.Count, source);
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

        protected override void DisposeCore()
        {
            /* Nothing to dispose of here.
             * The script assemblies are cached and garbage-collected automatically.
             */
        }

        #endregion
    }
}
