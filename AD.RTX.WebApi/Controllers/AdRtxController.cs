using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AD.RTX.WebApi.Controllers
{
    public class AdRtxController : ApiController
    {
        public IEnumerable<string> Get()
        {
            List<string> list = new List<string>();
            list.Add("跳舞");
            list.Add("猫腻");
            list.Add("三少");
            list.Add("番茄");
            list.Add("江南");
            list.Add("烽火");
            return list;
        }
    }
}
