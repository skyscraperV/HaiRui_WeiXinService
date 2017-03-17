using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Model;
using WS.Utility;
using WS.ViewModel;

namespace WS.Web.Areas.Admin.Controllers
{
    public class GoodsController : Controller
    {
        [Authorize]

        // GET: Admin/Goods
        public ActionResult Add()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });
            }
            Guid accountid = Guid.Parse(Session["CurrentAccountID"].ToString());

            Goods_SendWay_BLL sendbll = new Goods_SendWay_BLL();
            ViewBag.SendWaysList = new SelectList(sendbll.GetList().OrderBy(a => a.SendWayOrder), "SendWayID", "SendWayName");
            Goods_Category_BLL categorybll = new Goods_Category_BLL();
            ViewBag.GoodsCategorysList = new SelectList(categorybll.GetList().OrderBy(a => a.CategoryOrder), "CategoryID", "CategoryName");


            Users user = new Users_BLL().GetCurrentUser();



            Goods_ViewModel model = new Goods_ViewModel();

            model.AccountID = accountid;
            return View(model);



        }
        [HttpPost]
        public ActionResult Add(Goods_ViewModel model)
        {
            Users user = new Users_BLL().GetCurrentUser();
            model.GoodID = Guid.NewGuid();
            model.UserID = user.UserID;


            model.IsSelling = true;


            model.CreateTime = DateTime.Now;

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase filebase = Request.Files[0] as HttpPostedFileBase;



                if (filebase.ContentLength > 0)
                {
                    byte[] buffer = new byte[filebase.ContentLength];
                    filebase.InputStream.Read(buffer, 0, filebase.ContentLength);

                    Stream stream = new MemoryStream(buffer);


                    string key = "goods/thumbnail/" + Guid.NewGuid().ToString() + Path.GetExtension(filebase.FileName);
                    if (QiNiuHelper.PutFile(ConfigurationManager.AppSettings["QiNiuBucket"], key, stream))
                    {

                        model.Image = key;
                    }

                }
            }

            AutoMapper.Mapper.CreateMap<Goods_ViewModel, Goods>();
            Goods newmodel = AutoMapper.Mapper.Map<Goods>(model);
            Goods_BLL bll = new Goods_BLL();
            if (bll.Add(newmodel) > 0)
            {
                return RedirectToAction("Add");
            }
            else
            {
                ModelState.AddModelError("", "添加失败，请稍后再试！");

                Goods_SendWay_BLL sendbll = new Goods_SendWay_BLL();
                ViewBag.SendWaysList = new SelectList(sendbll.GetList().OrderBy(a => a.SendWayOrder), "SendWayID", "SendWayName");
                Goods_Category_BLL categorybll = new Goods_Category_BLL();
                ViewBag.GoodsCategorysList = new SelectList(categorybll.GetList().OrderBy(a => a.CategoryOrder), "CategoryID", "CategoryName");




                return View(model);
            }

        }


        public ActionResult Edit(Guid id)
        {


            Goods_SendWay_BLL sendbll = new Goods_SendWay_BLL();
            ViewBag.SendWaysList = new SelectList(sendbll.GetList().OrderBy(a => a.SendWayOrder), "SendWayID", "SendWayName");
            Goods_Category_BLL categorybll = new Goods_Category_BLL();
            ViewBag.GoodsCategorysList = new SelectList(categorybll.GetList().OrderBy(a => a.CategoryOrder), "CategoryID", "CategoryName");


            Goods_BLL bll = new Goods_BLL();
            Goods good = bll.Get(a => a.GoodID == id);

            AutoMapper.Mapper.CreateMap<Goods, Goods_ViewModel>();
            Goods_ViewModel model = AutoMapper.Mapper.Map<Goods_ViewModel>(good);

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Goods_ViewModel model)
        {

            Goods_BLL bll = new Goods_BLL();


            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase filebase = Request.Files[0] as HttpPostedFileBase;



                if (filebase.ContentLength > 0)
                {
                    byte[] buffer = new byte[filebase.ContentLength];
                    filebase.InputStream.Read(buffer, 0, filebase.ContentLength);

                    Stream stream = new MemoryStream(buffer);




                    string key = "goods/thumbnail/" + Guid.NewGuid().ToString() + Path.GetExtension(filebase.FileName);
                    if (QiNiuHelper.PutFile(ConfigurationManager.AppSettings["QiNiuBucket"], key, stream))
                    {
                        if (model.Image != null)
                        {
                            QiNiuHelper.Delete(ConfigurationManager.AppSettings["QiNiuBucket"], model.Image);
                        }
                        model.Image = key;
                    }

                }


            }


            AutoMapper.Mapper.CreateMap<Goods_ViewModel, Goods>();
            Goods newmodel = AutoMapper.Mapper.Map<Goods>(model);
            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("MyList");
            }
            else
            {
                ModelState.AddModelError("", "修改失败，请稍后再试！");


                Goods_SendWay_BLL sendbll = new Goods_SendWay_BLL();
                ViewBag.SendWaysList = new SelectList(sendbll.GetList().OrderBy(a => a.SendWayOrder), "SendWayID", "SendWayName");
                Goods_Category_BLL categorybll = new Goods_Category_BLL();
                ViewBag.GoodsCategorysList = new SelectList(categorybll.GetList().OrderBy(a => a.CategoryOrder), "CategoryID", "CategoryName");


                return View(model);
            }

        }

        public ActionResult MyList()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccount", new { Area = "Admin" });
            }
            Guid accountid = Guid.Parse(Session["CurrentAccountID"].ToString());



            Goods_BLL bll = new Goods_BLL();
            List<Goods> sourselist = bll.GetList(a => a.AccountID == accountid).OrderByDescending(a => a.CreateTime).ToList();

            AutoMapper.Mapper.CreateMap<Goods, Goods_ViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName));
            List<Goods_ViewModel> list = AutoMapper.Mapper.Map<List<Goods_ViewModel>>(sourselist);


            return View(list);
        }


        public ActionResult Details(Guid id)
        {
            Goods_BLL bll = new Goods_BLL();
            Goods good = bll.Get(a => a.GoodID == id);
            AutoMapper.Mapper.CreateMap<Goods, Goods_ViewModel>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName));
            Goods_ViewModel newmodel = AutoMapper.Mapper.Map<Goods_ViewModel>(good);
            return View(newmodel);
        }

        public ActionResult Delete(Guid id)
        {
            try
            {

                Goods_BLL bll = new Goods_BLL();

                Goods good = bll.Get(a => a.GoodID == id);
                if (good.Image != null)
                {
                    QiNiuHelper.Delete(ConfigurationManager.AppSettings["QiNiuBucket"], good.Image);

                }

                bll.Delete(a => a.GoodID == id);

                return RedirectToAction("MyList");


            }
            catch (Exception ex)
            {
                return RedirectToAction("MyList");

            }

        }


        public ActionResult _SetSellState(Guid GoodID, bool IsSelling)
        {
            Goods_BLL bll = new Goods_BLL();
            Goods off = bll.Get(a => a.GoodID == GoodID);
            off.IsSelling = IsSelling;
            bll.Update(off);
            return PartialView(off);
        }

    }
}
