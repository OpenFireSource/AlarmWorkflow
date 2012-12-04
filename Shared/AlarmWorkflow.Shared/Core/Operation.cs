using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
        /// Gets/sets the date and time when this operation was created.
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
        /// Gets or sets the Ort object.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Gets/sets the street. The street may contain the StreetNumber.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Gets/sets the street number. This may be empty and/or the street number may be merged into the Street-property.
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Gets or sets the Ort object.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Gets/sets the zip code of the City.
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Gets or sets the Objekt object.
        /// </summary>
        public string Property { get; set; }
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
        /// Gets the Einsatzstichwort object.
        /// </summary>
        public string EmergencyKeyword { get; set; }
        /// <summary>
        /// Gets the Stichwort object.
        /// </summary> 
        public string Keyword { get; set; }
        /// <summary>
        /// Gets/sets the list of all resources requested by the call center.
        /// </summary>
        public OperationResourceCollection Resources { get; set; }
        /// <summary>
        /// Gets/sets the custom data for this operation.
        /// </summary>
        public IDictionary<string, object> CustomData { get; set; }
        /// <summary>
        /// Gets/sets the image data that contains the route plan.
        /// </summary>
        public byte[] RouteImage { get; set; }
        /// <summary>
        /// Gets/sets whether or not this operation is acknowledged, that means that this operation is no longer necessary to be displayed in the UI as "fresh".
        /// If this is set to "false" then this operation will always been shown in the UI. By default, an operation is set to "acknowledged"
        /// either if the user manually acknowledges it or after a defined timespan (usually 8 hours).
        /// </summary>
        public bool IsAcknowledged { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        public Operation()
        {
            CustomData = new Dictionary<string, object>();
            Resources = new OperationResourceCollection();
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
            return new PropertyLocation()
            {
                ZipCode = this.ZipCode,
                City = this.City,
                Street = this.Street,
                StreetNumber = this.StreetNumber,
            };
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
            if (string.IsNullOrWhiteSpace(format))
            {
                return format;
            }

            StringBuilder sb = new StringBuilder(format);
            // Replace common control chars
            sb.Replace("\n", Environment.NewLine);

            Regex regex = new Regex(@"{(\w+)}");
            foreach (Group match in regex.Matches(format))
            {
                string macroText = match.Value;
                string propertyName = macroText.Substring(1, macroText.Length - 2);

                string propertyValue = "[?]";
                object rawValue = null;

                PropertyInfo property = this.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (property != null)
                {
                    rawValue = property.GetValue(this, null);
                }
                else
                {
                    rawValue = null;
                    if (CustomData.ContainsKey(propertyName))
                    {
                        rawValue = CustomData[propertyName];
                    }
                }

                if (rawValue != null)
                {
                    propertyValue = rawValue.ToString();
                }

                sb.Replace(macroText, propertyValue);
            }

            return sb.ToString();
        }

        #endregion
    }
}