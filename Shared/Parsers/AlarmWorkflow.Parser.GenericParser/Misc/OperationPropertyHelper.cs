using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    class OperationPropertyHelper
    {
        #region Constants

        private static readonly string[] DisallowedProperties = new[] { "CustomData", "IsAcknowledged", "Id", "Resources", "RouteImage" };
        private static readonly string[] OperationProperties;

        public static IEnumerable<string> PropertyNames
        {
            get { return OperationProperties; }
        }

        static OperationPropertyHelper()
        {
            List<string> propertiesTemp = new List<string>();
            FillAllowedProperties(typeof(Operation), "", propertiesTemp);
            propertiesTemp.Sort();

            OperationProperties = propertiesTemp.ToArray();
        }

        private static void FillAllowedProperties(Type child, string hierarchySoFar, IList<string> propertiesBucket)
        {
            foreach (PropertyInfo property in child.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (DisallowedProperties.Contains(property.Name))
                {
                    continue;
                }
                if (!property.CanWrite)
                {
                    continue;
                }

                // If the property may be extensible (just assume that blindly if it is not in the System-namespace)
                Type propertyType = property.PropertyType;
                if (!propertyType.Namespace.StartsWith("System"))
                {
                    FillAllowedProperties(propertyType, hierarchySoFar + property.Name + ".", propertiesBucket);
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

        #endregion

    }
}
