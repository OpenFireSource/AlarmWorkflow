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

namespace AlarmWorkflow.Shared.ObjectExpressions.Scripting
{
    class ScriptEngineFactory : IScriptEngineFactory
    {
        #region Constants

        private const string IdCSharp = "cs";

        #endregion

        #region Constructors

        internal ScriptEngineFactory()
        {
        }

        #endregion

        #region IScriptEngineFactory implementation

        IScriptEngine IScriptEngineFactory.CreateEngine(string id)
        {
            if (id.Equals(IdCSharp, StringComparison.OrdinalIgnoreCase))
            {
                return new CSharpScriptEngine();
            }

            return null;
        }

        #endregion
    }
}
