using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using WS.BLL;
using WS.Model;

namespace WS.Handler.WeiXinService
{
    public class SubscribeService
    {
        /// <summary>
        /// 关注事件
        /// </summary>
        /// <param name="requestMessage"></param>
        public void Subscribe(RequestMessageEvent_Subscribe requestMessage)
        {
            OfficialAccount_BLL obll = new OfficialAccount_BLL();
            OfficialAccount model = obll.Get(a => a.OriginalID == requestMessage.ToUserName);

            if (!AccessTokenContainer.CheckRegistered(model.AppID))//检查是否已经注册
            {
                AccessTokenContainer.Register(model.AppID, model.AppSecret);//如果没有注册则进行注册
            }
            Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson uij = UserApi.Info(model.AppID, requestMessage.FromUserName);

            bool isInArea = false;
            if (model.AreaLimit != null)
            {
                if (model.AreaLevel == "国家" && model.AreaLimit.Contains(uij.country))
                {
                    isInArea = true;
                }
                if (model.AreaLevel == "省份" && model.AreaLimit.Contains(uij.province))
                {
                    isInArea = true;
                }
                if (model.AreaLevel == "城市" && model.AreaLimit.Contains(uij.city))
                {
                    isInArea = true;
                }
            }
            else
            {
                isInArea = true;
            }
            if (!isInArea)
            {
                try
                {
                    Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(model.AppID, uij.openid, "对不起，该活动只限" + model.AreaLimit + "地区用户！");

                }
                catch (Exception e)
                {
                   
                }
            }

            //判断返回体是否带有参数
            bool hasPara = !string.IsNullOrWhiteSpace(requestMessage.EventKey);


            string popenid = hasPara ? requestMessage.EventKey.Substring(8) : null;
            Subscriber_BLL bll = new Subscriber_BLL();

            //扫码者是否存在
            if (bll.GetCount(a => a.OpenID == uij.openid && a.AccountID == model.AccountID) > 0)
            {
                //如果存在（指的是数据库中有没有用的信息）
                Subscriber sub = bll.Get(a => a.OpenID == uij.openid && a.AccountID == model.AccountID);
                if (sub.IsOK == true)
                {
                    sub.FromOpenID = popenid;
                    bll.Update(sub);

                    //如果已关注
                    //Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(model.AppID, sub.OpenID, model.ReSubscribeWelcome);//吃菊
                }
                else
                {
                    //如果已取消关注
                    sub.FromOpenID = popenid;
                    sub.IsOK = true;
                    bll.Update(sub);
                    try
                    {
                        CustomApi.SendText(model.AppID, sub.OpenID, model.ReSubscribeWelcome);
                    }
                    catch (Exception e)
                    {
                       
                    }
                    
                    //再次关注发送消息
                }
            }
            else
            {
                //如果不存在
              


                //添加扫码者信息
                Subscriber sub = new Subscriber
                {
                    SubscribeID = Guid.NewGuid(),
                    AccountID = model.AccountID,
                    NickName = uij.nickname,
                    Province = uij.province,
                    City = uij.city,
                    Country = uij.country,
                    OpenID = uij.openid,
                    Sex = uij.sex,
                    HeadImgUrl = uij.headimgurl,
                    FromOpenID = popenid,
                    SubscribeTime = DateTime.Parse("1970-01-01 08:00").AddSeconds(uij.subscribe_time),
                    Score = 0,
                    SignScore = 0,
                    DirectScore = 0,
                    IndirectScore = 0,
                    SignContinuityCount = 0,
                    UnSubscribeCount = 0,
                    IsOK = true
                };

                bll.Add(sub);
                //发送首次关注欢迎信息
                try
                {
                    CustomApi.SendText(model.AppID, sub.OpenID, model.SubscribeWelcome);

                }
                catch (Exception e)
                {
                   
                }

                if (hasPara)
                {


                    //判断参数对应用户是否存在(第一级)
                    if (bll.GetCount(a => a.OpenID == popenid && a.AccountID == model.AccountID) > 0)
                    {

                        if (isInArea)
                        {
                            Subscriber psub = bll.Get(a => a.OpenID == popenid && a.AccountID == model.AccountID);
                            if (psub.DirectScore != null)
                            {
                                psub.DirectScore += model.SubscribeAddScore;
                            }
                            else
                            {
                                psub.DirectScore = model.SubscribeAddScore;

                            }
                            psub.DirectScoreUpdateTime =
                                DateTime.Parse("1970-01-01 08:00").AddSeconds(uij.subscribe_time);
                            if (psub.Score != null)
                            {
                                psub.Score += model.SubscribeAddScore;
                            }
                            else
                            {
                                psub.Score = model.SubscribeAddScore;
                            }

                            bll.Update(psub);

                            //发送第一级
                            StringBuilder msgp = new StringBuilder();
                            msgp.Append("恭喜，您有新的粉丝【" + uij.nickname + "】加入！\n");
                            msgp.Append("获得推广积分:" + model.SubscribeAddScore + "分，累计积分:" + psub.Score.ToString() + "分\n");
                            CustomApi.SendText(model.AppID, psub.OpenID, msgp.ToString());

                            

                        }


                    }
                    else
                    {
                        if (isInArea)
                        {
                            //第一级用户不存在，需要补充信息
                            Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson puij = UserApi.Info(model.AppID, popenid);
                            //补充上一级信息
                            Subscriber psub = new Subscriber
                            {
                                SubscribeID = Guid.NewGuid(),
                                AccountID = model.AccountID,
                                NickName = puij.nickname,
                                Province = puij.province,
                                City = puij.city,
                                Country = puij.country,
                                OpenID = puij.openid,
                                Sex = puij.sex,
                                HeadImgUrl = puij.headimgurl,
                                SubscribeTime = DateTime.Parse("1970-01-01 08:00").AddSeconds(puij.subscribe_time),
                                Score = model.SubscribeAddScore,
                                SignScore = 0,
                                DirectScore = model.SubscribeAddScore,
                                DirectScoreUpdateTime = sub.SubscribeTime,
                                IndirectScore = 0,
                                SignContinuityCount = 0,
                                UnSubscribeCount = 0,
                                IsOK = true
                            };

                            bll.Add(psub);
                            //发送第一级
                            StringBuilder msgp = new StringBuilder();
                            msgp.Append("恭喜，您有新的粉丝【" + uij.nickname + "】加入！\n");
                            msgp.Append("获得推广积分:" + model.SubscribeAddScore + "分，累计积分:" + psub.Score.ToString() + "分\n");
                            CustomApi.SendText(model.AppID, psub.OpenID, msgp.ToString());
                        }
                        else
                        {
                            //第一级用户不存在，需要补充信息
                            Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson puij = UserApi.Info(model.AppID, popenid);
                            //补充上一级信息
                            Subscriber psub = new Subscriber
                            {
                                SubscribeID = Guid.NewGuid(),
                                AccountID = model.AccountID,
                                NickName = puij.nickname,
                                Province = puij.province,
                                City = puij.city,
                                Country = puij.country,
                                OpenID = puij.openid,
                                Sex = puij.sex,
                                HeadImgUrl = puij.headimgurl,
                                SubscribeTime = DateTime.Parse("1970-01-01 08:00").AddSeconds(puij.subscribe_time),
                                Score = 0,
                                SignScore = 0,
                                DirectScore = 0,
                                DirectScoreUpdateTime = sub.SubscribeTime,
                                IndirectScore = 0,
                                SignContinuityCount = 0,
                                UnSubscribeCount = 0,
                                IsOK = true
                            };

                            bll.Add(psub);
                            //发送第一级


                        }
                    }
                }

            }

        }
        /// <summary>
        /// 取消关注时间
        /// </summary>
        /// <param name="requestMessage"></param>

