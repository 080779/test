using AD.RTX.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MvcWebApiTest.Controllers
{
    public class mainController : Controller
    {        
        public ActionResult Index()
        {
            DomainUser user = new DomainUser();
            user.Name = "lpks";
            user.DisplayName = "猎空";
            user.Department = "财务部";            
            user.TelephoneNumber = "15615615656";
            user.Mail = "aiii10120@qq.com";
            user.Gender = 0;
            user.UserPwd = "Asd123456";
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strUser= js.Serialize(user);
            ViewBag.User = strUser;
            return View();
        }
    }
}