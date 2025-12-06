using Malama.Utilities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Malama.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckInOutAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Get the userType (from route or query string)
            var userType = context.RouteData.Values["userType"]?.ToString()
                            ?? context.HttpContext.Request.Query["userType"].ToString();

            if (string.IsNullOrEmpty(userType))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            // Your existing centralized access logic
            bool isAuthorized = DashboardAuthorizationHelper.CheckCheckInOutAccess(user, userType);

            if (!isAuthorized)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
