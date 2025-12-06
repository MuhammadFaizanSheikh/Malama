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
                return HasAccess(user, "CheckInOutStaff_Admin_View");
            }
            else if (userType == "client")
            {
                return HasAccess(user, "CheckInOutStaff_Client_View");
            }
            else
            {
                return false;
            }
        }


    }

}
