using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
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
            this.mailContent = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailContentTemplate.html"));
        }

        public void send()
        {
            //設定信件內容
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(this.mailaddress),
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            mail.To.Add(ReceiveAddress);
            mail.Subject = "升大學職涯型落點分析-免費密勝秘笈";
            //mail.Attachments.Add(new Attachment(@"D:\如何成為推甄中的亮點-中華大學俞征武教授.pdf"));
            var view = AlternateView.CreateAlternateViewFromString(this.mailContent,Encoding.UTF8,"text/html");
            mail.AlternateViews.Add(view);
            mail.IsBodyHtml = true;

            //設定SMTP
            SmtpClient smtp = new SmtpClient()
            {
                //UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this.mailaddress,this.mailPwd),
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
            smtp.Send(mail);
        }
    }
}
