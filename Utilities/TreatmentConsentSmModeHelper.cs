using Microsoft.AspNetCore.Http;

namespace ExcelFilesCompiler.Utilities
{
    public static class TreatmentConsentSmModeHelper
    {
        public const string SessionActiveKey = "TreatmentConsent.SmMode.Active";
        public const string SessionServiceMemberIdKey = "TreatmentConsent.SmMode.ServiceMembersChildId";
        public const string SessionStartedByUserIdKey = "TreatmentConsent.SmMode.StartedByUserId";
        public const string SessionStartedAtKey = "TreatmentConsent.SmMode.StartedAtUtc";

        public static bool IsActive(ISession session)
        {
            return string.Equals(session.GetString(SessionActiveKey), "1", StringComparison.Ordinal);
        }

        public static long? GetLockedServiceMembersChildId(ISession session)
        {
            var raw = session.GetString(SessionServiceMemberIdKey);
            if (long.TryParse(raw, out var id) && id > 0)
            {
                return id;
            }

            return null;
        }

        public static void Start(ISession session, long serviceMembersChildId, string userId)
        {
            session.SetString(SessionActiveKey, "1");
            session.SetString(SessionServiceMemberIdKey, serviceMembersChildId.ToString());
            session.SetString(SessionStartedByUserIdKey, userId ?? string.Empty);
            session.SetString(SessionStartedAtKey, DateTime.UtcNow.ToString("O"));
        }

        public static void Clear(ISession session)
        {
            session.Remove(SessionActiveKey);
            session.Remove(SessionServiceMemberIdKey);
            session.Remove(SessionStartedByUserIdKey);
            session.Remove(SessionStartedAtKey);
        }

        public static bool IsAllowedPath(PathString path, long lockedServiceMembersChildId, QueryString query)
        {
            var value = (path.Value ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrEmpty(value))
            {
                value = "/";
            }

            // Unlock / status endpoints must remain reachable while locked.
            if (IsExactPath(value, "/TreatmentConsent/UnlockSmMode")
                || IsExactPath(value, "/TreatmentConsent/StartSmMode")
                || IsExactPath(value, "/TreatmentConsent/SmModeStatus"))
            {
                return true;
            }

            // Only the locked service member station page is allowed.
            if (IsExactPath(value, "/TreatmentConsent/TreatmentConsentStation"))
            {
                var smIdRaw = QueryHelpersGet(query, "serviceMembersChildId");
                if (long.TryParse(smIdRaw, out var requestedId)
                    && requestedId == lockedServiceMembersChildId)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsExactPath(string path, string expected)
        {
            return string.Equals(path, expected, StringComparison.OrdinalIgnoreCase);
        }

        private static string? QueryHelpersGet(QueryString query, string key)
        {
            if (!query.HasValue)
            {
                return null;
            }

            var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query.Value);
            if (parsed.TryGetValue(key, out var values) && values.Count > 0)
            {
                return values[0];
            }

            return null;
        }
    }
}
