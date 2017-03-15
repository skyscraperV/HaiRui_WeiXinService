using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Model;
using WS.ViewModel;

namespace WS.Web.Areas.WeiXin.Controllers
{
    public class GoodsController : Controller
    {
        //
        // GET: /WeiXin/Goods/

        public ActionResult List(Guid SubscribeID)
        {
            ViewBag.SubscribeID = SubscribeID;

            Subscribes_BLL subbll = new Subscribes_BLL();

            Subscribes mysub = subbll.Get(a => a.SubscribeID == SubscribeID);
            ViewBag.MyScore = mysub.Score;
            Goods_BLL bll = new Goods_BLL();
            List<Goods> sourselist = bll.GetList(a => a.AccountID == mysub.AccountID && a.IsSelling == true && a.Count > 0).OrderByDescending(a => a.CreateTime).ToList();


            AutoMapper.Mapper.CreateMap<Goods, Goods_ViewModel>();
            List<Goods_ViewModel> list = AutoMapper.Mapper.Map<List<Goods_ViewModel>>(sourselist);


            return View(list);
        }

    }
}

