using System;

namespace AlarmWorkflow.Windows.UI.Extensibility
{
    /// <summary>
    /// Configures a type that implements <see cref="T:IOperationViewer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OperationViewerAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the type of the parser that is represented by this Operation viewer.
        /// </summary>
        public Type ParserType { get; private set; }

        #endregion

        #region Constructors

        private OperationViewerAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parserType">The type of the parser that is represented by this Operation viewer.</param>
        public OperationViewerAttribute(Type parserType)
            : this()
        {
            ParserType = parserType;
        }

        #endregion
    }
}
