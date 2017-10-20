using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.RTX.WebApi
{
    public class AdOperate
    {
        private string Path { get; set; }
        private string OUPath { get; set; }
        private string AdminUser { get; set; }
        private string Password { get; set; }
        private ADConfig ADC { get; set; }

        public AdOperate(){ }

        public AdOperate(string filePath)
        {
            ADC = Helper.DeserializeFromXML<ADConfig>(filePath);
            if (ADC != null)
            {
                this.Path = ADC.DoMainPath;
                this.OUPath = ADC.DoMainOUPath;
                this.AdminUser = ADC.AdminUser;
                this.Password = ADC.Password;
            }
        }

        public bool SetADConfig(string comName,string filePath)
        {
            if(ADC.DoMainOUPath != ADC.DoMainPath1 + "OU=" + comName + "," + ADC.DoMainPath3)
            {
                ADC.DoMainOUPath = ADC.DoMainPath1 + "OU=" + comName + "," + ADC.DoMainPath3;
                return Helper.SerializeToXml(ADC, filePath);
            }
            return true;
        }

        public void GetADConfig(string filePath,string comName)
        {
            ADConfig adc = Helper.DeserializeFromXML<ADConfig>(filePath);
            if (!string.IsNullOrEmpty(comName))
            {
                adc.DoMainPath2 = "OU=" + comName + ",";
                adc.SetDoMainOUPath();
            }              
            if(adc!=null)
            {
                this.Path = adc.DoMainPath;
                this.OUPath = adc.DoMainOUPath; 
                this.AdminUser = adc.AdminUser;
                this.Password = adc.Password;
            }            
        }

        public DirectoryEntry GetEntry()
        {
            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path = OUPath;
                domain.Username = AdminUser;
                domain.Password = Password;
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return domain;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DirectoryEntry GetRootEntry()
        {
            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path =Path;
                domain.Username = AdminUser;
                domain.Password = Password;
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return domain;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DirectoryEntry GetEntry(string path, string adminUser, string password)
        {
            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path = path;
                domain.Username = adminUser;
                domain.Password = password;
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return domain;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CheckADUser(string domainPath, string adminUser, string password)
        {
            try
            {
                DirectoryEntry domain = new DirectoryEntry(domainPath, adminUser, password);
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsADUserExist(DirectoryEntry entry, string userName)
        {
            using (DirectorySearcher search = new DirectorySearcher(entry))
            {
                try
                {
                    search.Filter = "(sAMAccountName=" + userName + ")";
                    //search.PropertiesToLoad.Add("cn"); //不指定加载查询属性，不会把属性查出来
                    SearchResult result= search.FindOne();
                    if(result==null)
                    {
                        return false;
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public DirectoryEntry GetUserEntry(DirectoryEntry entry,string userName)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(entry, "(&(objectCategory=person)(objectClass=user)(cn=" + userName + "))"))
            {
                try
                {
                    SearchResult src = objSearcher.FindOne();
                    return src.GetDirectoryEntry();
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }

        public DirectoryEntry GetUserEntry(string doMainPath, string adminUser, string userName, string password)
        {
            DirectoryEntry userPath = GetEntry(doMainPath, adminUser, password);
            using (DirectorySearcher objSearcher = new DirectorySearcher(userPath, "(&(objectCategory=person)(objectClass=user)(cn=" + userName + "))"))
            {
                try
                {
                    SearchResult src = objSearcher.FindOne();
                    return src.GetDirectoryEntry();
                }
                catch(Exception ex)
                {
                    return null;
                }                
            }
        }

        public DirectoryEntry GetOUEntry(DirectoryEntry ouEntry, string filter)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(ouEntry, filter))
            {
                SearchResult src = objSearcher.FindOne();
                if (src != null)
                {
                    return src.GetDirectoryEntry();
                }
                return null;
            }
        }
        /*
        public void OUEntrySyncRtx(DirectoryEntry entry, string filter, RtxDeptManager dept)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(entry, filter))
            {
                objSearcher.PropertiesToLoad.Add("distinguishedName");
                SearchResultCollection srcs = objSearcher.FindAll();
                foreach (SearchResult src in srcs)
                {
                    string[] strs = src.Properties["distinguishedName"][0].ToString().Split(',');
                    if (!strs[1].Contains("OU="))
                    {
                        dept.AddDept(strs[0].Replace("OU=", ""), "");
                    }
                    else
                    {
                        dept.RemoveDept(strs[0].Replace("OU=", ""));
                        dept.AddDept(strs[0].Replace("OU=", ""), strs[1].Replace("OU=", ""));
                    }
                }
            }
        }
        
        public void UserEntrySyncRtx(DirectoryEntry entry,string filter,RtxUserManager user,RtxDeptManager dept)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(entry, filter))
            {
                objSearcher.PropertiesToLoad.Add("distinguishedName");
                objSearcher.PropertiesToLoad.Add("name");
                objSearcher.PropertiesToLoad.Add("displayName");
                objSearcher.PropertiesToLoad.Add("mail");
                objSearcher.PropertiesToLoad.Add("telephoneNumber");
                objSearcher.PropertiesToLoad.Add("initials");
                SearchResultCollection srcs = objSearcher.FindAll();
                foreach (SearchResult src in srcs)
                {
                    string[] paths = src.Properties["distinguishedName"][0].ToString().Replace("DC=", "").Replace("OU=", "").Replace("CN=", "").Split(',');
                    string deptName = paths[1];
                    StringBuilder builder = new StringBuilder();
                    for (int i = paths.Length - 1; i > 0; i--)
                    {
                        builder.Append(paths[i]).Append(@"\");
                    }
                    string path = builder.ToString().Replace(@"com\test\", "");
                    string userName = src.Properties["name"][0].ToString();
                    string displayName = "RTX_NULL";
                    int gender = -1;
                    string mobile = "RTX_NULL";
                    string email = "RTX_NULL";
                    string phone = "RTX_NULL";
                    if (src.Properties["displayName"].Count == 1)
                    {
                        displayName = src.Properties["displayName"][0].ToString();
                    }
                    if (src.Properties["mail"].Count == 1)
                    {
                        email = src.Properties["mail"][0].ToString();
                    }
                    if (src.Properties["telephoneNumber"].Count == 1)
                    {
                        mobile = src.Properties["telephoneNumber"][0].ToString();
                        phone = mobile;
                    }
                    if(src.Properties["initials"].Count==1)
                    {
                        gender = Convert.ToInt32(src.Properties["initials"][0]);
                    }
                    if(user.IsUserExist(userName))
                    {
                        user.SetBasicRtxUser(userName, displayName, gender, phone, email, phone, 1);
                        dept.AddUserToDept(userName, dept.GetUserDeptsName(userName), path, false);
                    }
                    else
                    {
                        user.AddRtxUser(userName, 1);
                        user.SetBasicRtxUser(userName, displayName, gender, mobile, email, phone, 1);
                        dept.AddUserToDept(userName, null, path, false);
                    }
                }
            }
        }*/

        public bool AddAccount(DirectoryEntry entry, DomainUser user)
        {
            try
            {
                DirectoryEntry NewUser = entry.Children.Add("CN=" + user.Name, "user");
                NewUser.Properties["sAMAccountName"].Add(user.Name); //account
                NewUser.Properties["userPrincipalName"].Value = user.Name+"@test.com"; //user logon name,xxx@bdxy.com
                if (!string.IsNullOrEmpty(user.Company))
                {
                    NewUser.Properties["company"].Value = user.Company;
                }
                if (!string.IsNullOrEmpty(user.Department))
                {
                    NewUser.Properties["department"].Value = user.Department;
                }
                if (!string.IsNullOrEmpty(user.Description))
                {
                    NewUser.Properties["description"].Value = user.Description;
                }
                if (!string.IsNullOrEmpty(user.DisplayName))
                {
                    NewUser.Properties["displayName"].Value = user.DisplayName;
                }
                if (!string.IsNullOrEmpty(user.GivenName))
                {
                    NewUser.Properties["givenName"].Value = user.GivenName;
                }
                if (!string.IsNullOrEmpty(user.Initials))
                {
                    NewUser.Properties["initials"].Value = user.Initials;
                }
                if (!string.IsNullOrEmpty(user.Mail))
                {
                    NewUser.Properties["mail"].Value = user.Mail;
                }
                if (!string.IsNullOrEmpty(user.Name))
                {
                    NewUser.Properties["name"].Value = user.Name;
                }
                if (!string.IsNullOrEmpty(user.PhysicalDeliveryOfficeName))
                {
                    NewUser.Properties["physicalDeliveryOfficeName"].Value = user.PhysicalDeliveryOfficeName;
                }
                if (!string.IsNullOrEmpty(user.SN))
                {
                    NewUser.Properties["sn"].Value = user.SN;
                }
                if (!string.IsNullOrEmpty(user.TelephoneNumber))
                {
                    NewUser.Properties["telephoneNumber"].Value = user.TelephoneNumber;
                }
                NewUser.Properties["initials"].Value = user.Gender;
                NewUser.CommitChanges();
                //设置密码
                //反射调用修改密码的方法（注意端口号的问题  端口号会引起方法调用异常）
                if (!string.IsNullOrEmpty(user.UserPwd))
                {
                    NewUser.Invoke("SetPassword", new object[] { user.UserPwd });
                    NewUser.Properties["userAccountControl"].Value = 0x200;
                    //默认设置新增账户启用          
                    NewUser.CommitChanges();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EditAccount(DirectoryEntry userEntry, DomainUser user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Company))
                {
                    userEntry.Properties["company"].Value = user.Company;
                }
                if (!string.IsNullOrEmpty(user.Department))
                {
                    userEntry.Properties["department"].Value = user.Department;
                }
                if (!string.IsNullOrEmpty(user.Description))
                {
                    userEntry.Properties["description"].Value = user.Description;
                }
                if (!string.IsNullOrEmpty(user.DisplayName))
                {
                    userEntry.Properties["displayName"].Value = user.DisplayName;
                }
                if (!string.IsNullOrEmpty(user.GivenName))
                {
                    userEntry.Properties["givenName"].Value = user.GivenName;
                }
                //if (!string.IsNullOrEmpty(user.Initials))
                //{
                //    userEntry.Properties["initials"].Value = user.Initials;
                //}
                if (!string.IsNullOrEmpty(user.Mail))
                {
                    userEntry.Properties["mail"].Value = user.Mail;
                }
                if (!string.IsNullOrEmpty(user.PhysicalDeliveryOfficeName))
                {
                    userEntry.Properties["physicalDeliveryOfficeName"].Value = user.PhysicalDeliveryOfficeName;
                }
                if (!string.IsNullOrEmpty(user.SN))
                {
                    userEntry.Properties["sn"].Value = user.SN;
                }
                if (!string.IsNullOrEmpty(user.TelephoneNumber))
                {
                    userEntry.Properties["telephoneNumber"].Value = user.TelephoneNumber;
                }
                userEntry.Properties["initials"].Value = user.Gender;
                userEntry.CommitChanges();
                //设置密码
                //反射调用修改密码的方法（注意端口号的问题  端口号会引起方法调用异常）
                //if (!string.IsNullOrEmpty(user.UserPwd))
                //{
                //    userEntry.Invoke("SetPassword", new object[] { user.UserPwd });
                //    userEntry.CommitChanges();
                //    return false;
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddOUEntry(DirectoryEntry entry, string ouName)
        {
            try
            {
                DirectoryEntry newOUEntry = entry.Children.Add("OU=" + ouName, "organizationalUnit");
                newOUEntry.Properties["description"].Value = ouName;
                newOUEntry.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UserMoveToOU(DirectoryEntry moveToEntry, DirectoryEntry userEntry)
        {
            try
            {
                userEntry.MoveTo(moveToEntry);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool OUEntryReName(DirectoryEntry entry,string newName)
        {
            try
            {
                entry.Rename("OU=" + newName);
                entry.Properties["description"].Value = newName;
                entry.CommitChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DelEntry(DirectoryEntry entry)
        {
            try
            {
                entry.DeleteTree();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        ///
    }
}
