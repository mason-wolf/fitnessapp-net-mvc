 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnessApp.Controllers
{
    public class WorkoutController : Controller
    {
        [UserAuthorization(AccessLevel = "User")]
        public ActionResult View(int id)
        {
            UserProfile userProfile = GetUserProfile(id);
            List<Workout> workoutList = new List<Workout>();

            if (IdOwnedByUser(id))
            {
                // Retrieve the user and their workouts based on ID.
                using (FitnessAppDb db = new FitnessAppDb())
                {
                    var workouts = db.Workouts.Where(workout => workout.WorkoutId == id).FirstOrDefault();
                    workoutList.Add(workouts);
                }

                if (workoutList[0] == null)
                {
                    ViewBag.NoWorkouts = "No workouts planned.";
                }
                else
                {
                    userProfile.Workouts = workoutList;
                }

                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [UserAuthorization(AccessLevel = "User")]

        public ActionResult Add(int id)
        {
            if (IdOwnedByUser(id))
            {
                return View(GetUserProfile(id));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        /// <summary>
        /// Retreives the user profile of the requested ID and checks if the session owner and user ID match the requested ID.
        /// </summary>
        /// <param name="userId">UserId URL Parameter</param>
        /// <returns>ownedByUser (true/false)</returns>
        public bool IdOwnedByUser(int userId)
        {
            bool ownedByUser = false;

            UserProfile userProfile = GetUserProfile(userId);

            if (userProfile != null && (string)Session["Username"] == userProfile.Username)
            {
                ownedByUser = true;
            }

            return ownedByUser;
        }

        public UserProfile GetUserProfile(int userId)
        {
            UserProfile userProfile;

            using (FitnessAppDb db = new FitnessAppDb())
            {
                userProfile = db.UserProfiles.Where(user => user.UserId == userId).FirstOrDefault();
            }

            return userProfile;
        }
    }
}