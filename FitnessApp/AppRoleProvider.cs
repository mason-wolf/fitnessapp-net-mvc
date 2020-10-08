using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace FitnessApp
{
    public class AppRoleProvider : System.Web.Security.RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return new string[] { "User" };
        }

        public override string[] GetRolesForUser(string username)
        {
            FitnessAppDb db = new FitnessAppDb();

            string userRole = (from userProfile in db.UserProfiles where userProfile.Username == username select userProfile.Role).SingleOrDefault();

            if (userRole == null)
            {
                return new string[0];
            }

            if (userRole == "User")
            {
                return new string[] { userRole };
            }

            return new string[0];
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] roles = GetRolesForUser(username);
            Debug.WriteLine("Role name: " + roleName + "User Role: " + roles[0]);          
            foreach (string role in roles)
            {
                if (roleName.Equals(role, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Role true");
                    return true;
                }
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}