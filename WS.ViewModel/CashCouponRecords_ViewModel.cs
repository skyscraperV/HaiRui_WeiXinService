using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WS.Model;

namespace WS.ViewModel
{
    public partial class CashCouponRecords_ViewModel
    {
        [Display(Name = "编号")]
        public System.Guid CashCouponID { get; set; }
        [Display(Name = "红包订单号")]
        public string Mch_Billno { get; set; }
        [Display(Name = "红包")]
        public Nullable<int> CashAmount { get; set; }
        [Display(Name = "发送时间")]
        public Nullable<System.DateTime> SendTime { get; set; }
        [Display(Name = "结果编号")]
        public string ResultCode { get; set; }
        [Display(Name = "结果")]
        public string ResultMessage { get; set; }
        [Display(Name = "花费积分")]
        public Nullable<int> CostScore { get; set; }
        [Display(Name = "粉丝")]
        public Nullable<System.Guid> SubscribeID { get; set; }
        [Display(Name = "公众号")]
        public Nullable<System.Guid> AccountID { get; set; }

        public virtual OfficialAccounts OfficialAccounts { get; set; }
        public virtual Subscribes Subscribes { get; set; }
    }


    public class CashCoupon_SelectScore_ViewModel
    {
        public Guid SubscribeID { get; set; }

        public int MinScore { get; set; }
        public int AvailableScore { get; set; }
        public int AvailableScoreCount { get; set; }
        [Range(1, 100, ErrorMessage = "zhongsdafkjlsdjfkljasd 豆腐干士大夫")]
        [Remote("_RemoteCheck_SelectScore", "CashCoupon", "WeiXin", AdditionalFields = "AvailableScoreCount", HttpMethod = "POST", ErrorMessage = "数据不对")]
        public int SelectScoreCount { get; set; }
        public int SelectScore { get; set; }
    }
}
