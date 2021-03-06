using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GSATPrediction.ViewModels
{
    public class OutputViewModel
    {
        public int status { get; set; }
        public SignUpViewModel input { get; set; }
        public ValidateInputViewModel validate { get; set; }

        public string message { get; set; }
    }
}
