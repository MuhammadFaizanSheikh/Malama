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
                if (user.IsInRole(role))
                {
                    if (attribute is null)
                        return true;

                    if (user.HasClaim("Attribute", attribute))
                        return true;
                }
            }

            return false;
        }
    }

}
