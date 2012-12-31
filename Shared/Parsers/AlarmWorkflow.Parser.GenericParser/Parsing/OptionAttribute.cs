using System;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Declares a property as an option to be specified/customized by a user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        #region Properties

        public string DisplayName { get; private set; }
        public string Description { get; set; }
        public string Category { get; set; }

        #endregion

        #region Constructors

        public OptionAttribute(string displayName)
        {
            Assertions.AssertNotEmpty(displayName, "displayName");

            this.DisplayName = displayName;
        }

        #endregion
    }
}
