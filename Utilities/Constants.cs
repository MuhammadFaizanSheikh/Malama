namespace ExcelFilesCompiler.Utilities
{
    public static class AppConstants
    {
        public static class RolesCategory
        {
            public const string BasicRoles = "BasicRole";
            public const string EventManagementRoles = "EventManagementRole";
        }

        public static class NeededOrNA
        {
            public const string Needed = "NEEDED";
            public const string NotApplicable = "N/A";
        }

        public static class ResponseCodes
        {
            public const string NotFound = "NOT_FOUND";
            public const string AlreadyExists = "ALREADY_EXISTS";
            public const string Success = "SUCCESS";
        }

        public static class ResponseMessages
        {
            public static string EventNotFound(string eventId) => $"No event found for EventID: {eventId}";
            public const string EventDataAlreadyExists = "Data already exists for this EventID.";
            public const string Success = "Operation completed successfully.";
        }
    }
}
