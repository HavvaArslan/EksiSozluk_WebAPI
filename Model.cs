using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eski_WebAPI.Models
{
    public class Model
    {
        public string entry { get; set; }
        public DateTime entry_date { get; set; }
        public string dates { get; set; }
        public bool success { get; set; }
    }
}