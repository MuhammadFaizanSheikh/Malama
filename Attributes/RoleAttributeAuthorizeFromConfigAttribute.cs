using Malama.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Malama.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAttributeAuthorizeFromConfigAttribute : TypeFilterAttribute
    {
        public RoleAttributeAuthorizeFromConfigAttribute(string featureName)
            : base(typeof(RoleAttributeAuthorizeFromConfigFilter))
        {
            Arguments = new object[] { featureName };
        }
    }

    public class RoleAttributeAuthorizeFromConfigFilter : IAuthorizationFilter
    {
        private readonly string _featureName;

        public RoleAttributeAuthorizeFromConfigFilter(string featureName)
        {
            _featureName = featureName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
            var requirement = RoleAttributeRequirementProvider.GetRequirement(_featureName);

            var task = authService.AuthorizeAsync(context.HttpContext.User, null, requirement);
            task.Wait();

            if (!task.Result.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
