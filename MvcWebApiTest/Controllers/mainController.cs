using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebApiTest.Controllers
{
    public class mainController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
    }
}