using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FitnessApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                using (FitnessAppDbContext db = new FitnessAppDbContext())
                {
                    var user = db.UserProfiles.Where(userObj => userObj.Username.Equals(userProfile.Username) && userObj.Password.Equals(userProfile.Password)).FirstOrDefault();

                    if (user != null)
                    {
                        Session["UserId"] = userProfile.UserId.ToString();
                        Session["Username"] = userProfile.Username.ToString();
                        FormsAuthentication.SetAuthCookie(userProfile.Username, false);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        if (!IsValidEmail(userProfile.Username))
                        {
                            ViewBag.Message = "Please enter a valid email.";
                        }
                        ViewBag.Message = "Incorrect username or password.";
                    }
                }
            }
            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                using (FitnessAppDbContext db = new FitnessAppDbContext())
                {
                    var user = db.UserProfiles.Where(userObj => userObj.Username.Equals(userProfile.Username) && userObj.Password.Equals(userProfile.Password)).FirstOrDefault();

                    if (!IsValidEmail(userProfile.Username))
                    {
                        ViewBag.Message = "Please enter a valid email.";
                    }
                    else
                    {
                        if (user == null && !string.IsNullOrEmpty(userProfile.Username) && !string.IsNullOrEmpty(userProfile.Password))
                        {
                            Session["Username"] = userProfile.Username.ToString();
                            userProfile.Role = "User";
                            userProfile.Username = userProfile.Username.ToLower();
                            db.UserProfiles.Add(userProfile);
                            db.SaveChanges();
                            var newUser = db.UserProfiles.Where(userObj => userObj.Username.Equals(userProfile.Username)).FirstOrDefault();
                            return RedirectToAction("Dashboard", new { id = newUser.UserId });
                        }
                        else if (user != null)
                        {
                            ViewBag.Message = "Account already exists.";
                        }
                        else
                        {
                            ViewBag.Message = "Please enter a valid email and password.";
                        }
                    }
                }
            }
            return View(userProfile);
        }

       [UserAuthorization(AccessLevel = "User")]
        public ActionResult Dashboard()
        {
            if (Session["Username"] != null && ModelState.IsValid)
            {
                UserProfile userProfile;

                // Retrieve the user and their workouts based on current session.
                using (FitnessAppDbContext db = new FitnessAppDbContext())
                {
                    string username = Session["Username"].ToString().ToLower();
                    userProfile = db.UserProfiles.Where(user => user.Username == username).FirstOrDefault();
                    var workouts = db.Workouts.Where(workout => workout.UserId == userProfile.UserId).ToList();
                    userProfile.Workouts = workouts;

                    if (userProfile != null)
                    {
                        if (userProfile.Workouts == null)
                        {
                            ViewBag.NoWorkouts = "No workouts on record yet!";
                        }
                        return View(userProfile);
                    }
                    else
                    {
                        // If the id is invalid for the user, redirect to login.
                        return RedirectToAction("Login");
                    }
                }
            }
            else
            {
                // Redirect to login if the session is invalid.
                return RedirectToAction("Login");
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                Debug.WriteLine(e);
                return false;
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}