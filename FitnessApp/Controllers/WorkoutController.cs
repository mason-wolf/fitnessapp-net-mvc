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
                return Json(new { newUrl = Url.Action("Add", "Workout") });
            }
            else
            {
                newWorkout.Title = workoutTitle;
                newWorkout.Exercises = exercises;

                using (FitnessAppDbContext db = new FitnessAppDbContext())
                {
                    db.Workouts.Add(newWorkout);
                    db.SaveChanges();
                }

                return Json(new { newUrl = Url.Action("View", "Workout") });
            }
        }

        [UserAuthorization(AccessLevel = "User")]
        public ActionResult Edit(int workoutId)
        {
            UserProfile userProfile = GetUserProfile();
            List<Exercise> exerciseList = new List<Exercise>();

            using (FitnessAppDbContext Db = new FitnessAppDbContext())
            {
                var userWorkout = Db.Workouts.Where(workout => workout.WorkoutId == workoutId && workout.UserId == userProfile.UserId).FirstOrDefault();

                if (userWorkout.Exercises != null)
                {
                    ViewBag.WorkoutTitle = userWorkout.Title;
                    ViewBag.WorkoutId = workoutId;
                    return View(userWorkout.Exercises);
                }
                else
                {
                    return RedirectToAction("Dashboard");
                }
            }
        }

        [UserAuthorization(AccessLevel = "User")]
        [HttpPost]
        public ActionResult Edit(List<Exercise> exercises, string workoutTitle)
        {
            int workoutId = 0;
            userProfile = GetUserProfile();

            if (workoutTitle == "")
            {
                TempData["formError"] = "Please enter a title for your workout.";
                return Json(new { newUrl = Url.Action("View", "Workout") });
            }
            else
            {
                using (FitnessAppDbContext Db = new FitnessAppDbContext())
                {
                    var userWorkout = Db.Workouts.Where(workout => workout.Title == workoutTitle && workout.UserId == userProfile.UserId).FirstOrDefault();
                    userWorkout.Exercises.Clear();
                    userWorkout.Exercises = exercises;
                    userWorkout.Title = workoutTitle;
                    workoutId = userWorkout.WorkoutId;
                    Db.SaveChanges();
                }

                return Json(new { newUrl = Url.Action("Edit", "Workout", new { @workoutId = workoutId } ) });
            }
        }

        [UserAuthorization(AccessLevel = "User")]
        [HttpPost]
        public ActionResult Delete(int workoutId)
        {
            userProfile = GetUserProfile();

            using (FitnessAppDbContext db = new FitnessAppDbContext())
            {
                var workoutToDelete = db.Workouts.Where(workout => workout.WorkoutId == workoutId && workout.UserId == userProfile.UserId).FirstOrDefault();
                db.Workouts.Attach(workoutToDelete);
                db.Workouts.Remove(workoutToDelete);
                db.SaveChanges();
            }

            return RedirectToAction("View", "Workout");
        }
        /// <summary>
        /// Retrieve the user's profile based on the current session.
        /// </summary>
        /// <returns>UserProfile</returns>
        public UserProfile GetUserProfile()
        {
            UserProfile userProfile;

            using (FitnessAppDbContext db = new FitnessAppDbContext())
            {
                string username = Session["username"].ToString().ToLower();
                userProfile = db.UserProfiles.Where(user => user.Username == username).FirstOrDefault();
            }

            return userProfile;
        }
    }
}