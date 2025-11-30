using Microsoft.AspNetCore.Authorization;

namespace Malama.Authorization
{
    public class RoleAttributeRequirement : IAuthorizationRequirement
    {
        public (string Role, string Attribute)[] RoleAttributePairs { get; }

        public RoleAttributeRequirement((string Role, string Attribute)[] roleAttributePairs)
        {
            RoleAttributePairs = roleAttributePairs;
        }
    }
}
