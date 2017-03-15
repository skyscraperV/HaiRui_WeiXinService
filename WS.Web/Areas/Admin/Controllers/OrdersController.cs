using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WS.BLL;
using WS.Model;
using WS.ViewModel;

namespace WS.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        // GET: Admin/Orders
        public ActionResult List(string n, string p, int id = 1)
        {

            ViewBag.SearchName = n ?? "";
            ViewBag.SearchPhone = p ?? "";


            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid accountid = Guid.Parse(Session["CurrentAccountID"].ToString());



            List<Orders> list = new List<Orders>();
            Orders_BLL ordbll = new Orders_BLL();

            list = ordbll.GetList(a => a.AccountID == accountid).OrderByDescending(a => a.CreateTime).ToList();

            if (!String.IsNullOrEmpty(n))
            {
                list = list.Where(a => a.ContactPerson.Contains(n)).ToList();

            }

            if (!String.IsNullOrEmpty(p))
            {
                list = list.Where(a => a.PhoneNumber.Contains(p)).ToList();

            }




            ViewBag.AccountID = accountid;

            //return null;
            return View(list.ToPagedList(id, 20));
        }

        public ActionResult FaHuo(Guid id)
        {
            Orders_BLL bll = new Orders_BLL();
            Orders order = bll.Get(a => a.OrderID == id);


            AutoMapper.Mapper.CreateMap<Orders, Orders_FaHuo_ViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Goods.Goods_Categorys.CategoryName))
                .ForMember(dest => dest.GoodsName, opt => opt.MapFrom(src => src.Goods.GoodName))
                .ForMember(dest => dest.GoodsImage, opt => opt.MapFrom(src => src.Goods.Image))
                .ForMember(dest => dest.SendWayName, opt => opt.MapFrom(src => src.Goods.Goods_SendWays.SendWayName));
            Orders_FaHuo_ViewModel model = AutoMapper.Mapper.Map<Orders_FaHuo_ViewModel>(order);
            return View(model);
        }
        [HttpPost]
        public ActionResult FaHuo(Orders_FaHuo_ViewModel model)
        {
            Orders_BLL bll = new Orders_BLL();
            Orders order = bll.Get(a => a.OrderID == model.OrderID);

            if (model.CategoryName == "实物商品")
            {
                if (model.SendWayName == "快递到付" || model.SendWayName == "商家包邮")
                {
                    order.ExpressCompany = model.ExpressCompany;
                    order.ExpressNumber = model.ExpressNumber;
                }
            }


            order.State = "已发货";
            bll.Update(order);


            return RedirectToAction("List", new { accountid = order.AccountID });
        }


    }
}
