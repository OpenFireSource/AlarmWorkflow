
namespace AlarmWorkflow.Tools.AutoUpdater
{
    static class Log
    {
        internal delegate void PostTextDelegate(string text);

        internal static event PostTextDelegate PostText;

        internal static void Write(string format, params object[] args)
        {
            var copy = PostText;
            if (copy != null)
            {
                copy(string.Format(format, args));
            }
        }

    }
}
