using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

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
                using (FitnessAppDb db = new FitnessAppDb())
                {
                    var user = db.UserProfiles.Where(userObj => userObj.Username.Equals(userProfile.Username) && userObj.Password.Equals(userProfile.Password)).FirstOrDefault();

                    if (user != null)
                    {
                        Session["UserId"] = userProfile.UserId.ToString();
                        Session["Username"] = userProfile.Username.ToString();
                        return RedirectToAction("Dashboard", new { id = user.UserId });
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
                using (FitnessAppDb db = new FitnessAppDb())
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
                            db.UserProfiles.Add(userProfile);
                            db.SaveChanges();
                            var newUser = db.UserProfiles.Where(userObj => userObj.Username.Equals(userProfile.Username)).FirstOrDefault();
                            Debug.WriteLine(newUser.UserId);
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

        public ActionResult Dashboard(int id)
        {
            if (Session["Username"] != null)
            {
                List<Workout> workoutList = new List<Workout>();

                using (FitnessAppDb db = new FitnessAppDb())
                {
                    var workouts = db.Workouts.Where(workout => workout.WorkoutId == id).FirstOrDefault();
                    workoutList.Add(workouts);
                }

                UserProfile userProfile = new UserProfile();
                userProfile.UserId = id;

                if (workoutList[0] == null)
                {
                    ViewBag.NoWorkouts = "No workouts yet!";
                }
                else
                {
                    userProfile.Workouts = workoutList;
                }
                return View(userProfile);
            }
            else
            {
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
                return false;
            }
            catch (ArgumentException e)
            {
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