using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.IlsAnsbachParser
{
    [Export("IlsAnsbachParser", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterParser(new IlsAnsbachParser());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
