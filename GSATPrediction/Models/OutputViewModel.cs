using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GSATPrediction.Models
{
    public class OutputViewModel
    {
        public HttpStatusCode status { get; set; }
        public SignUpViewModel input { get; set; }

        public string message { get; set; }
    }
}
