using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrowserAPI.Models
{
    public class Term
    {
        public int Id { get; set; }
        [Required]
        public string Value { get; set; }
    }
}