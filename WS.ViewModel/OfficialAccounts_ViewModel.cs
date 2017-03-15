using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.ViewModel
{
    public class OfficialAccounts_ViewModel
    {


        [Display(Name = "微信编号")]
        public System.Guid AccountID { get; set; }
        [Display(Name = "微信名称")]
        public string WeiXinName { get; set; }

        [Display(Name = "原始ID")]
        public string OriginalID { get; set; }
        [Display(Name = "微信账号")]
        public string WeiXinNumber { get; set; }
        [Display(Name = "AppID")]
        public string AppID { get; set; }
        [Display(Name = "AppSecret")]
        public string AppSecret { get; set; }
        [Display(Name = "添加时间")]
        public Nullable<System.DateTime> CreateTime { get; set; }
        [Display(Name = "用户编号")]
        public Nullable<System.Guid> UserID { get; set; }
        [Display(Name = "用户名称")]
        public string UserName { get; set; }
        public Nullable<bool> IsConfigure { get; set; }
        public string QRCodeBgImg { get; set; }
        [Display(Name = "增加积分")]

        public Nullable<int> SubscribeAddScore { get; set; }
        [Display(Name = "减少积分")]

        public Nullable<int> UnSubscribeReduceScore { get; set; }
        [Display(Name = "首次关注欢迎语", ShortName = "欢迎语")]
        public string SubscribeWelcome { get; set; }


    }
    public class OfficialAccounts_BaseInfo_ViewModel
    {
        public System.Guid AccountID { get; set; }

        [Required(ErrorMessage = "请输入微信名称")]

        [Display(Name = "微信名称")]
        public string WeiXinName { get; set; }

        [Display(Name = "原始ID")]
        [Required(ErrorMessage = "请输入原始ID")]
        public string OriginalID { get; set; }
        [Display(Name = "微信账号")]
        [Required(ErrorMessage = "请输入微信账号")]
        public string WeiXinNumber { get; set; }
        [Display(Name = "AppID")]
        [Required(ErrorMessage = "请输入AppID")]
        public string AppID { get; set; }
        [Display(Name = "AppSecret")]
        [Required(ErrorMessage = "请输入AppSecret")]
        public string AppSecret { get; set; }

    }
    public class OfficialAccounts_YouZan_ViewModel
    {
        public System.Guid AccountID { get; set; }


        public Nullable<bool> YouZanIsConfig { get; set; }
        [Required(ErrorMessage = "请输入有赞AppID")]

        [Display(Name = "有赞AppID")]
        public string YouZanAppID { get; set; }

        [Display(Name = "有赞AppSecret")]
        [Required(ErrorMessage = "请输入有赞AppSecret")]
        public string YouZanAppSecret { get; set; }
        [Display(Name = "有赞一级分销利率")]
        [Required(ErrorMessage = "请输入有赞一级分销利率")]
        public Nullable<decimal> YZIncomeFirstPercent { get; set; }
        [Display(Name = "有赞二级分销利率")]
        [Required(ErrorMessage = "请输入有赞有赞二级分销利率")]
        public Nullable<decimal> YZIncomeSecondPercent { get; set; }
        [Display(Name = "取现最低金额")]
        [Required(ErrorMessage = "请输入取现最低金额")]
        public Nullable<decimal> YZIncomeCashLimit { get; set; }
        [Display(Name = "启用三级分销")]
        [Required(ErrorMessage = "请输入启用三级分销")]
        public Nullable<bool> YouZanEnable { get; set; }



    }

    public class OfficialAccounts_Score_ViewModel
    {
        public System.Guid AccountID { get; set; }

        [Display(Name = "粉丝关注增加积分")]
        [Required(ErrorMessage = "请输入粉丝关注增加积分")]

        public Nullable<int> SubscribeAddScore { get; set; }
        [Display(Name = "粉丝取消关注减少积分")]
        [Required(ErrorMessage = "请输入粉丝取消关注减少积分")]
        public Nullable<int> UnSubscribeReduceScore { get; set; }
        [Display(Name = "粉丝关注上一级增加积分")]
        [Required(ErrorMessage = "请输入粉丝关注上一级增加积分")]
        public Nullable<int> SubscribeParentAddScore { get; set; }
        [Display(Name = "粉丝取消关注上一级减少积分")]
        [Required(ErrorMessage = "请输入粉丝取消关注上一级减少积分")]
        public Nullable<int> UnSubscribeParentReduceScore { get; set; }
        [Display(Name = "每日签到增加积分")]
        [Required(ErrorMessage = "请输入每日签到增加积分")]
        public Nullable<int> SignAddScore { get; set; }
    }

    /// <summary>
    /// 账号信息完成度
    /// </summary>
    public class OfficialAccounts_Finish_ViewModel
    {
        public Guid AccountID { get; set; }
        public bool BaseInfo { get; set; }
        public bool HaoBao { get; set; }
        public bool PromptLanguage { get; set; }
        public bool Score { get; set; }
        public bool YouZan { get; set; }
    }


    public class OfficialAccounts_AreaLimit_ViewModel
    {
        public Guid AccountID { get; set; }
        [Required]
        [Display(Name = "区域")]
        public string AreaLimit { get; set; }
        [Required]
        [Display(Name = "区域级别")]
        public string AreaLevel { get; set; }

    }



}
