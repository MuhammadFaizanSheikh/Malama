using Microsoft.AspNetCore.Authorization;

namespace Malama.Authorization
{
    public class RoleAttributeHandler : AuthorizationHandler<RoleAttributeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleAttributeRequirement requirement)
        {
            foreach (var pair in requirement.RoleAttributePairs)
            {
                bool hasRole = context.User.IsInRole(pair.Role);
                bool hasAttribute = string.IsNullOrEmpty(pair.Attribute) || context.User.HasClaim("Attribute", pair.Attribute);

                if (hasRole && hasAttribute)
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
