using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using WS.BLL;
using WS.Handler.WeiXinService;
using WS.Model;
using WS.Utility;

namespace WS.Handler.WeiXinService
{
    public class HaiBaoService
    {
        public void GetHaiBao(string fromUser, string toUser, string temppath)
        {
            
            GetHaiBaoDelegate del = new GetHaiBaoDelegate(CreateHaiBao);
            IAsyncResult result = del.BeginInvoke(fromUser, toUser, temppath, GetHaiBaoCallback, null);
            //del.EndInvoke(result);

        }




        public delegate void GetHaiBaoDelegate(string fromUser, string toUser, string temppath);

        public void CreateHaiBao(string fromUser, string toUser, string temppath)
        {

            
            OfficialAccount_BLL obll = new OfficialAccount_BLL();
            //OfficialAccount model = obll.Get(a => a.OriginalID == toUser);
            OfficialAccount model = obll.Get(a => a.OriginalID == "gh_9229f06559cd");

            


            if (!AccessTokenContainer.CheckRegistered(model.AppID)) //检查是否已经注册
            {
                AccessTokenContainer.Register(model.AppID, model.AppSecret); //如果没有注册则进行注册
            }
            try
            {

              


                Account_QRCode_Template aqt =
                    new Account_QRCode_Template_BLL().GetList(a => a.AccountID == model.AccountID)
                        .OrderByDescending(a => a.CreateTime)
                        .FirstOrDefault();

                QRCode_Template temp = new QRCode_Template_BLL().Get(a => a.TemplateID == aqt.TemplateID);



                DateTime now = DateTime.Now.AddMinutes(-2);
              WS.Utility.FileHelper.DeleteFilesBeforeTime(temppath, now);




                WebClient wc = new WebClient();
                ICredentials cred;
                cred = new NetworkCredential("user-22", "user-22");

                WebProxy wp = new WebProxy("http://172.18.226.109:808/", true, null, cred);
                wc.Proxy = wp;
                //下载原图
                //string yuanfilename = Guid.NewGuid().ToString() + ".png";
                string yuanlocalfilename =
                    temppath + Guid.NewGuid().ToString() + ".jpg";
                //wc.DownloadFile(ConfigurationManager.AppSettings["QiNiuDomain"] + model.QRCodeBgImg, yuanlocalfilename);
                wc.DownloadFile("http://qiniu.weixin.hairuiit.com/" + aqt.QRCodeBgImg, yuanlocalfilename);


                Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson uij =
                    Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(model.AppID, fromUser);
                //加入昵称
                string savename1 = temppath + Guid.NewGuid().ToString() + ".jpg";
                ImageHelper.Add_FontMark(yuanlocalfilename, savename1, uij.nickname, temp.NickName_FontFamily, (int)temp.NickName_FontSize, Color.Black, (int)temp.NickName_FontX, (int)temp.NickName_FontY);

                //下载头像
                string touxiang = temppath + Guid.NewGuid().ToString() + ".png";
                wc.DownloadFile(uij.headimgurl, touxiang);
                //下载二维码
               // Senparc.Weixin.MP.AdvancedAPIs.QrCode.CreateQrCodeResult createQrCodeResult =QrCodeApi.CreateByStr(model.AppID, fromUser);
                Senparc.Weixin.MP.AdvancedAPIs.QrCode.CreateQrCodeResult createQrCodeResult =QrCodeApi.Create(model.AppID,0,0,Senparc.Weixin.MP.QrCode_ActionName.QR_LIMIT_STR_SCENE,fromUser);
                string localcodefile =
                    temppath + Guid.NewGuid().ToString() + ".png";
                wc.DownloadFile(QrCodeApi.GetShowQrCodeUrl(createQrCodeResult.ticket), localcodefile);
                wc.Dispose();

                ///加头像

                string savename2 = temppath + Guid.NewGuid().ToString() + ".jpg";

                ImageHelper.Add_ImageMark(savename1, touxiang, savename2, (int)temp.HeadImg_X, (int)temp.HeadImg_Y, (int)temp.HeadImg_Width, (int)temp.HeadImg_Height);



                string savename3 = temppath + Guid.NewGuid().ToString() + ".jpg";

                ImageHelper.Add_ImageMark(savename2, localcodefile, savename3, (int)temp.QRCode_X, (int)temp.QRCode_Y, (int)temp.QRCode_Width, (int)temp.QRCode_Height);

                Senparc.Weixin.MP.AdvancedAPIs.Media.UploadTemporaryMediaResult media =
                    Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(model.AppID,
                        Senparc.Weixin.MP.UploadMediaFileType.image, savename3);

                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(model.AppID, fromUser,
                    "专属海报已经接收成功，请保存到手机相册，分享海报兑换奖品哦！");
                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendImage(model.AppID, fromUser,
                    media.media_id);


                Thread.Sleep(6000);
                WS.Utility.FileHelper.Delete(yuanlocalfilename);
                WS.Utility.FileHelper.Delete(savename1);
                WS.Utility.FileHelper.Delete(savename2);
                WS.Utility.FileHelper.Delete(savename3);
                WS.Utility.FileHelper.Delete(touxiang);
                WS.Utility.FileHelper.Delete(localcodefile);


                //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                //responseMessage.Content = "生成成功";
                //return responseMessage;

                //return null;


            }
            catch (Exception ex)
            {
                //LogHelper.ErrorInfo(ex);
                //Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(model.AppID, requestMessage.FromUserName,
                //    ex.Message.ToString());
                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(model.AppID, fromUser,
                   ex.ToString());

            }
        }

        public void GetHaiBaoCallback(IAsyncResult result)
        {
            AsyncResult asyResult = (AsyncResult)result;
            GetHaiBaoDelegate del = (GetHaiBaoDelegate)asyResult.AsyncDelegate;
            del.EndInvoke(result);

        }
    }
}