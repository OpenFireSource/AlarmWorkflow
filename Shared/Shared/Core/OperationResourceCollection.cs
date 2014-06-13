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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Represents a collection that holds many <see cref="OperationResource"/>s and is displayable as a string.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Count = {Items.Count}")]
    public sealed class OperationResourceCollection : Collection<OperationResource>, IFormattable
    {
        #region Constants

        /// <summary>
        /// Defines the format string that is used to format this instance as a single line.
        /// </summary>
        public static readonly string SingleLineFormatString = "{FullName}; {Timestamp}; {RequestedEquipment} | ";
        /// <summary>
        /// Defines the format string that is used to format this instance using three lines per resource.
        /// </summary>
        public static readonly string ThreeLinesFormatString = "{FullName}\n{Timestamp}\n{RequestedEquipment}\n";

        #endregion

        #region	Fields

        private string _toStringCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResourceCollection"/> class.
        /// </summary>
        public OperationResourceCollection()
            : base()
        {

        }

        #endregion

        #region	Methods

        /// <summary>
        /// Overridden to invalidate the ToString-cache so that it gets recreated to reflect the instance.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, OperationResource item)
        {
            // Sanity-check: Don't allow adding empty resources!
            if (item == null || string.IsNullOrWhiteSpace(item.FullName))
            {
                return;
            }

            base.InsertItem(index, item);
            InvalidateToStringCache();
        }

        /// <summary>
        /// Overridden to invalidate the ToString-cache so that it gets recreated to reflect the instance.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            InvalidateToStringCache();
        }

        /// <summary>
        /// Overridden to invalidate the ToString-cache so that it gets recreated to reflect the instance.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, OperationResource item)
        {
            base.SetItem(index, item);
            InvalidateToStringCache();
        }

        /// <summary>
        /// Overridden to invalidate the ToString-cache so that it gets recreated to reflect the instance.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            InvalidateToStringCache();
        }

        private void InvalidateToStringCache()
        {
            _toStringCache = null;
        }

        /// <summary>
        /// Adds a new resource with the given name.
        /// </summary>
        /// <param name="fullName">The full name of the resource.</param>
        /// <returns>The <see cref="OperationResource"/> that was added if it didn't exist.
        /// -or- the <see cref="OperationResource"/> that existed under the given name.</returns>
        public OperationResource AddResource(string fullName)
        {
            return GetResourceOrCreate(fullName);
        }

        private OperationResource GetResourceOrCreate(string fullName)
        {
            OperationResource resource = this.Items.FirstOrDefault(it => string.Equals(it.FullName, fullName));
            if (resource == null)
            {
                resource = new OperationResource();
                resource.FullName = fullName;
                this.Add(resource);
            }
            return resource;
        }

        /// <summary>
        /// Adds a new equipment entry to the given resource (creates it if it does not exist yet) and returns the updated OperationResource.
        /// </summary>
        /// <param name="fullName">The full name of the resource to add the equipment to.</param>
        /// <param name="requestedEquipmentText">The text of the equipment to add.</param>
        /// <returns>The <see cref="OperationResource"/> that was added if it didn't exist.
        /// -or- the <see cref="OperationResource"/> that existed under the given name.</returns>
        public OperationResource AddEquipment(string fullName, string requestedEquipmentText)
        {
            OperationResource resource = GetResourceOrCreate(fullName);
            resource.RequestedEquipment.Add(requestedEquipmentText);
            return resource;
        }

        /// <summary>
        /// Creates and returns a string that contains all resources and their equipment in one line.
        /// </summary>
        /// <remarks>This method caches the initially created string. To update it, you must call the other methods or add/clear/delete items.</remarks>
        /// <returns></returns>
        public override string ToString()
        {
            // If the cache needs to be built (first time or entries changed)
            if (_toStringCache == null)
            {
                _toStringCache = ToString(ToStringStyle.SingleLine);
            }
            return _toStringCache;
        }

        /// <summary>
        /// Creates and returns a string that contains all resources and their equipment in a predefined format.
        /// </summary>
        /// <param name="style">The style determining how the output will look like.</param>
        /// <returns></returns>
        public string ToString(ToStringStyle style)
        {
            switch (style)
            {
                case ToStringStyle.ThreeLinesPerResource:
                    return ToString(ThreeLinesFormatString, null);
                case ToStringStyle.SingleLine:
                default:
                    return ToString(SingleLineFormatString, null);
            }
        }

        #endregion

        #region IFormattable Members

        /// <summary>
        /// Creates and returns a string that contains all resources and their equipment in a custom format.
        /// See documentation for further information and formatting options.
        /// </summary>
        /// <remarks>The <paramref name="format"/>-parameter specifies the format to use for one resource and its equipment.
        /// </remarks>
        /// <param name="format">The custom format, which is used to format a single resource and its equpiment.</param>
        /// <param name="formatProvider">Not used.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The format-parameter was null or empty.</exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentNullException("format");
            }

            List<string> parts = new List<string>();

            // TODO: This loop is unoptimized!
            foreach (OperationResource resource in this.Items)
            {
                StringBuilder sb = new StringBuilder(format);
                // Replace common control chars
                sb.Replace("\n", Environment.NewLine);

                Regex regex = new Regex(@"{(\w+)}");
                foreach (Group match in regex.Matches(format))
                {
                    string macroText = match.Value;
                    string propertyName = macroText.Substring(1, macroText.Length - 2);

                    string propertyValue = "(No value)";
                    object rawValue = null;

                    PropertyInfo property = resource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                    if (property != null)
                    {
                        rawValue = property.GetValue(resource, null);
                        if (rawValue != null)
                        {
                            IList<string> enumerable = rawValue as IList<string>;
                            if (enumerable != null)
                            {
                                // TODO: Hardcoded separator is not good!
                                propertyValue = string.Join(", ", enumerable);
                            }
                            else
                            {
                                propertyValue = rawValue.ToString();
                            }
                        }
                    }

                    sb.Replace(macroText, propertyValue);
                }

                parts.Add(sb.ToString());
            }

            return string.Concat(parts);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies the way the ToString-representation is created.
        /// </summary>
        public enum ToStringStyle
        {
            /// <summary>
            /// Puts all resources and their requested equipment in one long line.
            /// </summary>
            SingleLine = 0,
            /// <summary>
            /// Creates three lines per resource.
            /// See documentation for further information.
            /// </summary>
            /// <remarks>
            /// 1. line contains the name of the resource.
            /// 2. line contains the timestamp
            /// 3. line contains all requested equipments of that resource, splitted by a comma.
            /// </remarks>
            ThreeLinesPerResource,
        }

        #endregion
    }
}