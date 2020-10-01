using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Login.Models
{
    public class GroupChat
    {
        [Key]
        public string ID { get; set; }
        public string Message { get; set; }
        public string Cookie { get; set; }
        public int Good { get; set; }
        public int Wrong { get; set; }
        public string Status { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

    }
}