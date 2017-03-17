using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Extension;
using WS.Model;
using WS.Utility;

namespace WS.Web.Areas.Admin.Controllers
{
    [AuthorizeExtension(Roles = "超级管理员")]

    public class QRCodeTemplateController : Controller
    {
        // GET: Admin/QRCodeTemplate
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(QRCode_Template model)
        {
            model.TemplateID = Guid.NewGuid();
            QRCode_Template_BLL bll = new QRCode_Template_BLL();
            bll.Add(model);
            return RedirectToAction("Add");
        }
        public ActionResult List()
        {
            QRCode_Template_BLL bll = new QRCode_Template_BLL();
            var list = bll.GetList();
            return View(list);
        }
        public ActionResult PSD(Guid id)
        {
            QRCode_Template_BLL bll = new QRCode_Template_BLL();
            QRCode_Template temp = bll.Get(a => a.TemplateID == id);
            return View(temp);
        }
        public ActionResult UploadPSD(Guid TemplateID, HttpPostedFileBase fileData)
        {

            if (fileData != null)
            {
                try
                {
                    QRCode_Template_BLL bll = new QRCode_Template_BLL();
                    QRCode_Template temp = bll.Get(a => a.TemplateID == TemplateID);



                    byte[] buffer = new byte[fileData.ContentLength];
                    fileData.InputStream.Read(buffer, 0, fileData.ContentLength);

                    Stream stream = new MemoryStream(buffer);

                    string key = "qrcodetemplatepsd/" + Guid.NewGuid().ToString() + Path.GetExtension(fileData.FileName);
                    if (QiNiuHelper.PutFile(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), key, stream))
                    {


                        if (temp.TemplatePSDUrl != null)
                        {
                            QiNiuHelper.Delete(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), temp.TemplatePSDUrl);
                        }




                        temp.TemplatePSDUrl = key;

                        bll.Update(temp);

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



        public ActionResult Example(Guid id)
        {
            QRCode_Template_BLL bll = new QRCode_Template_BLL();
            QRCode_Template temp = bll.Get(a => a.TemplateID == id);
            return View(temp);
        }
        public ActionResult UploadExample(Guid TemplateID, HttpPostedFileBase fileData)
        {

            if (fileData != null)
            {
                try
                {
                    QRCode_Template_BLL bll = new QRCode_Template_BLL();
                    QRCode_Template temp = bll.Get(a => a.TemplateID == TemplateID);



                    byte[] buffer = new byte[fileData.ContentLength];
                    fileData.InputStream.Read(buffer, 0, fileData.ContentLength);

                    Stream stream = new MemoryStream(buffer);

                    string key = "qrcodetemplateexample/" + Guid.NewGuid().ToString() + Path.GetExtension(fileData.FileName);
                    if (QiNiuHelper.PutFile(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), key, stream))
                    {


                        if (temp.TemplatePSDUrl != null)
                        {
                            QiNiuHelper.Delete(ConfigurationManager.AppSettings["QiNiuBucket"].ToString().Trim(), temp.TemplatePSDUrl);
                        }




                        temp.ExampleUrl = key;

                        bll.Update(temp);

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



    }
}
