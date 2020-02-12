using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace GSATPrediction.Services
{
    public class EmailService:ISend
    {
        private string mailaddress;
        private string mailPwd;
        private string mailContent;
        private string receiveAddress;

        public string ReceiveAddress { get; set; }

        public EmailService()
        {
            this.mailaddress = ConfigurationManager.AppSettings["mailaddress"];
            this.mailPwd = ConfigurationManager.AppSettings["mailPwd"];
            this.mailContent = File.ReadAllText("EmailContentTemplate.html");
        }

        public void send()
        {
            //設定信件內容
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(@"chu.predict@gmail.com"),
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            mail.To.Add(ReceiveAddress);
            mail.Attachments.Add(new Attachment(@"D:\test2.docx"));
            AlternateView view = AlternateView.CreateAlternateViewFromString(this.mailContent,Encoding.UTF8,"text/html");
            mail.AlternateViews.Add(view);
            //設定SMTP
            SmtpClient smtp = new SmtpClient()
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(@"chu.predict@gmail.com", "e215@dbLab"),
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
            smtp.Send(mail);
        }
    }
}
