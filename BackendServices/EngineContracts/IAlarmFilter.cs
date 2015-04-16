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


namespace AlarmWorkflow.BackendService.EngineContracts
{
    /// <summary>
    /// Defines a method that can be used by <see cref="IAlarmSource"/> implementations to filter incoming alarms before processing them.
    /// See documentation for further information.
    /// </summary>
    public interface IAlarmFilter
    {
        /// <summary>
        /// Determines whether or not the provided raw alarm source text qualifies for being accepted and processed.
        /// This is used to realize a global black/whitelist feature by the alarm sources.
        /// </summary>
        /// <param name="source">The raw source text directly from the alarm source.</param>
        /// <returns>A boolean value indicating whether or not the source is accepted.
        /// This means that the source contains no words that are on the blacklist and contains at least one word from the whitelist (if words are specified for it).
        /// -or- false, if the input <paramref name="source"/> was null.</returns>
        bool QueryAcceptSource(string source);
    }
}
