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
using Jint;

namespace AlarmWorkflow.Shared.ObjectExpressions.Scripting
{
    /// <summary>
    /// Represents a script engine that executes user-written JavaScript code.
    /// See documentation for further information.
    /// </summary>
    /// <remarks><para>This class allows the user to write some code in JavaScript, which is then compiled by this formatter and invoked when being used.
    /// Writing a script in JavaScript has a number of advantages over writing a script in C#:
    /// <list type="bullet">
    /// <item>Simple syntax</item>
    /// <item>Higher security (no access to the .NET library)</item>
    /// <item>No caching needs to performed</item>
    /// </list>
    /// <example><code>
    /// /* See below an example of a JavaScript script file.
    ///  */
    /// function getResult(graph) {
    ///     return graph.Einsatzort.ZipCode;
    /// }
    /// 
    /// /* The global 'G' variable contains the 'graph' object which is to be formatted.
    ///  * When the script file concludes, the completion value is returned.
    ///  * You don't need to 'return' anything by yourself.
    ///  */
    /// getResult(G);
    /// </code></example>
    /// </para><para>
    /// Additional information:
    /// <list type="bullet">
    /// <item>jQuery is currently not explicitly supported. Plans to support jQuery exist.</item>
    /// </list>
    /// </para></remarks>
    class JavaScriptEngine : ScriptEngineBase
    {
        #region Fields

        private Engine _engine;

        #endregion

        #region Constructors

        internal JavaScriptEngine()
        {
        }

        #endregion

        #region Methods

        protected override void Initialize(IServiceProvider serviceProvider)
        {
            base.Initialize(serviceProvider);

            _engine = new Engine(_ => _.Strict());
        }

        protected override object Execute(string source, object[] args)
        {
            object graph = SetValues["graph"];

            _engine.SetValue("G", graph);

            return _engine.Execute(source).GetCompletionValue().ToString();
        }

        protected override void DisposeCore()
        {
            _engine = null;
        }

        #endregion
    }
}
