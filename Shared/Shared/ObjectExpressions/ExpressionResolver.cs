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


namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents a method that gets called by <see cref="ObjectExpressionFormatter{TInput}"/> if it couldn't resolve a specific path.
    /// </summary>
    /// <typeparam name="TInput">The type of the object that is in focus.</typeparam>
    /// <param name="graph">The object that is in focus.</param>
    /// <param name="expression">The expression that needs to be resolved.</param>
    /// <returns>An instance of <see cref="ResolveExpressionResult"/> which represents the result from this operation.</returns>
    public delegate ResolveExpressionResult ExpressionResolver<TInput>(TInput graph, string expression);
}