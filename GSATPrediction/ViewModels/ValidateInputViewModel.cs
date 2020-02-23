using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GSATPrediction.ViewModels
{
    public class ValidateInputViewModel
    {
        [Required]
        public string phone { get; set; }
        public string code { get; set; }
    }
}
