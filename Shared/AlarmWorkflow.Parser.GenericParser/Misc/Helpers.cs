using System;
using System.Reflection;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    static class Helpers
    {
        /// <summary>
        /// Sets the property value and casts it correctly from a string.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        internal static void SetValue(this PropertyInfo property, Operation operation, string value)
        {
            object realValue = null;
            switch (property.PropertyType.FullName)
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

            property.SetValue(operation, realValue, null);
        }
    }
}
