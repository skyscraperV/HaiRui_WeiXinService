using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.IO;
using Qiniu.RPC;
using Qiniu.RS;
using Qiniu.Util;

namespace WS.Utility
{
    public static class QiNiuHelper
    {
        static QiNiuHelper()
        {
            Qiniu.Conf.Config.ACCESS_KEY = ConfigurationManager.AppSettings["QiNiuAccessKey"];
            Qiniu.Conf.Config.SECRET_KEY = ConfigurationManager.AppSettings["QiNiuSecretKey"];
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bucket">空间名称</param>
        /// <param name="key">文件名</param>
        /// <param name="putStream">文件流</param>
        public static bool PutFile(string bucket, string key, Stream putStream)
        {

            var policy = new PutPolicy(bucket, 3600);
            string upToken = policy.Token();
            PutExtra extra = new PutExtra();
            IOClient client = new IOClient();
            //client.PutFile(upToken, key, fname, extra);
            CallRet ret = client.Put(upToken, key, putStream, extra);
            return ret.OK;
        }
        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="bucket">文件所在的空间名</param>
        /// <param name="key">文件key</param>
        public static bool Delete(string bucket, string key)
        {
            Console.WriteLine("\n===> Delete {0}:{1}", bucket, key);
            RSClient client = new RSClient();
            CallRet ret = client.Delete(new EntryPath(bucket, key));
            return ret.OK;
        }
    }
}
