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
        public string AccessLevel { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorized = false;

            // Check if a session has been established, query the database for that user's role and return true if authorized.
            if (httpContext.Session["Username"] != null)
            {
                string username = httpContext.Session["Username"].ToString();

                if (!string.IsNullOrEmpty(username))
                {
                    using (FitnessAppDbContext db = new FitnessAppDbContext())
                    {
                        var userRole = (from users in db.UserProfiles where users.Username == username select new { users.Role }).FirstOrDefault();

                        if (userRole.Role == "User")
                        {
                            return true;
                        }
                    }
                }
            }
            return authorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result != null)
            {
                // Redirect to login if unauthorized.
                if (filterContext.Result.GetType() == typeof(HttpUnauthorizedResult))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                }
            }
        }
    }
}