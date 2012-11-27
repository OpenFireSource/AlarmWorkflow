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
        private ImapClient _imapClient;

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
        }

        void IAlarmSource.RunThread()
        {
            switch (_configuration.POPIMAP.ToLower())
            {

                case "imap":
                    using (_imapClient = new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.UserName, _configuration.Password, S22.Imap.AuthMethod.Login, _configuration.SSL))
                    {
                        _imapClient.NewMessage += new EventHandler<IdleMessageEventArgs>(_imapClient_NewMessage);
                        while (true)
                        {
                            checkMail_imap(_imapClient);
                            Thread.Sleep(_configuration.PollInterval);
                        }
                    }
                    break;



                case "pop":
                    using (Pop3Client _Pop3Client = new Pop3Client(_configuration.ServerName, _configuration.Port, _configuration.UserName, _configuration.Password, S22.Pop3.AuthMethod.Login, _configuration.SSL))
                    {
                        while (true)
                        {
                            checkMail_pop(_Pop3Client);
                            Thread.Sleep(_configuration.PollInterval);
                        }
                    }

                    break;

            }            
        }

        void _imapClient_NewMessage(object sender, IdleMessageEventArgs e)
        {
            checkMail_imap(_imapClient);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Methods

        private void checkMail_imap(ImapClient client)
        {
            int trys = 0;
        retry:
            try
            {
                uint[] uids = client.Search(S22.Imap.SearchCondition.Unseen());
                foreach (uint uid in uids)
                {
                    var msg = client.GetMessage(uid);

                    //
                    // HERE GO's WHAT TODO WITH THE NEW, UNREAD E-MAIL 
                    //
                }
            }
            catch (S22.Imap.NotAuthenticatedException ex)
            {
                if (trys <= 3)
                {
                    client.Login(_configuration.UserName, _configuration.Password, S22.Imap.AuthMethod.Login);
                    trys++;
                goto retry;
                }                
            }

        }

        private void checkMail_pop(Pop3Client client)
        {
            int trys = 0;
        retry:
            try
            {
                System.Net.Mail.MailMessage[] msgs = client.GetMessages();
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
                    client.Login(_configuration.UserName, _configuration.Password, S22.Pop3.AuthMethod.Login);
                    trys++;
                    goto retry;
                }
            }
            
        }

        #endregion
    }
}
