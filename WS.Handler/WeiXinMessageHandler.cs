using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using WS.BLL;
using WS.Handler.WeiXinService;
using WS.Model;

namespace WS.Handler
{
    public class WeiXinMessageHandler : MessageHandler<WeiXinMessageContext>
    {
        //private string appId = string.Empty;
        public WeiXinMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            //if (!string.IsNullOrEmpty(postModel.AppId))
            //{
            //    appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            //}

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            //return responseMessage;
            return null;
        }

        /// <summary>
        /// 处理菜单点击事件
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {


            return null;



        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {

            return null;
        }


        #region 定位服务
        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {


            return null;
        }




        #endregion


        #region 关注事件

        /// <summary>
        /// 处理关注事件
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            SubscribeService ss = new SubscribeService();
            ss.Subscribe(requestMessage);

            return null;

        }


        #endregion



        #region 取消关注事件
        /// <summary>
        /// 处理取消关注事件
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {

            SubscribeService ss = new SubscribeService();
            ss.UnSubscribe(requestMessage);
            return null;


        }

        #endregion

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            

            if (requestMessage.Content == "获取海报")
            {
             
                //string path = HttpContext.Current.Server.MapPath("~/Temp/");
                string path = @"E:/test/temp/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

               
                    HaiBaoService hs = new HaiBaoService();
                    hs.GetHaiBao(requestMessage.FromUserName, requestMessage.ToUserName, path);
           


                //HaiBaoService.GetHaiBaoDelegate hdel = new HaiBaoService.GetHaiBaoDelegate(HaiBaoService.CreateHaiBao);
                //IAsyncResult result = hdel.BeginInvoke(requestMessage, path, HaiBaoService.GetHaiBaoCallback, null);
                //hdel.EndInvoke(result);
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "正在发送海报，大约需要几秒钟，请稍候...";


                return responseMessage;
            }

            return null;

        }
    }
}