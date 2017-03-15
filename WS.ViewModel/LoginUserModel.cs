using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WS.ViewModel
{
    public class LoginUserModel
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入用户名！")]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码！")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }


    }
    public class ChangePassWordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassWord { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassWord { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassWord", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassWord { get; set; }
    }
    public class RegisterUserModel
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入用户名！")]
        [Remote("CheckUser", "Users", ErrorMessage = "该用户名已存在")]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码！")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [System.ComponentModel.DataAnnotations.Compare("PassWord", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassWord { get; set; }


        [Display(Name = "邮箱地址")]
        [Required(ErrorMessage = "请输入邮箱地址！")]

        public string EMail { get; set; }


        [Display(Name = "手机号码")]
        [Required(ErrorMessage = "请输入手机号码！")]

        public string PhoneNumber { get; set; }

    }
}
