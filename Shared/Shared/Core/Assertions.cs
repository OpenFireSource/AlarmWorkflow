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


namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Defines methods that can be used to assert cases; may be used as extensions.
    /// </summary>
    public static class Assertions
    {
        /// <summary>
        /// Asserts that a given variable is not null, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c>.</exception>
        public static void AssertNotNull(object value, string name)
        {
            AssertNotNull(value, name, "Value is null!");
        }
        /// <summary>
        /// Asserts that a given variable is not null, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="message">The message to display.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c>.</exception>
        public static void AssertNotNull(object value, string name, string message)
        {
            if (value == null)
            {
                throw new AssertionFailedException(message, AssertionType.AssertNotNull, new string[] { name });
            }
        }
        /// <summary>
        /// Asserts that a given string is not empty, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The string-value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c> or empty.</exception>
        public static void AssertNotEmpty(string value, string name)
        {
            AssertNotEmpty(value, name, "Value is empty!");
        }
        /// <summary>
        /// Asserts that a given string is not empty, and raises an exception if it is.
        /// </summary>
        /// <param name="value">The string-value to check.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="message">The message to display.</param>
        /// <exception cref="AssertionFailedException">Raised if the <paramref name="value"/> is <c>null</c> or empty.</exception>
        public static void AssertNotEmpty(string value, string name, string message)
        {
            AssertNotNull(name, "name");
            if (string.IsNullOrEmpty(value))
            {
                throw new AssertionFailedException(message, AssertionType.AssertNotEmpty, new string[] { name });
            }
        }
    }
}