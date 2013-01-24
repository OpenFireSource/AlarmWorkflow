using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    class OperationPropertyHelper
    {
        #region Constants

        private static readonly string[] DisallowedProperties = new[] { "CustomData", "IsAcknowledged", "Id", "Resources", "RouteImage" };
        private static readonly string[] OperationProperties;

        static OperationPropertyHelper()
        {
            OperationProperties = ObjectExpressionTools.GetPropertyNames(typeof(Operation), DisallowedProperties, true);
        }

        public static IEnumerable<string> PropertyNames
        {
            get { return OperationProperties; }
        }

        #endregion

    }
}
