using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Web.Security;

namespace TrackMyBills.Services
{
    public class AccountService : IAccountService
    {
        public bool Login(string userKey)
        {
            if (userKey == "35E8EA3880B597A70D7BDA41ABDCA757")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ComputeUserKey(string username, string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(username + "saltyhash" + password, "md5");
            //var bytes = Encoding.Default.GetBytes(username + "saltyhash" + password);
            //var userKey = MD5.Create().ComputeHash(bytes);
            //return Encoding.Default.GetString(userKey);
        }
    }
}