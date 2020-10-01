using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Login.Models
{
    public class Users
    {
        [Key]
        public string ID { get; set; }
        public string FirstCookie { get; set; }//如果cookie被替換掉的話， 指存入第一次的，目的是要分出Message
        public string RoomID { get; set; }//進入的房間名稱
        public string Choose { get; set; }//選擇哪一個房間進入
        public string Wait { get; set; }//沒有房間，有沒有在等房間
        public string Status { get; set; }//現在這個人是甚麼狀態
       

    }
}