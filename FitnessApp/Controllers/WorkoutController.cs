 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnessApp.Controllers
{
    public class WorkoutController : Controller
    {
        private UserProfile userProfile;

        [UserAuthorization(AccessLevel = "User")]
        public new ActionResult View()
        {
            UserProfile userProfile;

            // Retrieve the user and their workouts based on current session.
            using (FitnessAppDb db = new FitnessAppDb())
            {
                string username = Session["Username"].ToString().ToLower();
                userProfile = db.UserProfiles.Where(user => user.Username == username).FirstOrDefault();
                var workouts = db.Workouts.Where(workout => workout.UserId == userProfile.UserId).ToList();
                userProfile.Workouts = workouts;

                if (userProfile != null)
                {
                    if (userProfile.Workouts == null)
                    {
                        ViewBag.NoWorkouts = "No workouts planned.";
                    }
                    return View(userProfile);
                }
                else
                {
                    // If the session invalid for the user, redirect to login.
                    return RedirectToAction("Login");
                }
            }
        }

        [UserAuthorization(AccessLevel = "User")]
        public ActionResult Add()
        {
            ViewBag.ErrorMessage = TempData["formError"];
            return View(new List<Exercise>());
        }

        [HttpPost]
        [UserAuthorization(AccessLevel = "User")]
        public ActionResult Add(List<Exercise> exercises, string workoutTitle)
        {
            Workout newWorkout = new Workout();
            userProfile = GetUserProfile();
            newWorkout.UserId = userProfile.UserId;
            newWorkout.Date = DateTime.Today;

            if (workoutTitle == "")
            {
                TempData["formError"] = "Please enter a title for your workout.";
            }
            else
            {
                newWorkout.Title = workoutTitle;
                newWorkout.Exercises = exercises;

                using (FitnessAppDb db = new FitnessAppDb())
                {
                    db.Workouts.Add(newWorkout);
                    db.SaveChanges();
                }
            }
            return Json(new { newUrl = Url.Action("Add", "Workout") });
        }

        /// <summary>
        /// Retrieve the user's profile based on the current session.
        /// </summary>
        /// <returns>UserProfile</returns>
        public UserProfile GetUserProfile()
        {
            UserProfile userProfile;

            using (FitnessAppDb db = new FitnessAppDb())
            {
                string username = Session["username"].ToString().ToLower();
                userProfile = db.UserProfiles.Where(user => user.Username == username).FirstOrDefault();
            }

            return userProfile;
        }
    }
}