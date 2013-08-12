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
    /// Represents the results of a user-defined property resolving operation.
    /// </summary>
    public class ResolveExpressionResult
    {
        #region Properties

        /// <summary>
        /// Gets/sets the status of resolving the property.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Gets/sets the resolved value. The value is only considered if <see cref="Success"/> evaluates to <c>true</c>.
        /// </summary>
        public object ResolvedValue { get; set; }

        /// <summary>
        /// Determines whether or not the <see cref="ResolvedValue"/> actually has a value after all.
        /// </summary>
        public bool ResolvedValueHasValue
        {
            get { return ResolvedValue != null; }
        }

        /// <summary>
        /// Returns a new instance of <see cref="ResolveExpressionResult"/> which indicates failure.
        /// </summary>
        public static ResolveExpressionResult Fail
        {
            get { return new ResolveExpressionResult(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveExpressionResult"/> class.
        /// </summary>
        public ResolveExpressionResult()
        {

        }

        #endregion
    }
}