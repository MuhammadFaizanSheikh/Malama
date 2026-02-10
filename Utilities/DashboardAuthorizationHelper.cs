using System.Security.Claims;

namespace Malama.Utilities
{
    public static class DashboardAuthorizationHelper
    {
        public static bool HasAccess(ClaimsPrincipal user, string permissionKey)
        {
            if (!RoleAttributeConfig.RoleAttributeCombinations.TryGetValue(permissionKey, out var allowedList))
                return false;

            foreach (var (role, attribute) in allowedList)
            {
                // Case 1: Attribute-only access (role == null)
                if (role is null && attribute is not null)
                {
                    if (user.HasClaim("Attribute", attribute))
                        return true;
                }
                // Case 2: Role-only access (attribute == null)
                else if (role is not null && attribute is null)
                {
                    if (user.IsInRole(role))
                        return true;
                }
                // Case 3: Role + Attribute must match
                else if (role is not null && attribute is not null)
                {
                    if (user.IsInRole(role) && user.HasClaim("Attribute", attribute))
                        return true;
                }
            }

            return false;
        }

        public static bool CheckCheckInOutAccess(ClaimsPrincipal user, string userType)
        {
            if (userType == "admin")
            {
                return HasAccess(user, "ScrubbedSheetUploader_View");
            }
            else if (userType == "client")
            {
                return HasAccess(user, "CheckInOutStaff_View");
            }
            else
            {
                return false;
            }
        }

        //This method is used to show rights of user on Account User List page
        //public static List<(string PageName, bool CanView, bool CanSave)> GetAccessByRoles(IList<string> userRoles)
        //{
        //    var accessList = new List<(string PageName, bool CanView, bool CanSave)>();

        //    // Get all unique pages
        //    var allPages = RoleAttributeConfig.RoleAttributeCombinations.Keys
        //        .Select(k => k.Substring(0, k.LastIndexOf("_"))) // remove _View/_Save
        //        .Distinct();

        //    foreach (var page in allPages)
        //    {
        //        bool canView = HasAccess(userRoles, page + "_View");
        //        bool canSave = HasAccess(userRoles, page + "_Save");

        //        accessList.Add((page, canView, canSave));
        //    }

        //    return accessList;
        //}

        //private static bool HasAccess(IList<string> userRoles, string permissionKey)
        //{
        //    if (!RoleAttributeConfig.RoleAttributeCombinations.TryGetValue(permissionKey, out var allowedList))
        //        return false;

        //    // If any role of user matches allowed role, return true
        //    return allowedList.Any(r => r.Role != null && userRoles.Contains(r.Role));
        //}
    }

}
