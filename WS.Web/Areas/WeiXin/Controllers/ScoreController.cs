using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using WS.BLL;
using WS.Model;

namespace WS.Web.Areas.WeiXin.Controllers
{
    public class ScoreController : Controller
    {
        //
        // GET: /WeiXin/Score/

        public ActionResult MyScore(string code, string state)
        {
            //string aaa = "code:" + code + " ; state:" + state;
            //return Content(aaa);


            OfficialAccount_BLL obll = new OfficialAccount_BLL();
            Guid id = Guid.Parse(state);
            OfficialAccount off = obll.Get(a => a.AccountID == id);

            //return Content(off.AppID.Trim()+";"+ off.AppSecret.Trim());

            OAuthAccessTokenResult result = OAuthApi.GetAccessToken(off.AppID.Trim(), off.AppSecret.Trim(), code);

            OAuthUserInfo info = OAuthApi.GetUserInfo(result.access_token, result.openid);
            //ViewBag.aaa = info.nickname;
            //ViewBag.bbb = info.openid;

            //return Content(info.nickname + info.openid);
            //return Content(result.openid);


            //string openid = "oqbOQs7w9UNUdksAEnd5zwIxUNCE";

            Subscriber_BLL subbll = new Subscriber_BLL();

            Subscriber mysub = subbll.Get(a => a.OpenID == info.openid);
            if (mysub != null)
            {
                //获取我的粉丝
                List<Subscriber> fans =
                    subbll.GetList(a => a.FromOpenID == info.openid).OrderBy(a => a.SubscribeTime).ToList();
                ViewBag.FansList = fans;
                //获取的我的分数

                if (mysub.Score != null)
                {
                    ViewBag.MyScore = mysub.Score;
                }
                else
                {
                    ViewBag.MyScore = 0;

                }
            }
            else
            {
                ViewBag.FansList = new List<Subscriber>();
                ViewBag.MyScore = 0;

            }



            return View();
        }


        public ActionResult MyScoreBySubscribeID(Guid SubscribeID)
        {


            Subscriber_BLL subbll = new Subscriber_BLL();

            Subscriber mysub = subbll.Get(a => a.SubscribeID == SubscribeID);
            if (mysub != null)
            {
                //获取我的粉丝
                List<Subscriber> fans =
                    subbll.GetList(a => a.FromOpenID == mysub.OpenID).OrderBy(a => a.SubscribeTime).ToList();
                ViewBag.FansList = fans;
                //获取的我的分数

                if (mysub.Score != null)
                {
                    ViewBag.MyScore = mysub.Score;
                }
                else
                {
                    ViewBag.MyScore = 0;
                }
            }
            else
            {
                ViewBag.FansList = new List<Subscriber>();
                ViewBag.MyScore = 0;

            }



            //Subscriber_ViewModel mysubvm = new Subscriber_ViewModel
            //{
            //    SubscribeID = mysub.SubscribeID,
            //    NickName = info.nickname,
            //    HeadImgUrl=info.headimgurl
            //};
            return View("MyScore");
        }





    }
}

