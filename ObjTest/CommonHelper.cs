using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ObjTest
{
    public class CommonHelper
    {
        public static T SendMessage<T>(string url, Dictionary<string, string> formFields)
        {
            WebClient wc = new WebClient();
            StringBuilder postData = new StringBuilder();
            foreach (var data in formFields)
            {
                postData.Append("&").Append(data.Key).Append("=").Append(data.Value);
            }
            byte[] sendData = Encoding.UTF8.GetBytes(postData.ToString().Trim('&'));
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("ContentLength", sendData.Length.ToString());
            byte[] recData = wc.UploadData(url, "POST", sendData);
            string resp = Encoding.UTF8.GetString(recData);
            //AjaxResult result = JsonConvert.DeserializeObject<AjaxResult>(resp);
            JavaScriptSerializer js = new JavaScriptSerializer();
            T result = js.Deserialize<T>(resp);
            return result;
        }
        public static T SendMessage<T>(string url)
        {
            WebClient wc = new WebClient();            
            byte[] recData = wc.DownloadData(url);
            string resp = Encoding.UTF8.GetString(recData);
            JavaScriptSerializer js = new JavaScriptSerializer();
            T result = js.Deserialize<T>(resp);
            return result;
        }
        public static HttpResponseMessage toJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}
