using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackMyBills.Services
{
    public interface IAccountService
    {
        bool Login(string userKey);

        string ComputeUserKey(string username, string password);
    }
}