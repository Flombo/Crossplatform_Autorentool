using System.Text;
using System;
using System.Reflection;

namespace Autorentool_RMT.Models
{
    public class Mail : Model
    {

        #region Mail attributes
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string Message { get; set; }
        #endregion

        #region Mail constructor
        /// <summary>
        /// Mail constructor
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="emailAddress"></param>
        /// <param name="emailSubject"></param>
        /// <param name="message"></param>
        public Mail(string firstname, string lastname, string emailAddress, string emailSubject, string message)
        {
            Firstname = firstname;
            Lastname = lastname;
            EmailAddress = emailAddress;
            EmailSubject = emailSubject;
            Message = message;
        }
        #endregion

        #region SenderOneLineSummary
        public string SenderOneLineSummary => $"{Firstname} {Lastname}";
        #endregion

    }
}
