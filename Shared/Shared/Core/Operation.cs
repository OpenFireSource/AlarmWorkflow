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
using System.Collections.Generic;
using System.Text;
using AlarmWorkflow.Shared.ObjectExpressions;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents an operation, which was created by analyzing and parsing an incoming alarmfax.
    /// </summary>
    [Serializable()]
    public sealed class Operation : IEquatable<Operation>, IFormattable
    {
        #region Constants

        /// <summary>
        /// Defines the default timespan after which new operations/alarms are set to "acknowledged". See "IsAcknowledged"-property for further information.
        /// </summary>
        public static readonly TimeSpan DefaultAcknowledgingTimespan = TimeSpan.FromHours(8d);

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the unique Id of this operation.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets/sets the guid of this operation that is globally unique.
        /// </summary>
        public Guid OperationGuid { get; set; }
        /// <summary>
        /// Gets/sets the timestamp of when the operation materialized ("incoming" timestamp).
        /// For the actual alarm timestamp, use the property <see cref="P:Timestamp"/>.
        /// </summary>
        public DateTime TimestampIncome { get; set; }
        /// <summary>
        /// Gets/sets the date and time of the actual alarm.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the Einsatznummer object.
        /// </summary>
        public string OperationNumber { get; set; }
        /// <summary>
        /// Gets or sets the Mitteiler object.
        /// </summary>
        public string Messenger { get; set; }
        /// <summary>
        /// Gets/sets the priority of this operation.
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// Gets/sets the "Einsatzort" (place of action).
        /// Usually this location contains the destination spot.
        /// </summary>
        public PropertyLocation Einsatzort { get; set; }
        /// <summary>
        /// Gets/sets the "Zielort" (destination location).
        /// This is usually empty.
        /// </summary>
        public PropertyLocation Zielort { get; set; }
        /// <summary>
        /// Gets/sets the comment text. Usually this contains the result from the "Bemerkung" or "Hinweis" (etc.)-sections.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Gets the Meldebild object.
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// Gets the Einsatzplan object.
        /// </summary>
        public string OperationPlan { get; set; }
        /// <summary>
        /// Gets/sets the keywords for this operation.
        /// </summary>
        public OperationKeywords Keywords { get; set; }
        /// <summary>
        /// Gets/sets the list of all resources requested by the call center.
        /// </summary>
        public OperationResourceCollection Resources { get; set; }
        /// <summary>
        /// Gets/sets the custom data for this operation.
        /// </summary>
        public IDictionary<string, object> CustomData { get; set; }
        /// <summary>
        /// Gets/sets whether or not this operation is acknowledged, that means that this operation is no longer necessary to be displayed in the UI as "fresh".
        /// If this is set to "false" then this operation will always been shown in the UI. By default, an operation is set to "acknowledged"
        /// either if the user manually acknowledges it or after a defined timespan (usually 8 hours).
        /// </summary>
        public bool IsAcknowledged { get; set; }
        /// <summary>
        /// Gets/sets the loop information that is associated with this operation.
        /// </summary>
        public OperationLoopCollection Loops { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class and sets the value of the <see cref="P:TimestampIncome"/>-property to <see cref="P:DateTime.Now"/>.
        /// </summary>
        public Operation()
        {
            CustomData = new Dictionary<string, object>();
            Resources = new OperationResourceCollection();
            OperationGuid = Guid.NewGuid();
            Loops = new OperationLoopCollection();

            Einsatzort = new PropertyLocation();
            Zielort = new PropertyLocation();
            Keywords = new OperationKeywords();

            TimestampIncome = DateTime.Now;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Looks up the custom data with the given name, returns the value if it was found, or returns a default value if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetCustomData<T>(string name)
        {
            if (CustomData.ContainsKey(name))
            {
                return (T)CustomData[name];
            }
            return default(T);
        }

        /// <summary>
        /// Gets the location information as a <see cref="PropertyLocation"/>-object.
        /// </summary>
        /// <returns>The location information as a <see cref="PropertyLocation"/>-object.</returns>
        public PropertyLocation GetDestinationLocation()
        {
            // TODO: If "Zielort" has a meaningful location, return that one instead?
            return this.Einsatzort;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/>-parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            if (obj is Operation)
            {
                return this.Equals((Operation)obj);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a string representation of the current operation similar to:
        /// "(OperationNumber) Timestamp, Location". For a custom formatting check out <see cref="M:ToString(string)"/>.
        /// </summary>
        /// <returns>A string representation of the current operation.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("({0}) {1}, {2}", OperationNumber, Timestamp, GetDestinationLocation().ToString());
            return sb.ToString();
        }

        #endregion

        #region IEquatable<Operation> Member

        /// <summary>
        /// Determines whether the specified <see cref="Operation"/> is equal to this instance.
        /// Two <see cref="Operation"/> are considered equal if they have the same <see cref="P:Id"/>.
        /// </summary>
        /// <param name="other">The <see cref="Operation"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Operation"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Operation other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Id == this.Id;
        }

        #endregion

        #region IFormattable Members

        /// <summary>
        /// Converts the value of the current <see cref="Operation"/> object to its equivalent string representation
        /// using the specified format.
        /// </summary>
        /// <param name="format">The format string. May contain the names of the properties to print enclosed in curly braces like '{<see cref="P:OperationNumber"/>}'.
        /// If a given property could not be found on the top-level, then it is looked after in the CustomData dictionary.
        /// If it wasn't found there either, a default string is printed.</param>
        /// <returns>A string representation of value of the current <see cref="Operation"/> object as specified by format.</returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Operation"/> object to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">The format string. May contain the names of the properties to print enclosed in curly braces like '{<see cref="P:OperationNumber"/>}'.
        /// If a given property could not be found on the top-level, then it is looked after in the CustomData dictionary.
        /// If it wasn't found there either, a default string is printed.</param>
        /// <param name="formatProvider">The format provider to use for formatting.</param>
        /// <returns>A string representation of value of the current <see cref="Operation"/> object as specified by format and provider.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format, ObjectFormatterOptions.Default, formatProvider);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Operation"/> object to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">The format string. May contain the names of the properties to print enclosed in curly braces like '{<see cref="P:OperationNumber"/>}'.
        /// If a given property could not be found on the top-level, then it is looked after in the CustomData dictionary.
        /// If it wasn't found there either, a default string is printed.</param>
        /// <param name="options">Options to use for controlling formatting.</param>
        /// <param name="formatProvider">The format provider to use for formatting.</param>
        /// <returns>A string representation of value of the current <see cref="Operation"/> object as specified by format and provider.</returns>
        public string ToString(string format, ObjectFormatterOptions options, IFormatProvider formatProvider)
        {
            ExtendedObjectExpressionFormatter<Operation> formatter = new ExtendedObjectExpressionFormatter<Operation>(ResolveProperty);
            formatter.Options = options;
            return formatter.ToString(this, format);
        }

        private static ResolveExpressionResult ResolveProperty(Operation operation, string expression)
        {
            // Look inside the custom dictionary for a key named like the expression.

            ResolveExpressionResult result = ResolveExpressionResult.Fail;

            if (operation.CustomData.ContainsKey(expression))
            {
                result.ResolvedValue = operation.CustomData[expression];
                result.Success = true;
            }

            return result;
        }

        #endregion
    }
}