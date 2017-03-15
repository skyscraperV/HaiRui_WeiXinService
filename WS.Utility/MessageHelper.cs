using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Utility
{
    public class MessageHelper
    {
        /// <summary>
        /// 弹出提示信息
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns>最终字符串</returns>
        public string Alert(string message)
        {
            return string.Format("<script>alert('{0}');</script>", message);

        }
        /// <summary>
        /// 弹出提示信息并返回URL地址
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="url">返回地址</param>
        /// <returns>最终字符串</returns>
        public string Alert(string message, string url)
        {
            return string.Format("<script>alert('{0}');window.location.href ='{1}'</script>", message, url);

        }
        /// <summary>
        /// 弹出确认信息并根据操作返回相应URL地址
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="url_Yes">YES返回地址</param>
        /// <param name="url_No">ＮＯ返回地址</param>
        /// <returns>最终字符串</returns>
        public string Confirm(string message, string url_Yes, string url_No)
        {
            return "<Script Language='JavaScript'>if ( window.confirm('" + message + "')) { window.location.href='" + url_Yes + "' } else {window.location.href='" + url_No + "' };</script>";
        }

    }
}
