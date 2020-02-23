using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using GSATPrediction.Models;

namespace GSATPrediction.Services
{
    public class SmsService
    {
        private string apiURL;
        private string id;
        private string pwd;
        private string msgContent;

        private PredictEntities db = new PredictEntities();

        public string PhoneNumber { get; set; }

        public SmsService()
        {
            apiURL = "http://api.message.net.tw/send.php";
            id = ConfigurationManager.AppSettings["smsid"];
            pwd = ConfigurationManager.AppSettings["smspwd"];
        }
        public string send()
        {
            msgContent = generateContent();

            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("id", id);
            postParams.Add("password", pwd);
            postParams.Add("tel", PhoneNumber);
            postParams.Add("msg", msgContent);
            postParams.Add("mtype", "G");
            postParams.Add("encoding", "utf8");
            

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postParams.ToString().Length;

            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }

            //API回傳的字串
            string responseStr = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                }
            }

            return responseStr;
        }

        private string generateContent()
        {
            int[] num = new int[6];
            Random random = new Random();
            for(int i=0;i<num.Length;i++)
            {
                num[i] = random.Next(1, 9);
            }

            string code = "";
            foreach(int tmp in num)
            {
                code = code + tmp.ToString();
            }
            var data = new Validation()
            {
                phone = PhoneNumber,
                code = code
            };
            db.Validations.Add(data);
            db.SaveChanges();
            
            return $"[職涯型落點分析系統] 您的驗證碼為 {code}，請盡速到系統中輸入您的驗證碼";
        }

        public Dictionary<string, int> responseFormat(string response)
        {
            string[] data = response.Split('\n');
            string[] coda = data[1].Split('=');
            string[] error = data[0].Split('=');
            Dictionary<string, int> pairs = new Dictionary<string, int>();
            if (coda[0].Equals("LCount"))
            {
                pairs.Add("smsNum", Convert.ToInt32(coda[1]));
            }
            if (error[0].Equals("ErrorCode") && Convert.ToInt32(error[1]) == 0)
            {
                pairs.Add("smsStatus", Convert.ToInt32(error[1]));
            }
            else
            {
                pairs.Add("smsStatus", -1);
            }
            return pairs;
        }
    }
}
