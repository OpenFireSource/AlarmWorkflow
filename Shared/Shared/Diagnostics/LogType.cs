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


namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// The type of the logging message.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Avoid using.
        /// </summary>
        None = 0,
        /// <summary>
        /// Log something on DEBUG Level (only when debug mode is true).
        /// Also used on Exceptions, since they can't be logged on console.
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Log something on TRACE level.
        /// Usually done when something is extremely unimportant yet interesting for record (i.e. error reports).
        /// </summary>
        Trace = 2,
        /// <summary>
        /// Log something on INFO Level.
        /// </summary>
        Info = 3,
        /// <summary>
        /// Log something on WARNING Level.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Log something on ERROR Level.
        /// </summary>
        Error = 5,
        /// <summary>
        /// Log something on EXCEPTION Level (only on Win32).
        /// These are not supposed to be used manually!
        /// </summary>
        Exception = 10,
        /// <summary>
        /// Message emitted by the console.
        /// </summary>
        Console = 11,
    }
}