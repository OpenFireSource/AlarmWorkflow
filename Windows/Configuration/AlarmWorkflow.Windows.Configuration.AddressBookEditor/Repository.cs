using System;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor
{
    class Repository
    {
        #region Singleton

        private static Repository _instance;

        internal static Repository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Repository();
                }
                return _instance;
            }
        }

        #endregion
    }
}
