using ExcelFilesCompiler.Utilities;

namespace Malama.Middleware
{
    public class TreatmentConsentSmModeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TreatmentConsentSmModeMiddleware> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentSmModeMiddleware);

        public TreatmentConsentSmModeMiddleware(
            RequestDelegate next,
            ILogger<TreatmentConsentSmModeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string methodName = nameof(InvokeAsync);

            try
            {
                if (context.Session != null
                    && TreatmentConsentSmModeHelper.IsActive(context.Session))
                {
                    var lockedSmId = TreatmentConsentSmModeHelper.GetLockedServiceMembersChildId(context.Session);
                    if (lockedSmId == null)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, SM mode active but ServiceMembersChildId missing. Clearing session.",
                            CLASSNAME, methodName);
                        TreatmentConsentSmModeHelper.Clear(context.Session);
                    }
                    else if (!TreatmentConsentSmModeHelper.IsAllowedPath(
                        context.Request.Path,
                        lockedSmId.Value,
                        context.Request.QueryString))
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Blocked path while SM mode active. Path={Path}, LockedSmId={LockedSmId}",
                            CLASSNAME, methodName, context.Request.Path, lockedSmId.Value);

                        if (IsApiOrAjaxRequest(context.Request))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(
                                "{\"success\":false,\"message\":\"Service member mode is active. Staff unlock is required.\"}");
                            return;
                        }

                        var redirectUrl =
                            $"/TreatmentConsent/TreatmentConsentStation?serviceMembersChildId={lockedSmId.Value}";
                        context.Response.Redirect(redirectUrl);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while enforcing Treatment Consent SM mode",
                    CLASSNAME, methodName);
            }

            await _next(context);
        }

        private static bool IsApiOrAjaxRequest(HttpRequest request)
        {
            if (string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var accept = request.Headers.Accept.ToString();
            return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
