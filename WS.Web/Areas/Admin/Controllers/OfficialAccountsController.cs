using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Model;
using WS.Utility;
using WS.ViewModel;

namespace WS.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class OfficialAccountsController : Controller
    {
        // GET: Admin/OfficialAccounts
        public ActionResult Index()
        {

            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);


            OfficialAccounts_Finish_ViewModel finish = new OfficialAccounts_Finish_ViewModel();
            //基本信息
            if (!string.IsNullOrWhiteSpace(off.WeiXinName)
                && !string.IsNullOrWhiteSpace(off.OriginalID)
                && !string.IsNullOrWhiteSpace(off.WeiXinNumber)
                && !string.IsNullOrWhiteSpace(off.AppID)
                && !string.IsNullOrWhiteSpace(off.AppSecret)
                )
            {
                finish.BaseInfo = true;
            }
            else
            {
                finish.BaseInfo = false;
            }
            //海报
            if (off.Account_QRCode_Template.Count() > 0)
            {
                finish.HaoBao = true;
            }
            else
            {
                finish.HaoBao = false;
            }

            //提示语
            if (!string.IsNullOrWhiteSpace(off.SubscribeWelcome)
                && !string.IsNullOrWhiteSpace(off.ReSubscribeWelcome)
                && !string.IsNullOrWhiteSpace(off.SignLanguage)
                )
            {
                finish.PromptLanguage = true;
            }
            else
            {
                finish.PromptLanguage = false;
            }
            //基本信息
            if (off.SubscribeAddScore != null
                && off.UnSubscribeReduceScore != null
                && off.SubscribeParentAddScore != null
                && off.UnSubscribeParentReduceScore != null
                && off.SignAddScore != null

                )
            {
                finish.Score = true;
            }
            else
            {
                finish.Score = false;
            }
            //基本信息
            if (!string.IsNullOrWhiteSpace(off.YouZanAppID)
                && !string.IsNullOrWhiteSpace(off.YouZanAppSecret)
                && off.YZIncomeFirstPercent != null
               && off.YZIncomeSecondPercent != null
               && off.YZIncomeCashLimit != null
                )
            {
                finish.YouZan = true;
            }
            else
            {
                finish.YouZan = false;
            }


            return View(finish);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(OfficialAccounts_BaseInfo_ViewModel model)
        {
            AutoMapper.Mapper.CreateMap<OfficialAccounts_BaseInfo_ViewModel, OfficialAccounts>();
            OfficialAccounts newmodel = AutoMapper.Mapper.Map<OfficialAccounts>(model);

            newmodel.AccountID = Guid.NewGuid();
            newmodel.CreateTime = DateTime.Now;
            Users user = new Users_BLL().GetCurrentUser();
            newmodel.UserID = user.UserID;
            newmodel.IsConfigure = false;

            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            if (bll.Add(newmodel) > 0)
            {
                return RedirectToAction("Select");
            }
            else
            {
                ModelState.AddModelError("", "添加失败，请稍后再试！");
                return View(model);
            }
        }

        public ActionResult List()
        {

            return View();
        }

        public ActionResult MyList()
        {




            Users user = new Users_BLL().GetCurrentUser();

            if (user.Users_Roles.RoleName != "普通用户")
            {
                OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
                List<OfficialAccounts> sourselist = bll.GetList().ToList();
                AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_ViewModel>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName));
                List<OfficialAccounts_ViewModel> list =
                    AutoMapper.Mapper.Map<List<OfficialAccounts_ViewModel>>(sourselist);

                ViewBag.IsAdmin = true;
                return View(list);


            }
            else
            {
                OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
                List<OfficialAccounts> sourselist = bll.GetList(a => a.UserID == user.UserID).ToList();
                AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_ViewModel>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users.UserName));
                List<OfficialAccounts_ViewModel> list =
                    AutoMapper.Mapper.Map<List<OfficialAccounts_ViewModel>>(sourselist);
                ViewBag.IsAdmin = false;

                return View(list);
            }


        }

        public ActionResult SelectTempLate()
        {

            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid AccountID = Guid.Parse(Session["CurrentAccountID"].ToString());


            ViewBag.AccountID = AccountID;

            QRCode_Templates_BLL bll = new QRCode_Templates_BLL();
            List<QRCode_Templates> list = bll.GetList().OrderBy(a => a.TemplateName).ToList();
            return View(list);

        }
        public ActionResult ConfirmTemplate(Guid AccountID, Guid TemplateID)
        {
            Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();
            Account_QRCode_Template aqt = new Account_QRCode_Template();
            aqt.ATID = Guid.NewGuid();
            aqt.AccountID = AccountID;
            aqt.TemplateID = TemplateID;
            aqt.CreateTime = DateTime.Now;
            bll.Add(aqt);
            return RedirectToAction("SetQRCodeBgImg", new { ATID = aqt.ATID });

        }

        public ActionResult _Configure(Guid AccountID)
        {
            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts off = bll.Get(a => a.AccountID == AccountID);
            off.IsConfigure = true;
            bll.Update(off);
            return PartialView(off);
        }

        public ActionResult Edit(Guid id)
        {

            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);


            AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_ViewModel>();
            OfficialAccounts_ViewModel model = AutoMapper.Mapper.Map<OfficialAccounts_ViewModel>(off);

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(OfficialAccounts_ViewModel model)
        {
            AutoMapper.Mapper.CreateMap<OfficialAccounts_ViewModel, OfficialAccounts>();
            OfficialAccounts newmodel = AutoMapper.Mapper.Map<OfficialAccounts>(model);

            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();

            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("MyList");
            }
            else
            {
                ModelState.AddModelError("", "修改失败，请稍后再试！");

                return View(model);
            }



        }

        //public ActionResult Delete(Guid id)
        //{
        //    try
        //    {

        //        OfficialAccounts_BLL bll = new OfficialAccounts_BLL();



        //        bll.Delete(a => a.AccountID == id);

        //        return RedirectToAction("MyList");


        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("MyList");

        //    }
        //}


        public ActionResult SetQRCodeBgImg(Guid AccountID, Guid TemplateID)
        {
            //Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();
            //Account_QRCode_Template off = bll.Get(a => a.ATID == ATID);

            ViewBag.AccountID = AccountID;
            ViewBag.TemplateID = TemplateID;
            return View();
        }

        public ActionResult GetQRCodeBgImg(Guid AccountID)
        {

            Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();
            Account_QRCode_Template off = bll.GetList(a => a.AccountID == AccountID).OrderByDescending(a => a.CreateTime).FirstOrDefault();


            string url = ConfigurationManager.AppSettings["QiNiuDomain"] + off.QRCodeBgImg;
            return Redirect(url);


        }
        public ActionResult GetCustomQRCodeBgImg(Guid AccountID, int width, int height)
        {
            Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();
            //Account_QRCode_Template off = bll.Get(a => a.ATID == ATID);

            Account_QRCode_Template off = bll.GetList(a => a.AccountID == AccountID).OrderByDescending(a => a.CreateTime).FirstOrDefault();


            string url = ConfigurationManager.AppSettings["QiNiuDomain"] + off.QRCodeBgImg + "?imageView2/1/w/" + width.ToString() + "/h/" + height.ToString();
            return Redirect(url);




        }



        public ActionResult UploadTemplateImage(Guid AccountID, Guid TemplateID, HttpPostedFileBase fileData)
        {

            if (fileData != null)
            {
                try
                {
                    Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();



                    Account_QRCode_Template aqt = new Account_QRCode_Template();
                    aqt.ATID = Guid.NewGuid();
                    aqt.AccountID = AccountID;
                    aqt.TemplateID = TemplateID;
                    aqt.CreateTime = DateTime.Now;
                    //bll.Add(aqt);


                    byte[] buffer = new byte[fileData.ContentLength];
                    fileData.InputStream.Read(buffer, 0, fileData.ContentLength);

                    Stream stream = new MemoryStream(buffer);

                    string key = "qrcodebgimg/" + Guid.NewGuid().ToString() + Path.GetExtension(fileData.FileName);
                    if (QiNiuHelper.PutFile(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), key, stream))
                    {


                        //if (aqt.QRCodeBgImg != null)
                        //{
                        //    QiNiuHelper.Delete(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), aqt.QRCodeBgImg);
                        //}




                        aqt.QRCodeBgImg = key;

                        bll.Add(aqt);

                        string filename = Path.GetFileName(fileData.FileName);
                        string fileurl = ConfigurationManager.AppSettings["QiNiuDomain"] + key;
                        return Json(new { Success = true, FileName = filename, SaveName = fileurl });

                    }
                    else
                    {
                        return Json(new { Success = false, Message = "上传失败，请稍后再试！" });

                    }


                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult _SubscribeMenu()
        {



            Users user = new Users_BLL().GetCurrentUser();
            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            List<OfficialAccounts> list = new List<OfficialAccounts>();
            if (user.Users_Roles.RoleName != "普通用户")
            {
                list = bll.GetList(a => a.IsConfigure == true)
                        .OrderByDescending(a => a.CreateTime)
                        .ToList();
            }
            else
            {
                list = bll.GetList(a => a.UserID == user.UserID && a.IsConfigure == true)
                   .OrderByDescending(a => a.CreateTime)
                   .ToList();
            }



            return PartialView(list);

        }
        public PartialViewResult _OrderMenu()
        {

            Users user = new Users_BLL().GetCurrentUser();
            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            List<OfficialAccounts> list = new List<OfficialAccounts>();
            if (user.Users_Roles.RoleName != "普通用户")
            {
                list = bll.GetList(a => a.IsConfigure == true)
                        .OrderByDescending(a => a.CreateTime)
                        .ToList();
            }
            else
            {
                list = bll.GetList(a => a.UserID == user.UserID && a.IsConfigure == true)
                   .OrderByDescending(a => a.CreateTime)
                   .ToList();
            }

            return PartialView(list);

        }

        public ActionResult CurrentQRCode(Guid AccountID)
        {

            Account_QRCode_Template_BLL bll = new Account_QRCode_Template_BLL();
            var list = bll.GetList(a => a.AccountID == AccountID).ToList();

            if (list.Count() > 0)
            {
                string url = ConfigurationManager.AppSettings["QiNiuDomain"].ToString() + list.OrderByDescending(a => a.CreateTime).FirstOrDefault().QRCodeBgImg;
                return Redirect(url);
            }
            else
            {
                return Content("当前没有进行中的活动！");
            }
        }

        public ActionResult Select()
        {

            Users user = new Users_BLL().GetCurrentUser();
            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            List<OfficialAccounts> list = new List<OfficialAccounts>();
            if (user.Users_Roles.RoleName != "普通用户")
            {
                list = bll.GetList()
                        .OrderByDescending(a => a.CreateTime)
                        .ToList();
            }
            else
            {
                list = bll.GetList(a => a.UserID == user.UserID)
                   .OrderByDescending(a => a.CreateTime)
                   .ToList();
            }

            return View(list);
        }
        public ActionResult ConfirmSelect(Guid id)
        {
            Session["CurrentAccountID"] = id.ToString();

            //return RedirectToAction("Test");
            return RedirectToAction("Index", "Home", new { Area = string.Empty });

        }

        public ActionResult Test()
        {
            if (Session["CurrentAccountID"] != null)
            {

                return Content(Session["CurrentAccountID"].ToString());

            }
            else
            {
                return Content("当前没有选择！");

            }
        }

        public ActionResult _GetCurrentAccountName()
        {

            if (Session["CurrentAccountID"] != null)
            {

                Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());


                OfficialAccounts oa = new OfficialAccounts_BLL().Get(a => a.AccountID == id);
                return Content(oa.WeiXinName);

            }
            else
            {
                return Content("当前没有选择！");

            }
        }


        public ActionResult Edit_BaseInfo()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);


            AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_BaseInfo_ViewModel>();
            OfficialAccounts_BaseInfo_ViewModel model = AutoMapper.Mapper.Map<OfficialAccounts_BaseInfo_ViewModel>(off);

            return View(model);

        }
        [HttpPost]

        public ActionResult Edit_BaseInfo(OfficialAccounts_BaseInfo_ViewModel model)
        {

            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts newmodel = bll.Get(a => a.AccountID == model.AccountID);

            newmodel.WeiXinName = model.WeiXinName;
            newmodel.OriginalID = model.OriginalID;
            newmodel.WeiXinNumber = model.WeiXinNumber;
            newmodel.WeiXinName = model.AppID;
            newmodel.WeiXinName = model.AppSecret;


            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("Edit_BaseInfo");
            }
            else
            {
                ModelState.AddModelError("", "修改失败，请稍后再试！");

                return View(model);
            }

        }

        public ActionResult Edit_PromptLanguage()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);

            return View(off);

        }
        [ValidateInput(false)]
        [HttpPost]

        public ActionResult Save_PromptLanguage(Guid id, string type, string content)
        {
            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts of = bll.Get(a => a.AccountID == id);
            try
            {
                //Regex.Replace(content, "<p>[\s]*</p>", "");
                content = Regex.Replace(content, "<p[^>]*>", "").Replace("</p>", "").Replace("<br>", "\n").Replace("<br/>", "\n");
                //content.Replace(@"<p>", "");
                //content.Replace(@"</p>", "");

                if (type == "welcome")
                {
                    of.SubscribeWelcome = content;
                    bll.Update(of);
                }
                else if (type == "rewelcome")
                {
                    of.ReSubscribeWelcome = content;
                    bll.Update(of);
                }
                else if (type == "sign")
                {
                    of.SignLanguage = content;
                    bll.Update(of);
                }

                return Content("修改成功！");
            }
            catch (Exception)
            {
                return Content("修改失败！");


            }


        }



        public ActionResult Edit_YouZan()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);


            AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_YouZan_ViewModel>();
            OfficialAccounts_YouZan_ViewModel model = AutoMapper.Mapper.Map<OfficialAccounts_YouZan_ViewModel>(off);

            return View(model);
        }

        [HttpPost]

        public ActionResult Edit_YouZan(OfficialAccounts_YouZan_ViewModel model)
        {



            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts newmodel = bll.Get(a => a.AccountID == model.AccountID);
            newmodel.YouZanIsConfig = true;
            newmodel.YouZanAppID = model.YouZanAppID;
            newmodel.YouZanAppSecret = model.YouZanAppSecret;
            newmodel.YZIncomeFirstPercent = model.YZIncomeFirstPercent;
            newmodel.YZIncomeSecondPercent = model.YZIncomeSecondPercent;
            newmodel.YZIncomeCashLimit = model.YZIncomeCashLimit;
            newmodel.YouZanEnable = model.YouZanEnable;




            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("Edit_YouZan");
            }
            else
            {
                ModelState.AddModelError("", "修改失败，请稍后再试！");

                return View(model);
            }

        }


        public ActionResult Edit_Score()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);


            AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_Score_ViewModel>();
            OfficialAccounts_Score_ViewModel model = AutoMapper.Mapper.Map<OfficialAccounts_Score_ViewModel>(off);

            return View(model);
        }

        [HttpPost]

        public ActionResult Edit_Score(OfficialAccounts_Score_ViewModel model)
        {



            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts newmodel = bll.Get(a => a.AccountID == model.AccountID);

            newmodel.SubscribeAddScore = model.SubscribeAddScore;
            newmodel.UnSubscribeReduceScore = model.UnSubscribeReduceScore;
            newmodel.SubscribeParentAddScore = model.SubscribeParentAddScore;
            newmodel.UnSubscribeParentReduceScore = model.UnSubscribeParentReduceScore;
            newmodel.SignAddScore = model.SignAddScore;





            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("Edit_Score");
            }
            else
            {
                ModelState.AddModelError("", "修改失败，请稍后再试！");

                return View(model);
            }

        }


        public ActionResult Edit_Area()
        {
            if (Session["CurrentAccountID"] == null)
            {
                return RedirectToAction("Select", "OfficialAccounts", new { Area = "Admin" });
            }
            Guid id = Guid.Parse(Session["CurrentAccountID"].ToString());
            OfficialAccounts off = new OfficialAccounts_BLL().Get(a => a.AccountID == id);
            AutoMapper.Mapper.CreateMap<OfficialAccounts, OfficialAccounts_AreaLimit_ViewModel>();
            OfficialAccounts_AreaLimit_ViewModel model = AutoMapper.Mapper.Map<OfficialAccounts_AreaLimit_ViewModel>(off);



            List<object> levelList = new List<object>
            {
                new {Value =  "国家", Text = "国家"},
                new {Value = "省份", Text = "省份"},
                new {Value = "城市", Text = "城市"}
            };
            ViewBag.LevelList = new SelectList(levelList, "Value", "Text", false);



            return View(model);
        }
        [HttpPost]

        public ActionResult Edit_Area(OfficialAccounts_AreaLimit_ViewModel model)
        {



            OfficialAccounts_BLL bll = new OfficialAccounts_BLL();
            OfficialAccounts newmodel = bll.Get(a => a.AccountID == model.AccountID);

            newmodel.AreaLevel = model.AreaLevel;
            newmodel.AreaLimit = model.AreaLimit;






            if (bll.Update(newmodel) > 0)
            {
                return RedirectToAction("Edit_Area");
            }
            else
            {
                List<object> levelList = new List<object>
            {
                new {Value =  "国家", Text = "国家"},
                new {Value = "省份", Text = "省份"},
                new {Value = "城市", Text = "城市"}
            };
                ViewBag.LevelList = new SelectList(levelList, "Value", "Text", false);


                ModelState.AddModelError("", "修改失败，请稍后再试！");

                return View(model);
            }

        }




        public ActionResult Delete()
        {
            Guid accountid = Guid.Parse("a7e89f64-9d8a-4a7b-9257-c6a260cc4110");

            Guid userid = (Guid)new OfficialAccounts_BLL().Get(a => a.AccountID == accountid).UserID;


            if (new Account_QRCode_Template_BLL().Delete(a => a.AccountID == accountid) >= 0) ;
            {

                if (new CashCouponRecords_BLL().Delete(a => a.AccountID == accountid) >= 0)
                {
                    if (new Orders_BLL().Delete(a => a.AccountID == accountid) >= 0)
                    {
                        if (new Goods_BLL().Delete(a => a.AccountID == accountid) >= 0)
                        {
                            if (new Subscribes_BLL().Delete(a => a.AccountID == accountid) >= 0)
                            {
                                if (new OfficialAccounts_BLL().Delete(a => a.AccountID == accountid) >= 0)
                                {

                                    if (new OfficialAccounts_BLL().GetCount(a => a.UserID == userid) == 0)
                                    {
                                        if (new Users_BLL().Delete(a => a.UserID == userid) >= 0)
                                        {
                                            return Content("成功并删除用户");
                                        }
                                    }
                                    return Content("成功");


                                }
                            }
                        }
                    }
                }
            }

            return Content("是吧");

        }


    }
}
