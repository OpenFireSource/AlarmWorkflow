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
using System.Linq;
using System.Reflection;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Provides tools that use an expression string to access properties from an object.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>An expression string is a string which "expresses" the path to a property of any given object graph.
    /// If you have a type that exposes a property "CustomTypeA" which itself exposes a property "SomeProperty", then you can
    /// express the path to "SomeProperty" as: "CustomTypeA.SomeProperty".</remarks>
    public static class ObjectExpressionTools
    {
        /// <summary>
        /// Traverses the property graph of an object and sets a certain property to a given value.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property to set. Must not be empty.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="T:MissingFieldException">A certain property in the expression was not found.</exception>
        /// <exception cref="T:ExpressionNotSupportedException">The expression was not supported.</exception>
        public static void SetValueFromExpression(object graph, string expression, object value)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(expression, "expression");

            PropertyInfo property = null;
            object target = null;
            GetPropertyFromExpression(graph, expression, true, out property, out target);

            property.SetValue(target, value, null);
        }

        /// <summary>
        /// Traverses the property graph of an object and sets a certain property to a given value.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property to set. Must not be empty.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Whether or not the value could be set. If this returns <c>false</c>, then the expression led to a nonexistent property.</returns>
        public static bool TrySetValueFromExpression(object graph, string expression, object value)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(expression, "expression");

            PropertyInfo property = null;
            object target = null;

            bool success = GetPropertyFromExpression(graph, expression, false, out property, out target);
            if (!success)
            {
                value = null;
                return false;
            }

            property.SetValue(target, value, null);
            return true;
        }

        /// <summary>
        /// Traverses the property graph of an object and returns the value of a certain property.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property to set. Must not be empty.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="T:MissingFieldException">A certain property in the expression was not found.</exception>
        /// <exception cref="T:ExpressionNotSupportedException">The expression was not supported.</exception>
        public static object GetValueFromExpression(object graph, string expression)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(expression, "expression");

            PropertyInfo property = null;
            object target = null;
            GetPropertyFromExpression(graph, expression, true, out property, out target);

            return property.GetValue(target, null);
        }

        /// <summary>
        /// Traverses the property graph of an object and returns the value of a certain property.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property to set. Must not be empty.</param>
        /// <param name="value">If the return value was <c>true</c>, then this contains the value of the property.</param>
        /// <returns>Whether or not the value could be retrieved. If this returns <c>false</c>, then the expression led to a nonexistent property.</returns>
        public static bool TryGetValueFromExpression(object graph, string expression, out object value)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(expression, "expression");

            PropertyInfo property = null;
            object target = null;

            bool success = GetPropertyFromExpression(graph, expression, false, out property, out target);
            if (!success)
            {
                value = null;
                return false;
            }

            value = property.GetValue(target, null);
            return true;
        }

        /// <summary>
        /// Traverses the property graph of an object and returns the property at the end of the expression.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property. Must not be empty.</param>
        /// <param name="throwOnError">Whether or not to throw an exception if any property in the expression did not exist.</param>
        /// <param name="property">If the return value is <c>true</c>, this parameter contains the property that was found.</param>
        /// <param name="target">If the return value is <c>true</c>, this parameter contains the instance of the object on which the property was found.</param>
        /// <returns>Whether or not the property could be retrieved. Returns false also in case of an error (if <paramref name="throwOnError"/> was set to false).</returns>
        /// <exception cref="T:MissingFieldException">A certain property in the expression was not found.</exception>
        /// <exception cref="T:ExpressionNotSupportedException">The expression was not supported.</exception>
        public static bool GetPropertyFromExpression(object graph, string expression, bool throwOnError, out PropertyInfo property, out object target)
        {
            string[] tokens = expression.Split(new[] { '.' });

            target = graph;
            property = null;
            for (int i = 0; i < tokens.Length; i++)
            {
                string propertyName = tokens[i];

                property = target.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    if (throwOnError)
                    {
                        string message = string.Format("Property with name '{0}' was not found in object type '{1}' (expression was '{2}').", propertyName, target.GetType().Name, expression);
                        throw new MissingFieldException(target.GetType().Name, propertyName);
                    }
                    return false;
                }
                else if (!IsPropertyGettable(property))
                {
                    if (throwOnError)
                    {
                        throw new ExpressionNotSupportedException(expression);
                    }
                    return false;
                }

                if (i < tokens.Length - 1)
                {
                    // Next iteration... step down one hierarchy level
                    target = property.GetValue(target, null);
                }
            }

            return true;
        }

        private static bool IsPropertyGettable(PropertyInfo property)
        {
            if (!property.CanRead)
            {
                return false;
            }
            if (property.GetIndexParameters().Length > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns an array containing the names of all properties in the specified type, including children, with no disallowed property names and not requiring CanWrite.
        /// </summary>
        /// <param name="type">The type to get all properties from. Must not be null.</param>
        /// <returns>An array containing the names of all properties in the specified type, including children.</returns>
        public static string[] GetPropertyNames(Type type)
        {
            string[] disallowedPropertyNames = new string[0];
            return GetPropertyNames(type, disallowedPropertyNames);
        }

        /// <summary>
        /// Returns an array containing the names of all properties in the specified type, including children, and not requiring CanWrite.
        /// </summary>
        /// <param name="type">The type to get all properties from. Must not be null.</param>
        /// <param name="disallowedPropertyNames">An array containing the names of the properties that shall be ignored in the result.</param>
        /// <returns>An array containing the names of all properties in the specified type, including children.</returns>
        public static string[] GetPropertyNames(Type type, string[] disallowedPropertyNames)
        {
            bool requireCanWrite = false;
            return GetPropertyNames(type, disallowedPropertyNames, requireCanWrite);
        }

        /// <summary>
        /// Returns an array containing the names of all properties in the specified type, including children.
        /// </summary>
        /// <param name="type">The type to get all properties from. Must not be null.</param>
        /// <param name="disallowedPropertyNames">An array containing the names of the properties that shall be ignored in the result.</param>
        /// <param name="requireCanWrite">Whether or not only writeable properties are returned.</param>
        /// <returns>An array containing the names of all properties in the specified type, including children.</returns>
        public static string[] GetPropertyNames(Type type, string[] disallowedPropertyNames, bool requireCanWrite)
        {
            Assertions.AssertNotNull(type, "type");
            if (disallowedPropertyNames == null)
            {
                disallowedPropertyNames = new string[0];
            }

            List<string> propertiesTemp = new List<string>();
            FillAllowedProperties(type, "", disallowedPropertyNames, requireCanWrite, propertiesTemp);
            propertiesTemp.Sort();

            return propertiesTemp.ToArray();
        }

        private static void FillAllowedProperties(Type child, string hierarchySoFar, string[] disallowedPropertyNames, bool requireCanWrite, IList<string> propertiesBucket)
        {
            foreach (PropertyInfo property in child.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (disallowedPropertyNames.Contains(property.Name))
                {
                    continue;
                }
                if (requireCanWrite && !property.CanWrite)
                {
                    continue;
                }

                Type propertyType = property.PropertyType;
                if (IsTypeExpandable(propertyType))
                {
                    FillAllowedProperties(propertyType, hierarchySoFar + property.Name + ".", disallowedPropertyNames, requireCanWrite, propertiesBucket);
                }
                else
                {
                    string name = property.Name;
                    if (property.DeclaringType != null)
                    {
                        name = hierarchySoFar + name;
                    }

                    propertiesBucket.Add(name);
                }
            }
        }

        private static bool IsTypeExpandable(Type propertyType)
        {
            bool isInSystemNamespace = propertyType.Namespace.StartsWith("System");
            bool isAnEnumeration = propertyType.GetInterface(typeof(System.Collections.IEnumerable).Name) != null;
            return !isInSystemNamespace && !isAnEnumeration;
        }
    }
}