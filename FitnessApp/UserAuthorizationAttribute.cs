using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FitnessApp
{
    public class UserAuthorizationAttribute : AuthorizeAttribute
    {
        public string Keys
        {
            get { return Keys; }
            set
            {
                Roles = value;
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result != null)
            {
                if (filterContext.Result.GetType() == typeof(HttpUnauthorizedResult))
                {
              //      filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                }
            }
        }
    }
}