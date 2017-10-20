using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTXSAPILib;

namespace AD.RTX.WebApi
{
    public class RtxManager
    {
        RTXSAPILib.RTXSAPIRootObj RootObj;  //声明一个根对象
        RTXSAPILib.RTXSAPIDeptManager DeptManager;
        RTXSAPILib.IRTXSAPIUserManager UserManager;
        
        public RtxManager()
        {
            RootObj = new RTXSAPIRootObj();     //创建根对象            
            RootObj.ServerIP = "127.0.0.1";
            RootObj.ServerPort = Convert.ToInt16("8006");
            DeptManager = RootObj.DeptManager;
            UserManager = RootObj.UserManager;
        }

        public bool IsUserExist(string bstrUserName)
        {
            return UserManager.IsUserExist(bstrUserName);
        }

        public bool AddRtxUser(string bstrUserName, int IAuthType)
        {
            try
            {
                UserManager.AddUser(bstrUserName, IAuthType);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveUser(string userName)
        {
            try
            {
                UserManager.DeleteUser(userName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetBasicRtxUser(string bstrUserName, string bstrName = "RTX_NULL", int gender = -1, string bstrMobile = "RTX_NULL", string bstrEMail = "RTX_NULL", string bstrPhone = "RTX_NULL", int IAuthType = -1)
        {
            try
            {
                UserManager.SetUserBasicInfo(bstrUserName, bstrName, gender, bstrMobile, bstrEMail, bstrPhone, IAuthType);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddUserToDept(string bstrUserName, string bstrSrcDeptName, string bstrDestDeptName, bool bIsCopy)
        {
            try
            {
                DeptManager.AddUserToDept(bstrUserName, bstrSrcDeptName, bstrDestDeptName, bIsCopy);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsDeptExist(string bstrDeptName)
        {
            return DeptManager.IsDeptExist(bstrDeptName);
        }

        public bool AddDept(string bstrDeptName, string bstrParentDept)
        {
            try
            {
                DeptManager.AddDept(bstrDeptName, bstrParentDept);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetDeptName(string bstrDeptName, string bstrNewDeptName)
        {
            try
            {
                DeptManager.SetDeptName(bstrDeptName, bstrNewDeptName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveDept(string bstrDeptName)
        {
            try
            {
                DeptManager.DelDept(bstrDeptName, false);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DelDept(string bstrDeptName)
        {
            try
            {
                DeptManager.DelDept(bstrDeptName, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetUserDeptsName(string userName)
        {
            try
            {
                string[] paths = DeptManager.GetUserDepts(userName).Split('"');
                return paths[1];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool AddEditRtxUser(DomainUser user,string path,int iAuthType)
        {
            string userName = user.Name;
            string displayName = "RTX_NULL";
            int gender = -1;
            string mobile = "RTX_NULL";
            string email = "RTX_NULL";
            string phone = "RTX_NULL";
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                displayName = user.DisplayName;
            }
            if (!string.IsNullOrEmpty(user.Mail))
            {
                email = user.Mail;
            }
            if (!string.IsNullOrEmpty(user.TelephoneNumber))
            {
                mobile = user.TelephoneNumber;
                phone = mobile;
            }
            if (user.Gender==0 || user.Gender==1)
            {
                gender = user.Gender;
            }

            if (IsUserExist(user.Name))
            {
                if(!SetBasicRtxUser(userName, displayName, gender, mobile, email, phone, iAuthType))
                {
                    return false;
                }
                if(!AddUserToDept(user.Name, GetUserDeptsName(user.Name), path, false))
                {
                    return false;
                }
                return true;
            }
            else
            {
                if(AddRtxUser(user.Name,iAuthType))
                {                    
                    if(SetBasicRtxUser(userName,displayName,gender,mobile,email,phone,iAuthType))
                    {
                        if(!AddUserToDept(user.Name, null, path, false))
                        {
                            RemoveUser(user.Name);
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        RemoveUser(user.Name);
                        return false;
                    }
                }
                else
                {
                    RemoveUser(user.Name);
                    return false;
                }
            }
        }
        
    }
}
