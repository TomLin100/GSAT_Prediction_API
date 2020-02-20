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
        //  "email": input_email,
        //  "phoneNumber": input_phone,
        //  "address": input_address,
        //  "location": select_location,
        //  "schoolName": select_school,
        //  "identity": select_identity,
        //  "interestedDepart": input_interest,
        //  "isApplyCHU": input_isApplyCHU,
        //  "user_input": data
        // }

        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
        public string interestedDepart { get; set; }
        public bool isApplyCHU { get; set; }
        public string location { get; set; }
        public string schoolName { get; set; }
        public string identity { get; set; }
        public Input user_input { get; set; }
}
}
