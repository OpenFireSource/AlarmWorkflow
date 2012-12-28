using System;
using System.Reflection;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    static class Helpers
    {
        private static object StringToObjectSimple(Type targetType, string value)
        {
            object realValue = null;
            switch (targetType.FullName)
            {
                case "System.String": realValue = value; break;
                case "System.Int16": realValue = Int16.Parse(value); break;
                case "System.Int32": realValue = Int32.Parse(value); break;
                case "System.Int64": realValue = Int64.Parse(value); break;
                case "System.UInt16": realValue = UInt16.Parse(value); break;
                case "System.UInt32": realValue = UInt32.Parse(value); break;
                case "System.UInt64": realValue = UInt64.Parse(value); break;
                case "System.Single": realValue = Single.Parse(value); break;
                case "System.Double": realValue = Double.Parse(value); break;
                default:
                    break;
            }
            return realValue;
        }

        internal static bool SetValueFromExpression(object graph, string expression, string value)
        {
            Assertions.AssertNotNull(graph, "graph");
            Assertions.AssertNotEmpty(expression, "expression");

            string[] tokens = expression.Split(new[] { '.' });

            // Find property
            object currentObject = graph;
            PropertyInfo property = null;
            for (int i = 0; i < tokens.Length; i++)
            {
                string propertyName = tokens[i];

                property = currentObject.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    Logger.Instance.LogFormat(LogType.Warning, typeof(GenericParser), "Property with name '{0}' was not found in object type '{1}' (expression was '{2}').", propertyName, currentObject.GetType().Name, expression);
                    break;
                }

                if (i < tokens.Length - 1)
                {
                    // Next iteration... step down one hierarchy level
                    currentObject = property.GetValue(currentObject, null);
                }
            }

            if (property != null)
            {
                property.SetValue(currentObject, StringToObjectSimple(property.PropertyType, value), null);
                return true;
            }

            // Property not found
            return false;
        }
    }
}
