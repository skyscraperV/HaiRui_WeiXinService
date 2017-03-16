using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WS.Web.Areas.WeiXin.Controllers
{
    public class ShopController : Controller
    {
        // GET: WeiXin/Shop
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Introduce()
        {
            return View();
        }

        public ActionResult Specialty()
        {
            return View();
        }
    }
}