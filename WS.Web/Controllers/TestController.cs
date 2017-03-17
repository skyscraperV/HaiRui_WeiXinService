using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Model;

namespace WS.Web.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {

            QRCode_Template_BLL bll = new QRCode_Template_BLL();
          var list=  bll.GetList();
            WebClient wc = new WebClient();
            foreach (var templatese in list)
            {
                wc.DownloadFile("http://7xofy9.com2.z0.glb.qiniucdn.com/" + templatese.ExampleUrl,@"D:/wenjian/img/" + templatese.ExampleUrl);
                wc.DownloadFile("http://7xofy9.com2.z0.glb.qiniucdn.com/" + templatese.TemplatePSDUrl, @"D:/wenjian/img/" + templatese.TemplatePSDUrl);


            }

            return View();
        }
    }
}