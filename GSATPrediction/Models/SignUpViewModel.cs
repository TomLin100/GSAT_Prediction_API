using PredictionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GSATPrediction.Models
{
    public class SignUpViewModel
    {
        // 輸入的JSON字串
        //{
        //    "email": input_email,
        //    "location": select_location,
        //    "schoolName": select_school,
        //    "identity": select_identity
        //}

        public string email { get; set; }
        public string lineID { get; set; }
        public string location { get; set; }
        public string schoolName { get; set; }
        public string identity { get; set; }
        public Input user_input { get; set; }
}
}
