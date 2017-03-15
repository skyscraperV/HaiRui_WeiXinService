using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using WS.BLL;
using WS.Model;
using WS.Utility;
using WS.ViewModel;

namespace WS.Web.Areas.WeiXin.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /WeiXin/Order/

        public ActionResult Add(Guid GoodID, Guid SubscribeID)
        {



            Orders_BLL obll = new Orders_BLL();
            int count = obll.GetCount(a => a.GoodID == GoodID && a.SubscribeID == SubscribeID);


            if (count > 0)
            {
                MessageHelper mh = new MessageHelper();
                string messagestr = mh.Alert("抱歉，您已经兑换过此商品，每人限兑换一个。", Url.Action("MyList", "Order", new { area = "WeiXin", SubscribeID = SubscribeID }));

                return Content(messagestr);

            }

            Goods good = new Goods_BLL().Get(a => a.GoodID == GoodID);





            ViewBag.GoodName = good.GoodName;
            ViewBag.CostScore = good.CostScore;






            Orders order = new Orders();
            order.OrderID = Guid.NewGuid();
            order.GoodID = GoodID;
            order.SubscribeID = SubscribeID;
            order.AccountID = good.AccountID;
            order.UserID = good.UserID;


            AutoMapper.Mapper.CreateMap<Orders, Orders_ViewModel>();
            Orders_ViewModel model = AutoMapper.Mapper.Map<Orders_ViewModel>(order);
            model.CategoryName = good.Goods_Categorys.CategoryName;
            model.SendWayName = good.Goods_SendWays.SendWayName;

            return View(model);
        }
        [HttpPost]
        public ActionResult Add(Orders_ViewModel model)
        {
            AutoMapper.Mapper.CreateMap<Orders_ViewModel, Orders>();
            Orders order = AutoMapper.Mapper.Map<Orders>(model);
            order.State = "未发货";

            if (model.CategoryName == "虚拟商品")
            {
                order.State = "已发货";

            }

            order.CreateTime = DateTime.Now;
            Orders_BLL bll = new Orders_BLL();
            bll.Add(order);
            Goods_BLL gbll = new Goods_BLL();
            Goods good = gbll.Get(a => a.GoodID == model.GoodID);

            Subscribes_BLL sbll = new Subscribes_BLL();
            Subscribes sub = sbll.Get(a => a.SubscribeID == model.SubscribeID);

            if (sub.Score >= 0 && sub.Score >= good.CostScore)
            {
                sub.Score = sub.Score - good.CostScore;
                if (sub.ScoreUsed != null)
                {
                    sub.ScoreUsed += good.CostScore;

                }
                else
                {
                    sub.ScoreUsed = good.CostScore;

                }
                sbll.Update(sub);

                good.Count = good.Count - 1;
                gbll.Update(good);
                //Subscribes sub = new Subscribes_BLL().Get(a => a.SubscribeID == model.SubscribeID);

                //string link = WeiXinHelper.AuthorizeUrl(sub.OfficialAccounts.AppID, Url.Content("~/WeiXin/Order/MyList"),
                //    sub.AccountID.ToString());

                return RedirectToAction("MyList", "Order", new { SubscribeID = model.SubscribeID });
            }
            else
            {
                ModelState.AddModelError("", "抱歉，您的积分不够！");
                return View(model);
            }


        }

        public ActionResult MyList(Guid SubscribeID)
        {

            List<Orders> list = new Orders_BLL().GetList(a => a.SubscribeID == SubscribeID).OrderByDescending(a => a.CreateTime).ToList();

            return View(list);

        }



        public ActionResult Details(Guid orderid)
        {
            Orders order = new Orders_BLL().Get(a => a.OrderID == orderid);
            AutoMapper.Mapper.CreateMap<Orders, Orders_Total_ViewModel>();
            Orders_Total_ViewModel model = AutoMapper.Mapper.Map<Orders_Total_ViewModel>(order);

            return View(model);
        }

        public ActionResult List(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            Guid accountid = Guid.Parse(state.Trim());

            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == accountid);

            OAuthAccessTokenResult result = new OAuthAccessTokenResult();
            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(off.AppID.Trim(), off.AppSecret.Trim(), code);

            }
            catch (Exception ex)
            {
                if (Session["OAuthAccessToken"] != null)
                {
                    result = Session["OAuthAccessToken"] as OAuthAccessTokenResult;
                    try
                    {
                        result = OAuthApi.RefreshToken(off.AppID.Trim(), result.refresh_token);

                    }
                    catch (Exception)
                    {

                        return Content("错误：" + result.errmsg);

                    }
                }
                else
                {
                    return Content("错误：" + result.errmsg);

                }

            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);

            }


            Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            Session["OAuthAccessToken"] = result;
            OAuthUserInfo info = OAuthApi.GetUserInfo(result.access_token, result.openid);

            Subscribes_BLL subbll = new Subscribes_BLL();

            Subscribes mysub = subbll.Get(a => a.OpenID == info.openid);

            List<Orders> list = new Orders_BLL().GetList(a => a.SubscribeID == mysub.SubscribeID).OrderByDescending(a => a.CreateTime).ToList();

            return View("MyList", list);
        }

    }
}
