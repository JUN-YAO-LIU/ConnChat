using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Login.Models
{
    public class Rooms
    {
        [Key]
        public string ID { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string Message { get; set; }
        public string IsActivity { get; set; }
    }
}