using AD.RTX.WebApi;
using Newtonsoft.Json;
using ObjTest;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AD.RTX.WebApi.Controllers
{
    public class AdRtxController : ApiController
    {
        /*
        [HttpGet]
        public IEnumerable<AjaxResult> GetAll()
        {
            List<AjaxResult> list = new List<AjaxResult>();
            list.Add(new AjaxResult { Status = "ok", Msg = "test" });
            list.Add(new AjaxResult { Status = "error", Msg = "testerroe" });
            return list;
        }        
        [HttpPost]
        public IHttpActionResult Post()
        {            
            return Json <DateTime>(DateTime.Now, ApiHelper.JsonSettings());
        }*/

        public IHttpActionResult AddUser(DomainUser user)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            DirectoryEntry entry = ado.GetEntry();
            string filter = "(&(objectclass=organizationalUnit)(ou=" + user.Department + "))";
            DirectoryEntry ouEntry = ado.GetOUEntry(entry, filter);
            if (ouEntry == null)
            {
                return Json(new AjaxResult { Status = "error", Msg = "nonDept" });
            }
            if (ado.IsADUserExist(ouEntry, user.Name))
            {
                return Json(new AjaxResult { Status = "error", Msg = "用户已经存在" });
            }
            if (!ado.AddAccount(ouEntry, user))
            {
                return Json(new AjaxResult { Status = "error", Msg = "添加用户到域失败"});
            }
            RtxManager rm = new RtxManager();
            string[] paths = ouEntry.Path.Replace("LDAP://192.168.31.134/", "").Replace(",DC=test,DC=com", "").Replace("OU=", "").Split(',');
            string path = "";
            for (int i = paths.Length - 1; i >= 0; i--)
            {
                path = path + paths[i] + @"\";
            }

            if (!rm.AddEditRtxUser(user, path, 1))
            {
                ado.GetUserEntry(entry, user.Name).DeleteTree();
                return Json(new AjaxResult { Status = "error", Msg = "rtx添加用户失败" });
            }
            return Json(new AjaxResult { Status = "ok", Msg = "success", Data = path });
        }

        public IHttpActionResult AddOUDept(string parentDeptName, string deptName)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            DirectoryEntry entry = ado.GetEntry();
            string filter = "(&(objectclass=organizationalUnit)(ou=" + parentDeptName + "))";
            DirectoryEntry ouEntry = ado.GetOUEntry(entry, filter);
            if (!ado.AddOUEntry(ouEntry, deptName))
            {
                return Json(new AjaxResult { Status = "error", Msg = "ad域中添加部门失败" });
            }
            RtxManager rm = new RtxManager();
            if (!rm.AddDept(deptName, parentDeptName))
            {
                filter = "(&(objectclass=organizationalUnit)(ou=" + deptName + "))";
                ado.DelEntry(ado.GetOUEntry(entry, filter));
                return Json(new AjaxResult { Status = "error", Msg = "RTX中添加部门失败" });
            }
            return Json(new AjaxResult { Status = "ok", Msg = "部门同步添加成功" });
        }

        public IHttpActionResult EditOUDept(string deptName, string newDeptName)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            DirectoryEntry entry = ado.GetEntry();
            string filter = "(&(objectclass=organizationalUnit)(ou=" + deptName + "))";
            DirectoryEntry ouEntry = ado.GetOUEntry(entry, filter);
            if (!ado.OUEntryReName(ouEntry, newDeptName))
            {
                return Json(new AjaxResult { Status = "error", Msg = "ad域中编辑部门失败" });
            }
            RtxManager rm = new RtxManager();
            if (!rm.SetDeptName(deptName, newDeptName))
            {
                filter = "(&(objectclass=organizationalUnit)(ou=" + newDeptName + "))";
                ado.OUEntryReName(ado.GetOUEntry(entry, filter), deptName);
                return Json(new AjaxResult { Status = "error", Msg = "RTX中编辑部门失败" });
            }
            return Json(new AjaxResult { Status = "ok", Msg = "部门同步编辑成功" });
        }

        public IHttpActionResult EditUser(DomainUser user)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            //ado.SetADConfig(user.ComName, filePath);
            DirectoryEntry entry = ado.GetEntry();
            string filter = "(&(objectclass=organizationalUnit)(ou=" + user.Department + "))";
            DirectoryEntry ouEntry = ado.GetOUEntry(entry, filter);
            if (ouEntry == null)
            {
                return Json(new AjaxResult { Status = "error", Msg = "部门不存在" });
            }
            DirectoryEntry userEntry = ado.GetUserEntry(entry, user.Name);
            if (userEntry == null)
            {
                return Json(new AjaxResult { Status = "error", Msg = "用户不存在" });
            }
            if (!userEntry.Path.Contains(user.Department))
            {
                ado.UserMoveToOU(ouEntry, userEntry);
            }
            if (!ado.EditAccount(userEntry, user))
            {
                return Json(new AjaxResult { Status = "error", Msg = "编辑用户到域失败" });
            }
            RtxManager rm = new RtxManager();
            string[] paths = ouEntry.Path.Replace("LDAP://192.168.31.134/", "").Replace(",DC=test,DC=com", "").Replace("OU=", "").Split(',');
            string path = "";
            for (int i = paths.Length - 1; i >= 0; i--)
            {
                path = path + paths[i] + @"\";
            }

            if (!rm.AddEditRtxUser(user, path, 1))
            {
                ado.GetUserEntry(entry, user.Name).DeleteTree();
                return Json(new AjaxResult { Status = "error", Msg = "rtx编辑用户失败" });
            }
            return Json(new AjaxResult { Status = "ok", Msg = "rtx编辑用户成功" });
        }
        
        public IHttpActionResult DelUser(string userName)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            DirectoryEntry entry = ado.GetEntry();
            DirectoryEntry userEntry = ado.GetUserEntry(entry, userName);
            ado.DelEntry(userEntry);
            RtxManager rm = new RtxManager();
            rm.RemoveUser(userName);
            return Json(new AjaxResult { Status = "ok", Msg = "用户删除成功" });
        }

        public IHttpActionResult DelOUDept(string deptName)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ADConfig.xml");
            AdOperate ado = new AdOperate(filePath);
            DirectoryEntry entry = ado.GetEntry();
            string filter = "(&(objectclass=organizationalUnit)(ou=" + deptName + "))";
            DirectoryEntry ouEntry = ado.GetOUEntry(entry, filter);
            ado.DelEntry(ouEntry);
            RtxManager rm = new RtxManager();
            rm.DelDept(deptName);
            return Json(new AjaxResult { Status = "ok", Msg = "部门删除成功" });
        }

    }
}
