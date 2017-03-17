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
    public class SubscriberController : Controller
    {
        [Authorize]
        // GET: Admin/Subscriber
        public ActionResult List(string n, int so = 0, int id = 1)
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });
            }
            Guid AccountID = Guid.Parse(Session["CurrentAccountID"].ToString());


            ViewBag.SearchParam = n ?? "";
            ViewBag.OrderType = so;


            ViewBag.AccountID = AccountID;
            Subscriber_BLL bll = new Subscriber_BLL();
            List<Subscriber> yuanlist = new List<Subscriber>();
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

            List<Subscriber> newlist = yuanlist;



            ViewBag.CurrentQuery = "";



            AutoMapper.Mapper.CreateMap<Subscriber, Subscriber_ViewModel>();
            List<Subscriber_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscriber_ViewModel>>(newlist);

            foreach (var item in vlist)
            {
                if (item.FromOpenID != null)
                {

                    try
                    {
                        Subscriber sub = bll.Get(a => a.AccountID == AccountID && a.OpenID == item.FromOpenID);

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


            var list = new StaticPagedList<Subscriber_ViewModel>(vlist, id, pageSize, totalCount);

            return View(list);
        }


        public ActionResult MapList(Guid accountid)
        {

            ViewBag.AccountId = accountid;
            return View();
        }

        public JsonResult GetSubscriberList(Guid accountid)
        {
            Subscriber_BLL bll = new Subscriber_BLL();
            List<Subscriber> list = bll.GetList(a => a.AccountID == accountid && a.Longitude != null && a.Latitude != null).OrderByDescending(a => a.SubscribeTime).ToList();


            AutoMapper.Mapper.CreateMap<Subscriber, Subscriber_ViewModel>();
            List<Subscriber_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscriber_ViewModel>>(list);

            foreach (var item in vlist)
            {
                if (item.FromOpenID != null)
                {
                    Subscriber sub = list.Where(a => a.AccountID == accountid && a.OpenID == item.FromOpenID).FirstOrDefault();
                    item.FromNickName = sub.NickName;
                    item.FromHeadImgUrl = sub.HeadImgUrl;
                }

            }


            return Json(vlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Subscribe(string openid, int id = 1)
        {
            Subscriber_BLL bll = new Subscriber_BLL();

            Subscriber parent = bll.Get(a => a.OpenID == openid);
            ViewBag.Parent = parent;
            List<Subscriber> childlist =
                bll.GetList(a => a.FromOpenID == openid).OrderByDescending(a => a.SubscribeTime).ToList();

            AutoMapper.Mapper.CreateMap<Subscriber, Subscriber_ViewModel>();
            List<Subscriber_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscriber_ViewModel>>(childlist);

            return View(vlist.ToPagedList(id, 50));
        }



        public ActionResult Test(int id = 1)
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });
            }
            Guid AccountID = Guid.Parse(Session["CurrentAccountID"].ToString());

            Subscriber_BLL bll = new Subscriber_BLL();
            //List<Subscriber> list = bll.GetListOrderBys(a => a.AccountID == AccountID, false,a => a.Score,a=>a.SubscribeTime).ToList();
            List<Subscriber> list = bll.GetPageListOrderBy(id, 50, a => a.AccountID == AccountID, a => a.Score, false).ToList();

            return View(list);
        }
    }
}