using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace AD.RTX.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //默认返回 json  
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));

            // Web API 路由
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //ConfigureApi(config);
        }
        public static void ConfigureApi(HttpConfiguration config)
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            //var settings = jsonFormatter.SerializerSettings;

            //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            ////这里使用自定义日期格式
            //timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd'";
            //settings.Converters.Add(timeConverter);

            //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
        }
    }
}
