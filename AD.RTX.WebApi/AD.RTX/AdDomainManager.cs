using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.RTX.WebApi
{
    public class AdDomainManager
    {
        private static AdOperate oad = new AdOperate();
        private static string path = "LDAP://192.168.31.134/OU=某单位,DC=test,DC=com";
        private static string adminUser = "Administrator";
        private static string password = "Abc123456";
        private static DirectoryEntry entry;

        public static bool AddUser(DomainUser user)
        {
            entry = oad.GetEntry(path, adminUser, password);
            if(oad.GetUserEntry(entry,user.Name)!=null)
            {
                return false;
            }
            string filter = "(&(objectclass=organizationalUnit)(ou=" + user.Department + "))";
            DirectoryEntry ouEntry = oad.GetOUEntry(entry, filter);
            if(ouEntry==null)
            {
                return false;
            }
            if(!oad.AddAccount(ouEntry, user))
            {
                return false;
            }
            return true;
        }
    }
}
