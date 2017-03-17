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



            Goods_Order_BLL obll = new Goods_Order_BLL();
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






            Goods_Order order = new Goods_Order();
            order.OrderID = Guid.NewGuid();
            order.GoodID = GoodID;
            order.SubscribeID = SubscribeID;
            order.AccountID = good.AccountID;
            order.UserID = good.UserID;


            AutoMapper.Mapper.CreateMap<Goods_Order, Goods_Order_ViewModel>();
            Goods_Order_ViewModel model = AutoMapper.Mapper.Map<Goods_Order_ViewModel>(order);
            model.CategoryName = good.Goods_Category.CategoryName;
            model.SendWayName = good.Goods_SendWay.SendWayName;

            return View(model);
        }
        [HttpPost]
        public ActionResult Add(Goods_Order_ViewModel model)
        {
            AutoMapper.Mapper.CreateMap<Goods_Order_ViewModel, Goods_Order>();
            Goods_Order order = AutoMapper.Mapper.Map<Goods_Order>(model);
            order.State = "未发货";

            if (model.CategoryName == "虚拟商品")
            {
                order.State = "已发货";

            }

            order.CreateTime = DateTime.Now;
            Goods_Order_BLL bll = new Goods_Order_BLL();
            bll.Add(order);
            Goods_BLL gbll = new Goods_BLL();
            Goods good = gbll.Get(a => a.GoodID == model.GoodID);

            Subscriber_BLL sbll = new Subscriber_BLL();
            Subscriber sub = sbll.Get(a => a.SubscribeID == model.SubscribeID);

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
                //Subscriber sub = new Subscriber_BLL().Get(a => a.SubscribeID == model.SubscribeID);

                //string link = WeiXinHelper.AuthorizeUrl(sub.OfficialAccount.AppID, Url.Content("~/WeiXin/Order/MyList"),
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

            List<Goods_Order> list = new Goods_Order_BLL().GetList(a => a.SubscribeID == SubscribeID).OrderByDescending(a => a.CreateTime).ToList();

            return View(list);

        }



        public ActionResult Details(Guid orderid)
        {
            Goods_Order order = new Goods_Order_BLL().Get(a => a.OrderID == orderid);
            AutoMapper.Mapper.CreateMap<Goods_Order, Goods_Order_Total_ViewModel>();
            Goods_Order_Total_ViewModel model = AutoMapper.Mapper.Map<Goods_Order_Total_ViewModel>(order);

            return View(model);
        }

        public ActionResult List(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            Guid accountid = Guid.Parse(state.Trim());

            OfficialAccount off = new OfficialAccount_BLL().Get(a => a.AccountID == accountid);

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

            Subscriber_BLL subbll = new Subscriber_BLL();

            Subscriber mysub = subbll.Get(a => a.OpenID == info.openid);

            List<Goods_Order> list = new Goods_Order_BLL().GetList(a => a.SubscribeID == mysub.SubscribeID).OrderByDescending(a => a.CreateTime).ToList();

            return View("MyList", list);
        }

    }
}
