using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WS.BLL;
using WS.Model;
using WS.ViewModel;

namespace WS.Web.Areas.Admin.Controllers
{
    public class SubscribesController : Controller
    {
        [Authorize]
        // GET: Admin/Subscribes
        public ActionResult List(string n, int so = 0, int id = 1)
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid AccountID = Guid.Parse(Session["CurrentAccountID"].ToString());


            ViewBag.SearchParam = n ?? "";
            ViewBag.OrderType = so;


            ViewBag.AccountID = AccountID;
            Subscribes_BLL bll = new Subscribes_BLL();
            List<Subscribes> yuanlist = new List<Subscribes>();
            int totalCount = 0;
            int pageSize = 20;
            if (so == 0)
            {
                if (!String.IsNullOrEmpty(n))
                {

                    yuanlist = bll.GetPageListOrderBy(id, pageSize, a => a.AccountID == AccountID && a.NickName.Contains(n), a => a.SubscribeTime, false).ToList();
                    totalCount = bll.GetCount(a => a.AccountID == AccountID && a.NickName.Contains(n));
                }
                else
                {

                    yuanlist = bll.GetPageListOrderBy(id, pageSize, a => a.AccountID == AccountID, a => a.SubscribeTime, false).ToList();
                    totalCount = bll.GetCount(a => a.AccountID == AccountID);

                }





            }
            else
            {

                if (!String.IsNullOrEmpty(n))
                {

                    yuanlist = bll.GetPageListOrderBy(id, pageSize, a => a.AccountID == AccountID && a.NickName.Contains(n), a => a.Score, false).ToList();
                    totalCount = bll.GetCount(a => a.AccountID == AccountID && a.NickName.Contains(n));

                }
                else
                {

                    yuanlist = bll.GetPageListOrderBy(id, pageSize, a => a.AccountID == AccountID, a => a.Score, false).ToList();
                    totalCount = bll.GetCount(a => a.AccountID == AccountID);

                }


            }

            List<Subscribes> newlist = yuanlist;



            ViewBag.CurrentQuery = "";



            AutoMapper.Mapper.CreateMap<Subscribes, Subscribes_ViewModel>();
            List<Subscribes_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscribes_ViewModel>>(newlist);

            foreach (var item in vlist)
            {
                if (item.FromOpenID != null)
                {

                    try
                    {
                        Subscribes sub = bll.Get(a => a.AccountID == AccountID && a.OpenID == item.FromOpenID);

                        item.FromNickName = sub.NickName;
                        item.FromHeadImgUrl = sub.HeadImgUrl;
                    }
                    catch (Exception ex)
                    {
                        var aaa = ex;

                    }

                }


                item.FansCount = bll.GetCount(a => a.AccountID == AccountID && a.FromOpenID == item.OpenID);
                item.FansStayCount = bll.GetCount(a => a.AccountID == AccountID && a.FromOpenID == item.OpenID && a.IsOK == true);
                item.FansLeaveCount = bll.GetCount(a => a.AccountID == AccountID && a.FromOpenID == item.OpenID && a.IsOK != true);

                double staypercent = 1.00;
                double leavepercent = 0.00;
                if (item.FansCount != 0)
                {
                    staypercent = Math.Round(Convert.ToDouble(item.FansStayCount) / Convert.ToDouble(item.FansCount), 2);
                    leavepercent = Math.Round(Convert.ToDouble(item.FansLeaveCount) / Convert.ToDouble(item.FansCount), 2);

                }
                item.FansStayPercent = (staypercent * 100).ToString() + "%";
                item.FansLeavePercent = (leavepercent * 100).ToString() + "%";
            }


            var list = new StaticPagedList<Subscribes_ViewModel>(vlist, id, pageSize, totalCount);

            return View(list);
        }


        public ActionResult MapList(Guid accountid)
        {

            ViewBag.AccountId = accountid;
            return View();
        }

        public JsonResult GetSubscribesList(Guid accountid)
        {
            Subscribes_BLL bll = new Subscribes_BLL();
            List<Subscribes> list = bll.GetList(a => a.AccountID == accountid && a.Longitude != null && a.Latitude != null).OrderByDescending(a => a.SubscribeTime).ToList();


            AutoMapper.Mapper.CreateMap<Subscribes, Subscribes_ViewModel>();
            List<Subscribes_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscribes_ViewModel>>(list);

            foreach (var item in vlist)
            {
                if (item.FromOpenID != null)
                {
                    Subscribes sub = list.Where(a => a.AccountID == accountid && a.OpenID == item.FromOpenID).FirstOrDefault();
                    item.FromNickName = sub.NickName;
                    item.FromHeadImgUrl = sub.HeadImgUrl;
                }

            }


            return Json(vlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Subscribe(string openid, int id = 1)
        {
            Subscribes_BLL bll = new Subscribes_BLL();

            Subscribes parent = bll.Get(a => a.OpenID == openid);
            ViewBag.Parent = parent;
            List<Subscribes> childlist =
                bll.GetList(a => a.FromOpenID == openid).OrderByDescending(a => a.SubscribeTime).ToList();

            AutoMapper.Mapper.CreateMap<Subscribes, Subscribes_ViewModel>();
            List<Subscribes_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscribes_ViewModel>>(childlist);

            return View(vlist.ToPagedList(id, 50));
        }



        public ActionResult Test(int id = 1)
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid AccountID = Guid.Parse(Session["CurrentAccountID"].ToString());

            Subscribes_BLL bll = new Subscribes_BLL();
            //List<Subscribes> list = bll.GetListOrderBys(a => a.AccountID == AccountID, false,a => a.Score,a=>a.SubscribeTime).ToList();
            List<Subscribes> list = bll.GetPageListOrderBy(id, 50, a => a.AccountID == AccountID, a => a.Score, false).ToList();

            return View(list);
        }
    }
}