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


namespace AlarmWorkflow.Job.eAlarm
{
    /// <summary>
    /// Content of the eAlarm message.
    /// The vars have to be lowercase!
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Representation of the operationdata.
        /// </summary>
        public Data data;

        /// <summary>
        /// The recipients.
        /// </summary>
        public string[] registration_ids;

        /// <summary>
        /// The transferred operationdata.
        /// </summary>
        public class Data
        {
            /// <summary>
            /// The location of the operation. (Required by the geocoding part of the mobile app).
            /// </summary>
            public string awf_location;

            /// <summary>
            /// The content of the message.
            /// </summary>
            public string awf_message;

            /// <summary>
            /// The title of the message.
            /// </summary>
            public string awf_title;
        }
    }
}

