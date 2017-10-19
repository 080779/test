using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace AD.RTX.WebApi
{
    public class ApiHelper
    {
        public static JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,//忽略循环引用，如果设置为Error，则遇到循环引用的时候报错（建议设置为Error，这样更规范）
                DateFormatString = "yyyy-MM-dd HH:mm:ss",//日期格式化，默认的格式也不好看
                ContractResolver = new
                Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()//json中属性开头字母小写的驼峰命名
            };
        }
    }
}