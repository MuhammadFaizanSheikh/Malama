namespace ExcelFilesCompiler.Utilities
{
    public static class AppConstants
    {
        public static class EventStatus
        {
            public const string InProgressComplete = "In Progress Complete";
            public const string EventManagementRoles = "EventManagementRole";
        }

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

        public static class Status
        {
            public const string Pending = "Pending";
            public const string Completed = "Completed";
        }

        public static class YesNo
        {
            public const string Yes = "Yes";
            public const string No = "No";
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
