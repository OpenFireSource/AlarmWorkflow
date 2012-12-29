using System;
using System.Reflection;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    static class Helpers
    {
        internal static bool SetValueFromExpression(object graph, string expression, string value)
        {
            object target = null;
            PropertyInfo property = null;
            if (ObjectExpressionTools.GetPropertyFromExpression(graph, expression, false, out property, out target))
            {
                return ObjectExpressionTools.TrySetValueFromExpression(graph, expression, StringToObjectSimple(property.PropertyType, value));
            }
            return false;
        }

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
    }
}
