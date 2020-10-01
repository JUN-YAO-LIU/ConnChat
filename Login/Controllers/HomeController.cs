using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Login.Models;


namespace Login.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {

            List<TestUserIntroduce> userintroduce = new List<TestUserIntroduce>
            {
                new TestUserIntroduce { UserID = "A001",UserPhotoUrl = "http://graph.facebook.com/2920433908068817/picture?type=normal",Name = "Jim",LoginStatus = true,GoodNumber = 100 ,ChatNumber = 100},
                new TestUserIntroduce { UserID = "A001",UserPhotoUrl = "http://graph.facebook.com/2920433908068817/picture?type=normal",Name = "Jim",LoginStatus = true,GoodNumber = 100 ,ChatNumber = 100},
                new TestUserIntroduce { UserID = "A001",UserPhotoUrl = "http://graph.facebook.com/2920433908068817/picture?type=normal",Name = "Jim",LoginStatus = true,GoodNumber = 100 ,ChatNumber = 100},
                new TestUserIntroduce { UserID = "A001",UserPhotoUrl = "http://graph.facebook.com/2920433908068817/picture?type=normal",Name = "Jim",LoginStatus = true,GoodNumber = 100 ,ChatNumber = 100}
            };

            return View(userintroduce);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
        public ActionResult GroupChat(string UserId)
        {
            //string UserSex = Session["UserSex"].ToString();

            ViewBag.Message = "Come Chat";
            ViewBag.UserId = UserId;
            ViewBag.Sex = Request.Cookies.Get("gender").Value.ToString().Trim();


            return View();
        }
    }
}