using System;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

using System.Threading;
using S22.Imap;
using S22.Pop3;

namespace AlarmWorkflow.AlarmSource.Mail
{
    [Export("MailAlarmSource", typeof(IAlarmSource))]
    class MailAlarmSource : IAlarmSource
    {
        #region Fields

        private MailConfiguration _configuration;
        private ImapClient _ImapClient;
        private Pop3Client _Pop3Client;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailAlarmSource"/> class.
        /// </summary>
        public MailAlarmSource()
        {
            _configuration = new MailConfiguration();
            _ImapClient.NewMessage += new EventHandler<IdleMessageEventArgs>(_ImapClient_NewMessage);
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
                    _Pop3Client = new Pop3Client(_configuration.ServerName,143, _configuration.UserName, _configuration.Password,S22.Pop3.AuthMethod.Login,_configuration.SSL);
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
                        lastMail_pop();
                        break;
                }

                Thread.Sleep(_configuration.PollInterval);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _ImapClient.Dispose();
            _Pop3Client.Dispose();
        }

        #endregion

        #region Methods

        void _ImapClient_NewMessage(object sender, IdleMessageEventArgs e)
        {
            lastMail_imap();
        }

        private void lastMail_imap()
        {
            int trys = 0;
        retry:
            try
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
            catch (S22.Imap.NotAuthenticatedException ex)
            {
                if (trys <= 3)
                {
                    _ImapClient.Login(_configuration.UserName, _configuration.Password, S22.Imap.AuthMethod.Login);
                    trys++;
                goto retry;
                }                
            }

        }

        private void lastMail_pop()
        {
            int trys = 0;
        retry:
            try
            {
                System.Net.Mail.MailMessage[] msgs = _Pop3Client.GetMessages();
                foreach (System.Net.Mail.MailMessage msg in msgs)
                {


                    //
                    // HERE GO's WHAT TODO WITH THE NEW, UNREAD E-MAIL 
                    //
                }
            }
            catch (S22.Pop3.NotAuthenticatedException ex)
            {
                if (trys <= 3)
                {
                    _Pop3Client.Login(_configuration.UserName, _configuration.Password, S22.Pop3.AuthMethod.Login);
                    trys++;
                    goto retry;
                }
            }
            
        }

        #endregion
    }
}
