using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackMyBills.Models;

namespace TrackMyBills.ActionFilters
{
    public class CheckLoggedInAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["LoggedInUser"] != null)
            {
                var user = filterContext.HttpContext.Session["LoggedInUser"] as UserSecurityModel;
                if (user != null && !string.IsNullOrEmpty(user.UserKey))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }

            var route = new System.Web.Routing.RouteValueDictionary();
            route.Add("controller", "Account");
            route.Add("action", "Login");
            route.Add("userKey", "");
            filterContext.Result = new RedirectToRouteResult(route);
            
        } 
    }
}