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
using WS.ViewModel;

namespace WS.Web.Areas.WeiXin.Controllers
{
    public class SubscriberController : Controller
    {
        //
        // GET: /WeiXin/Subscriber/

        public ActionResult Index(string code, string state)
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

            if (mysub != null)
            {

                ViewBag.SubscribeID = mysub.SubscribeID;


            }
            else
            {
                return Content("对不起，您尚未关注该公众号！");
            }
            return View(mysub);
        }
        public ActionResult IndexById(Guid SubscribeID)
        {

            Subscriber_BLL subbll = new Subscriber_BLL();

            Subscriber mysub = subbll.Get(a => a.SubscribeID == SubscribeID);

            if (mysub != null)
            {

                ViewBag.SubscribeID = mysub.SubscribeID;


            }
            else
            {
                return Content("对不起，您尚未关注该公众号！");
            }
            return View("Index", mysub);
        }

        public ActionResult _LayoutHeader(Guid SubscribeID)
        {
            Subscriber_BLL subbll = new Subscriber_BLL();

            Subscriber mysub = subbll.Get(a => a.SubscribeID == SubscribeID);


            LayoutHeader_ViewModel vm = new LayoutHeader_ViewModel();
            vm.SubscribeID = mysub.SubscribeID;
            vm.Score = mysub.Score == null ? 0 : (int)mysub.Score;
            vm.SubscribeTime = (DateTime)mysub.SubscribeTime;
            vm.NickName = mysub.NickName;
            vm.HeadImgUrl = mysub.HeadImgUrl;

            vm.ChildSubscribeCount = subbll.GetCount(a => a.FromOpenID == mysub.OpenID);
            vm.IsYouZan = mysub.OfficialAccount.YouZanEnable;
            return PartialView(vm);


        }

        public ActionResult ScoreRanking(Guid SubscribeID)
        {
            Subscriber_BLL bll = new Subscriber_BLL();

            Subscriber my = bll.Get(a => a.SubscribeID == SubscribeID);
            List<Subscriber> list = bll.GetPageListOrderBy(1, 20, a => a.AccountID == my.AccountID && a.Score > 0, a => a.Score, false).ToList();

            AutoMapper.Mapper.CreateMap<Subscriber, Subscriber_ViewModel>();
            List<Subscriber_ViewModel> vlist = AutoMapper.Mapper.Map<List<Subscriber_ViewModel>>(list);


            foreach (var item in vlist)
            {


                item.FansCount = bll.GetCount(a => a.FromOpenID == item.OpenID);


            }


            ViewBag.SubscribeID = SubscribeID;


            return View(vlist);
        }
    }
}
