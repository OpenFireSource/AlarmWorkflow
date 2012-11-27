using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

using System.Threading;
using S22.Imap;

namespace AlarmWorkflow.AlarmSource.Mail
{
    [Export("MailAlarmSource", typeof(IAlarmSource))]
    class MailAlarmSource : IAlarmSource
    {
        #region Fields

        private MailConfiguration _configuration;
        private ImapClient _ImapClient;
        //private Pop3Client _Pop3Client;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailAlarmSource"/> class.
        /// </summary>
        public MailAlarmSource()
        {
            _configuration = new MailConfiguration();
        }

       
        #endregion

        #region IAlarmSource Members

        /// <summary>
        /// Raised when a new alarm has surfaced and processed for the Engine to handle.
        /// See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        private void OnNewAlarm(Operation operation)
        {
            var copy = NewAlarm;
            if (copy != null)
            {
                copy(this, new AlarmSourceEventArgs(operation));
            }
        }
        
        void IAlarmSource.Initialize()
        {
            switch (_configuration.POPIMAP.ToLower())
            {
                case "pop":
                    //_Pop3Client = new Pop3Client(_configuration.ServerName, _configuration.UserName, _configuration.Password, _configuration.Port, _configuration.SSL, false);
                    break;

                case "imap":
                    _ImapClient = new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.UserName, _configuration.Password, S22.Imap.AuthMethod.Login, _configuration.SSL);
                    break;
            }
        }

        void IAlarmSource.RunThread()
        {
            while (true)
            {
                switch (_configuration.POPIMAP.ToLower())
                {
                    case "imap":
                        lastMail_imap();
                        break;

                    case "pop":
                        break;
                }

                Thread.Sleep(_configuration.PollInterval);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO: Clean up this instance
        }

        #endregion

        #region Methods

       
        private void lastMail_imap()
        {
            uint[] uids = _ImapClient.Search(S22.Imap.SearchCondition.Unseen());
            foreach (uint uid in uids)
            {
                var msg = _ImapClient.GetMessage(uid);
                
                //
                // HERE GO's WHAT TODO WITH THE NEW, UNREAD E-MAIL 
                //
            }
        }

        private void lastMail_pop()
        {
            //var msg = _Pop3Client.GetMessage(MessageID);

        }

        #endregion
    }
}
