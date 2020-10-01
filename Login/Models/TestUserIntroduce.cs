using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Windows.Input;

namespace Login.Models
{
    public class TestUserIntroduce
    {

        [Key]
        public string UserID { get; set; }

        public string UserPhotoUrl { get; set; }
        public string Name { get; set; }
        public bool LoginStatus { get; set; }//是不是登入的
        public int GoodNumber { get; set; }//被按讚的次數
        public int ChatNumber { get; set; }//聊天次數

    }
}