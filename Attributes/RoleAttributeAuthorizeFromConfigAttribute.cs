using Malama.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Malama.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAttributeAuthorizeFromConfig : Attribute, IAuthorizationFilter
    {
        private readonly string[] _featureNames;

        public RoleAttributeAuthorizeFromConfig(params string[] featureNames)
        {
            _featureNames = featureNames;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices
                .GetRequiredService<IAuthorizationService>();

            foreach (var feature in _featureNames)
            {
                var requirement = RoleAttributeRequirementProvider.GetRequirement(feature);
                var result = authService.AuthorizeAsync(
                    context.HttpContext.User, null, requirement).Result;

                if (result.Succeeded)
                    return; // ✅ OR logic
            }

            context.Result = new ForbidResult();
        }
    }

}
