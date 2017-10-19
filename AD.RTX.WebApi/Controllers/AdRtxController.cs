using AD.RTX.WebApi;
using Newtonsoft.Json;
using ObjTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AD.RTX.WebApi.Controllers
{
    public class AdRtxController : ApiController
    {
        //[HttpGet]
        //public string Get()
        //{
        //    return "三少";
        //}
        [HttpGet]
        public IEnumerable<AjaxReslut> GetAll()
        {
            List<AjaxReslut> list = new List<AjaxReslut>();
            list.Add(new AjaxReslut { Status = "ok", Msg = "test" });
            list.Add(new AjaxReslut { Status = "error", Msg = "testerroe" });
            list.Add(new AjaxReslut { Status = "fail", Msg = "testfail" });
            //list.Add("跳舞");
            //list.Add("猫腻");
            //list.Add("三少");
            //list.Add("番茄");
            //list.Add("江南");
            //list.Add("烽火");
            return list;
        }        
        [HttpPost]
        public IHttpActionResult Post()
        {            
            return Json <DateTime>(DateTime.Now, ApiHelper.JsonSettings());
        }
        [HttpPost]
        public IHttpActionResult PostTest()
        {
            AjaxReslut rs = new AjaxReslut { Status = "ok", Msg = "test" };            
            return Json(rs,ApiHelper.JsonSettings());
        }
    }
}
