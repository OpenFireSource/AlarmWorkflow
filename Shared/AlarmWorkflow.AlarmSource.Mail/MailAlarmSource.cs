using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using S22.Imap;

namespace AlarmWorkflow.AlarmSource.Mail
{
    [Export("MailAlarmSource", typeof(IAlarmSource))]
    internal class MailAlarmSource : IAlarmSource
    {
        #region Fields

        private readonly MailConfiguration _configuration;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MailAlarmSource" /> class.
        /// </summary>
        public MailAlarmSource()
        {
            _configuration = new MailConfiguration();
        }

        #endregion Constructors

        #region IAlarmSource Members

        /// <summary>
        ///     Raised when a new alarm has surfaced and processed for the Engine to handle.
        ///     See documentation for further information.
        /// </summary>
        public event EventHandler<AlarmSourceEventArgs> NewAlarm;

        void IAlarmSource.Initialize()
        {
        }

        void IAlarmSource.RunThread()
        {
            switch (_configuration.POPIMAP.ToLower())
            {
                case "imap":
                    using (
                        _ImapClient =
                        new ImapClient(_configuration.ServerName, _configuration.Port, _configuration.UserName,
                                       _configuration.Password, AuthMethod.Login, _configuration.SSL))
                    {
                        if (_ImapClient.Supports("IDLE"))
                        {
                            _ImapClient.NewMessage += ImapClientNewMessage;
                        }
                        else
                        {
                            Logger.Instance.LogFormat(LogType.Info, this,
                                                      "IMAP IDLE wird vom Server nicht unterstützt!!!");
                        }
                        while (true)
                        {
                            CheckImapMail(_ImapClient);
                            Thread.Sleep(1000);
                        }
                    }
            }
        }

        private void OnNewAlarm(Operation operation)
        {
            EventHandler<AlarmSourceEventArgs> copy = NewAlarm;
            if (copy != null)
            {
                copy(this, new AlarmSourceEventArgs(operation));
            }
        }

        private void ImapClientNewMessage(object sender, IdleMessageEventArgs e)
        {
        }

        #endregion IAlarmSource Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion IDisposable Members

        #region Methods

        private void CheckImapMail(ImapClient client)
        {
            const int maxtrys = 10;
            for (int i = 0; i < maxtrys; i++)
            {
                try
                {
                    uint[] uids = client.Search(SearchCondition.Unseen());
                    foreach (MailMessage msg in uids.Select(uid => client.GetMessage(uid)))
                    {
                        Logger.Instance.LogFormat(LogType.Debug, this, "NEUE MAIL " + msg.Subject);
                        MailOperation(msg);
                    }
                    break;
                }
                catch (NotAuthenticatedException)
                {
                    client.Login(_configuration.UserName, _configuration.Password, AuthMethod.Login);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Error, this, ex.ToString());
                }
            }
        }

        private void MailOperation(MailMessage message)
        {
            if (message.Subject.ToLower().Contains(_configuration.MailSubject.ToLower()) &&
                message.From.Address.ToLower() == _configuration.MailSender.ToLower())
            {
                message.Body = message.Body.Replace("----------------------------------------", String.Empty);
                string[] lines = message.Body.Split(Environment.NewLine.ToCharArray(),
                                                    StringSplitOptions.RemoveEmptyEntries);
                IList<string> fields = new List<string>
                                           {
                                               "Ort",
                                               "Ortsteil",
                                               "Straße",
                                               "Hausnummer",
                                               "Koordinaten X/Y (GK)",
                                               "Zusatzinfos zum Objekt",
                                               "Betroffene",
                                               "Einsatzart",
                                               "Stichwort",
                                               "Sondersignal",
                                               "Zusatzinformationen",
                                               "Alarmierungen",
                                               "Meldende(r)",
                                               "Telefon"
                                           };
                IDictionary<string, string> result = Analyse.AnalyseData(lines, fields, ":", Environment.NewLine);
                var op = new Operation();
                op.OperationNumber = op.Id.ToString();
                foreach (var pair in result)
                {
                    string value = pair.Value.Replace(Environment.NewLine, ";");
                    switch (pair.Key)
                    {
                        case "Ort":
                            op.City = value;
                            break;

                        case "Ortsteil":
                            op.City += " " + value;
                            break;

                        case "Straße":
                            op.Street = value;
                            break;

                        case "Hausnummer":
                            op.StreetNumber = value;
                            break;

                        case "Koordinaten X/Y (GK)":
                            op.Comment += value;
                            break;

                        case "Zusatzinfos zum Objekt":
                            op.Comment = value;
                            break;

                        case "Einsatzart":
                            op.EmergencyKeyword = value;
                            break;

                        case "Stichwort":
                            op.Keyword = value;
                            break;

                        case "Sondersignal":
                            op.Comment += value;
                            break;

                        case "Zusatzinformationen":
                            op.Picture = value;
                            break;

                        case "Alarmierungen":
                            op.Resources.AddResource(value);
                            break;

                        case "Meldende(r)":
                            op.Messenger = value;
                            break;

                        case "Telefon":
                            break;
                    }
                }
                OnNewAlarm(op);
            }
        }

        #endregion Methods

        private ImapClient _ImapClient { get; set; }
    }
}