        public void UnSubscribe(RequestMessageEvent_Unsubscribe requestMessage)
        {
            OfficialAccount_BLL obll = new OfficialAccount_BLL();
            OfficialAccount model = obll.Get(a => a.OriginalID == requestMessage.ToUserName);

            Subscriber_BLL sbll = new Subscriber_BLL();
            if (sbll.GetCount(a => a.OpenID == requestMessage.FromUserName && a.AccountID == model.AccountID) > 0)
            {
                Subscriber sub = sbll.Get(a => a.OpenID == requestMessage.FromUserName && a.AccountID == model.AccountID);
                sub.IsOK = false;
                sub.UnSubscribeTime = DateTime.Now;
                if (sub.UnSubscribeCount != null)
                {
                    sub.UnSubscribeCount += 1;
                }
                else
                {
                    sub.UnSubscribeCount = 1;

                }
                sub.Score = 0;
                sbll.Update(sub);



                if (sub.FromOpenID != null)
                {


                    //if (sub.UnSubscribeCount > 1)
                    //{


                        Subscriber sup = sbll.Get(a => a.OpenID == sub.FromOpenID && a.AccountID == model.AccountID);

                        if (sup.IsOK == true)
                        {

                            sup.Score -= model.UnSubscribeReduceScore;


                            sup.DirectScore -= model.UnSubscribeReduceScore;


                            sbll.Update(sup);

                            StringBuilder sbparent = new StringBuilder();
                            sbparent.Append("您好，你的粉丝【" + sub.NickName + "】取消关注了本公众号！\n");


                            sbparent.Append("减少积分:" + model.UnSubscribeReduceScore + "分，累计积分:" + sup.Score.ToString() +
                                            "分\n");
                            CustomApi.SendText(model.AppID, sup.OpenID, sbparent.ToString());

                        }


                       
                    //}


                }

            }
        }

    }
}