using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Model;

namespace WS.ViewModel
{
    public class Orders_ViewModel
    {
        [Display(Name = "订单编号")]
        [Required(ErrorMessage = "请输入订单编号")]
        public System.Guid OrderID { get; set; }
        [Display(Name = "商品编号")]
        [Required(ErrorMessage = "请输入商品编号")]
        public Nullable<System.Guid> GoodID { get; set; }
        [Display(Name = "微信编号")]
        [Required(ErrorMessage = "请输入微信编号")]
        public Nullable<System.Guid> SubscribeID { get; set; }
        [Display(Name = "收货地址")]
        [Required(ErrorMessage = "请输入收货地址")]
        public string Address { get; set; }
        [Display(Name = "手机号")]
        [Required(ErrorMessage = "请输入手机号")]
        public string PhoneNumber { get; set; }
        [Display(Name = "邮政编码")]
        [Required(ErrorMessage = "请输入邮政编码")]
        public string ZipCode { get; set; }
        [Display(Name = "联系人")]
        [Required(ErrorMessage = "请输入联系人")]
        public string ContactPerson { get; set; }
        [Display(Name = "订单状态")]
        [Required(ErrorMessage = "请输入订单状态")]
        public string State { get; set; }
        [Display(Name = "创建时间")]
        [Required(ErrorMessage = "请输入创建时间")]
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<System.Guid> AccountID { get; set; }
        public string CategoryName { get; set; }
        public string SendWayName { get; set; }

        /////////////
        /// 



    }
    /// <summary>
    ///确认发货订单ViewModel
    /// </summary>
    public class Orders_FaHuo_ViewModel
    {
        public System.Guid OrderID { get; set; }
        public Nullable<System.Guid> GoodID { get; set; }
        public Nullable<System.Guid> SubscribeID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ZipCode { get; set; }
        public string ContactPerson { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> AccountID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public string ExpressCompany { get; set; }
        public string ExpressNumber { get; set; }
        public string UserName { get; set; }
        public string GoodsImage { get; set; }
        public string GoodsName { get; set; }
        public string CategoryName { get; set; }
        public string SendWayName { get; set; }
    }


    public class Orders_Total_ViewModel
    {
        public System.Guid OrderID { get; set; }
        public Nullable<System.Guid> GoodID { get; set; }
        public Nullable<System.Guid> SubscribeID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ZipCode { get; set; }
        public string ContactPerson { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> AccountID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public string ExpressCompany { get; set; }
        public string ExpressNumber { get; set; }

        public virtual Goods Goods { get; set; }
        public virtual OfficialAccounts OfficialAccounts { get; set; }
        public virtual Subscribes Subscribes { get; set; }
        public virtual Users Users { get; set; }
    }
}
