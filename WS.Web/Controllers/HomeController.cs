using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (Session["CurrentAccountID"] == null)
            {

                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });

            }
            return RedirectToAction("Index", "OfficialAccount", new { Area = "Admin" });


        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
    }
}