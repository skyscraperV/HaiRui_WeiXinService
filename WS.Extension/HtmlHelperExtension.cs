using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// Bool类型转String
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="boolin">Bool输入</param>
        /// <param name="truestring">肯定字符串</param>
        /// <param name="falsestring">否定字符串</param>
        /// <returns></returns>
        public static string BoolToString(this HtmlHelper helper, bool boolin, string truestring, string falsestring)
        {
            if (boolin)
            {
                return truestring;
            }
            else
            {
                return falsestring;
            }

        }

        /// <summary>
        /// 过滤掉HTML标签，只保留文本内容
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="HtmlString">HTML文本</param>
        /// <returns>纯文本</returns>
        public static string NoHtml(this HtmlHelper helper, string HtmlString)
        {

            if (!string.IsNullOrEmpty(HtmlString))
            {
                //删除脚本
                HtmlString = Regex.Replace(HtmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删除HTML
                HtmlString = Regex.Replace(HtmlString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"-->", "", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"<!--.*", "", RegexOptions.IgnoreCase);

                HtmlString = Regex.Replace(HtmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                HtmlString = Regex.Replace(HtmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);

                HtmlString.Replace("<", "");
                HtmlString.Replace(">", "");
                HtmlString.Replace("\r\n", "");
                HtmlString = HttpContext.Current.Server.HtmlEncode(HtmlString).Trim();
                return HtmlString.ToString();

            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(this HtmlHelper helper, string str, int length, bool IsAddDot)
        {
            if (str.Length > length)
            {
                if (IsAddDot)
                {
                    str = str.Substring(0, length - 1) + "...";
                }
                else
                {
                    str = str.Substring(0, length - 1);

                }
            }
            else if (string.IsNullOrEmpty(str))
            {
                str = "暂无详细描述。";
            }



            return str;
        }
        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="datetime"></param>
        /// <param name="formatstring"></param>
        /// <returns></returns>
        public static string DateFormat(this HtmlHelper helper, object datetime, string formatstring)
        {
            if (datetime != null)
            {
                return ((DateTime)datetime).ToString(formatstring);

            }
            else
            {
                return "";
            }
        }

        public static string QiNiuImage(this HtmlHelper helper, string key, int width, int height)
        {
            string url = ConfigurationManager.AppSettings["QiNiuDomain"] + key + "?imageView2/1/w/" + width.ToString() + "/h/" + height.ToString();
            return url;
        }
        public static string QiNiuImage(this HtmlHelper helper, string key, int width)
        {
            string url = ConfigurationManager.AppSettings["QiNiuDomain"] + key + "?imageView/1/w/" + width.ToString();
            return url;
        }
        public static string QiNiuImage(this HtmlHelper helper, string key)
        {
            string url = ConfigurationManager.AppSettings["QiNiuDomain"] + key;
            return url;
        }

        public static string SexNumberToString(this HtmlHelper helper, Nullable<int> sex)
        {

            if (sex == 1)
            {
                return "男";

            }
            else if (sex == 2)
            {
                return "女";

            }
            return "未知";

        }

    }
}
