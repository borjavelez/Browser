using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrowserAPI.Models
{
    public class Doc
    {
        public int Id { get; set; }
        [Required]
        public String Url { get; set; }
    }
}