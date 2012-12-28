using System;
using System.Reflection;

namespace AlarmWorkflow.Shared.Core
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
        /// <exception cref="System.MissingFieldException">A certain property in the expression was not found.</exception>
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
        /// Traverses the property graph of an object and returns the value of a certain property.
        /// </summary>
        /// <param name="graph">The object graph to use. Must not be null.</param>
        /// <param name="expression">The expression of the property to set. Must not be empty.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="System.MissingFieldException">A certain property in the expression was not found.</exception>
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
        /// <exception cref="System.MissingFieldException">A certain property in the expression was not found.</exception>
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

        private static bool GetPropertyFromExpression(object graph, string expression, bool throwOnMissing, out PropertyInfo property, out object target)
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
                    if (throwOnMissing)
                    {
                        string message = string.Format("Property with name '{0}' was not found in object type '{1}' (expression was '{2}').", propertyName, target.GetType().Name, expression);
                        throw new MissingFieldException(target.GetType().Name, propertyName);
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

    }
}
