using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Autorentool_RMT.ViewModels
{
    public class ContactViewModel : ViewModel
    {

        private string sendButtonColour = "LightGray";
        private bool isSendButtonEnabled = false;
        private string firstname = "";
        private string lastname = "";
        private string sender = "";
        private string subject = "";
        private string message = "";

        #region Firstname
        public string Firstname
        {
            get => firstname;
            set
            {
                firstname= value;
                SetIsSendButtonEnabled();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Lastname
        public string Lastname
        {
            get => lastname;
            set
            {
                lastname= value;
                SetIsSendButtonEnabled();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Sender
        public string Sender
        {
            get => sender;
            set
            {
                sender= value;
                SetIsSendButtonEnabled();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Subject
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                SetIsSendButtonEnabled();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Message
        public string Message
        {
            get => message;
            set
            {
                message = value;
                SetIsSendButtonEnabled();
                OnPropertyChanged();
            }
        }
        #endregion

        #region SendButtonColour
        public string SendButtonColour
        {
            get => sendButtonColour;
            set
            {
                sendButtonColour = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region IsSendButtonEnabled
        /// <summary>
        /// If the isSendButtonEnabled-property is changed, the SendButtonColour has to be changed aswell.
        /// </summary>
        public bool IsSendButtonEnabled
        {
            get => isSendButtonEnabled;
            set
            {
                isSendButtonEnabled = value;
                SendButtonColour = GetBackgroundColour(isSendButtonEnabled, "#1E88E5");
                OnPropertyChanged();
            }
        }
        #endregion

        #region SetIsSendButtonEnabled
        /// <summary>
        /// Sets the isSendButtonEnabled property depending on the length of the firstname, lastname, sender, subject and message.
        /// </summary>
        private void SetIsSendButtonEnabled()
        {
            bool value = firstname.Length > 0 && lastname.Length > 0 && sender.Length > 0 && subject.Length > 0 && message.Length > 0;

            IsSendButtonEnabled = value;
        }
        #endregion

        #region OnSendMail
        /// <summary>
        /// Tries to send the email and returns the message status.
        /// For testing a gmail server is used (ToDo).
        /// If an error occured an exception will be thrown. 
        /// If the email isn't correctly formed an exception with a custom message will be thrown.
        /// Else an default exception will be thrown.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> OnSendMail()
        {
            try
            {
                if (IsValidEmail(sender))
                {
                    SmtpSender smtpSender = new SmtpSender(() => new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("herbertpeter.meyer@gmail.com", "euwwuwbdxrbhevsm")
                    });

                    Email.DefaultSender = smtpSender;

                    SendResponse response = await Email.From(sender)
                        .To("florian.pfuetzenreuter@hs-furtwangen.de")
                        .Subject(subject)
                        .Body(message)
                        .SendAsync();

                    return response.Successful;

                } else
                {
                    throw new Exception("Kein korrektes E-Mail-Format");
                }
            } catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region IsValidEmail
        /// <summary>
        /// Checks if the given email is valid.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(
                        email, @"(@)(.+)$", DomainMapper,
                        RegexOptions.None, TimeSpan.FromMilliseconds(200)
                    );

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    IdnMapping idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        #endregion

    }
}
