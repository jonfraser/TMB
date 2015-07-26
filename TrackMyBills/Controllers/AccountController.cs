using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackMyBills.Services;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using TrackMyBills.Models;

namespace TrackMyBills.Controllers
{
    public class AccountController : Controller
    {
        IAccountService _accountService = null;
        public AccountController(IAccountService accountService) : base()
        {
            this._accountService = accountService;
        }

        public AccountController() : base()
        {
            this._accountService = new AccountService();
        }

        public ActionResult Account()
        {
            return Login("","");
        }

        //public ActionResult Login()
        //{
        //    return View("Login");
        //}

        public ActionResult LoggedIn()
        {
            return View();
        }

        //public ActionResult Login(string userKey)
        //{
        //    if (!string.IsNullOrEmpty(userKey))
        //    {
        //        if (this._accountService.Login(userKey))
        //        {
        //            return RedirectToAction("Dashboard", "Dashboard");
        //        }
        //        else
        //        {
        //            return View("Login");
        //        }
        //    }
        //    return View();
        //}

        public ActionResult AutoLogin(string id)
        {
            if (Session["AvoidLogin"] != null && (bool)Session["AvoidLogin"])
            {
                Session["AvoidLogin"] = null;
                return View();
            }
            else
            {
                return RedirectToAction("Login", new { usernameOrKey = id });
            }
        }

        public ActionResult Login(string usernameOrKey, string password)
        {
            if (string.IsNullOrEmpty(usernameOrKey) && string.IsNullOrEmpty(password))
            {
                return View();
            }

            if (!string.IsNullOrEmpty(usernameOrKey) && string.IsNullOrEmpty(password))
            {
                if (this._accountService.Login(usernameOrKey))
                {
                    Session["LoggedInUser"] = new UserSecurityModel { UserKey = usernameOrKey };
                    return RedirectToAction("Bill", "Dashboard");
                }
                else
                {
                    return View("Login");
                }
            }
            //var bytes = Encoding.UTF8.GetBytes(validUsername + "saltyhash" + validPassword);
            //var userKey = MD5.Create().ComputeHash(bytes);
            //Encoding.UTF8.GetString(userKey);

            return Login(this._accountService.ComputeUserKey(usernameOrKey, password), string.Empty);
        }

        public ActionResult Logoff()
        {
            Session["LoggedInUser"] = null;
            return RedirectToAction("Login");
        }
    }
}
