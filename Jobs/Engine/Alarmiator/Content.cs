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


namespace AlarmWorkflow.Job.Alarmiator
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
        public Notification notification;

        /// <summary>
        /// Representation of the operationdata.
        /// </summary>
        public Data data;


        /// <summary>
        /// The transferred operationdata.
        /// </summary>
        public class Notification
        {
            /// <summary>
            /// Representation of the alert title.
            /// </summary>
            public string alert;


            /// <summary>
            /// Representation of the alert title.
            /// </summary>
            public string title;

            /// <summary>
            /// Representation of the alert body.
            /// </summary>
            public string body;

            /// <summary>
            /// Representation of the alert sound.
            /// </summary>
            public string sound;

        }


        /// <summary>
        /// The recipients.
        /// </summary>
        public string[] registration_ids;

        /// <summary>
        /// Priority of the message
        /// </summary>
        public string priority;


        /// <summary>
        /// have iOS process the payload of the APNS Notification
        /// </summary>
        public bool content_available;


        /// <summary>
        /// The transferred operationdata.
        /// </summary>
        public class Data
        {   
            public string oplat;
            public string oplon;

            /// <summary>
            /// The content of the message.
            /// </summary>
            public string opdesc;

            /// <summary>
            /// The title of the message.
            /// </summary>
            public string opkeyword;

            /// <summary>
            /// The title of the message.
            /// </summary>
            public string opid;

            /// <summary>
            /// The timestamp of the operation.
            /// </summary>
            public string optimestamp;

            /// <summary>
            /// have iOS process the payload of the APNS Notification
            /// </summary>
            public string content_available;

            /// <summary>
            /// The Timestamp the operation came in at AlarmWorkflow
            /// </summary>
            public string optimestampIncoming;
        }

        public Content()
        {
            priority = "high";
            content_available = true;
        }
    }
}

