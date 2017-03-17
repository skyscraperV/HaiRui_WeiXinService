using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using WS.BLL;
using WS.Model;

namespace WS.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        // GET: Admin/Menu
        public ActionResult Index()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });
            }
            Guid accountid = Guid.Parse(Session["CurrentAccountID"].ToString());

            ViewBag.AccountID = accountid;
            OfficialAccount_BLL bll = new OfficialAccount_BLL();
            OfficialAccount oa = bll.Get(a => a.AccountID == accountid);
            ViewBag.AppID = oa.AppID;
            string accessToken = AccessTokenContainer.TryGetAccessToken(oa.AppID, oa.AppSecret);

            GetMenuResult result = CommonApi.GetMenu(accessToken);


            return View(result);
            //return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMenuJson(Guid accountid)
        {

            OfficialAccount_BLL bll = new OfficialAccount_BLL();
            OfficialAccount oa = bll.Get(a => a.AccountID == accountid);

            string accessToken = AccessTokenContainer.TryGetAccessToken(oa.AppID, oa.AppSecret);

            GetMenuResult result = CommonApi.GetMenu(accessToken);

            foreach (var item in result.menu.button)
            {
                var a2a = item.GetType();
            }

            var aa = result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGuid()
        {
            return Content(Guid.NewGuid().ToString());
        }

        public ActionResult SaveMenu(FormCollection fc)
        {
            Guid id = Guid.Parse(fc["accountid"].ToString());
            string menustr = fc["menustr"].ToString();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var jsonResult = js.Deserialize<GetMenuResultFull>(menustr);
            ButtonGroup bg = new ButtonGroup();
            GetMenuResult aasaas = CommonApi.GetMenuFromJsonResult(jsonResult, bg);
            OfficialAccount_BLL bll = new OfficialAccount_BLL();
            OfficialAccount oa = bll.Get(a => a.AccountID == id);
            string accessToken = AccessTokenContainer.TryGetAccessToken(oa.AppID, oa.AppSecret);
            Senparc.Weixin.Entities.WxJsonResult rrr = CommonApi.CreateMenu(accessToken, aasaas.menu);
            return Json(rrr.errcode.ToString(), JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult _AddParentMenu()
        {

            Guid id = Guid.NewGuid();
            ViewBag.ParentId = id;
            return PartialView();
        }

        public ActionResult _AddChildMenu()
        {
            Guid id = Guid.NewGuid();
            ViewBag.ChildId = id;

            return PartialView();
        }
    }
}
