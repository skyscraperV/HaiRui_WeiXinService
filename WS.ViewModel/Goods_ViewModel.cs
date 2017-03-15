using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.ViewModel
{
    public class Goods_ViewModel
    {
        public System.Guid GoodID { get; set; }
        [Display(Name = "商品名称")]
        [Required(ErrorMessage = "请输入商品名称")]
        public string GoodName { get; set; }
        [Display(Name = "商品描述")]
        [Required(ErrorMessage = "请输入商品描述")]
        public string Description { get; set; }
        [Display(Name = "商品预览图")]
        [Required(ErrorMessage = "请输入商品预览图")]
        public string Image { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        [Display(Name = "微信公众号")]
        [Required(ErrorMessage = "请输入微信公众号")]
        public Nullable<System.Guid> AccountID { get; set; }
        [Display(Name = "市场价格")]
        [Required(ErrorMessage = "请输入市场价格")]
        public Nullable<double> Price { get; set; }
        [Display(Name = "兑换积分")]
        [Required(ErrorMessage = "请输入兑换积分")]
        public Nullable<int> CostScore { get; set; }
        [Display(Name = "库存数量")]
        [Required(ErrorMessage = "请输入库存数量")]
        public Nullable<int> Count { get; set; }
        public string UserName { get; set; }

        [Display(Name = "商品类型")]

        public Nullable<System.Guid> CategoryID { get; set; }
        [Display(Name = "配送方式")]
        public Nullable<System.Guid> SendWayID { get; set; }
        [Display(Name = "取货地址")]
        public string GetAddress { get; set; }
        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }
        [Display(Name = "商品链接")]
        public string GoodLink { get; set; }
        public Nullable<bool> IsSelling { get; set; }
    }
}
