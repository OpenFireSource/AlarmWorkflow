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


namespace AlarmWorkflow.Job.Divera
{
    /// <summary>
    /// Content of the Divera Web-API message.
    /// The vars have to be lowercase!
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Keyword (30 zeichen)
        /// </summary>
        public string type;

        /// <summary>
        /// Priority, with or without blue flashing light
        /// (0 = ohne, 1 = mit)
        /// </summary>
        public int priority;

        /// <summary>
        /// text (1000 Zeichen)
        /// </summary>
        public string text;

        /// <summary>
        /// address
        /// </summary>
        public string address;

        /// <summary>
        /// longitude
        /// </summary>
        public float? lng;

        /// <summary>
        /// latitude
        /// </summary>
        public float? lat;

        /// <summary>
        /// loops, like defined in in the field "Alarmierungs-RIC", comma seperated
        /// </summary>
        public string ric;

        /// <summary>
        /// vehicles, like defined in the field "Alarmierungs-RIC", comma seperated
        /// </summary>
        public string vehicle;

        /// <summary>
        /// seconds since the alarm is done. For the monitor.
        /// </summary>
        public int delay;

        public Content()
        {
            priority = 1;
        }
    }
}

