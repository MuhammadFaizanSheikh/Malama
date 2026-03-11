namespace Malama.Utilities
{
    public static class Helper
    {
        public static DateTime? NormalizeDateTime(DateTime? dt)
        {
            if (dt == null) return null;
            return DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified);
        }

        public static DateTime ConvertToUtcBasedOnTimezone(DateTime localDate, TimeSpan? time, string timeZoneId, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                DateTime localDateTime = localDate.Date + (time ?? TimeSpan.Zero);
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                errorMessage = $"Invalid timezone ID: {timeZoneId}";
            }
            catch (InvalidTimeZoneException)
            {
                errorMessage = $"Invalid timezone data for: {timeZoneId}";
            }
            catch (Exception ex)
            {
                errorMessage = "Error converting time to UTC: " + ex.Message;
            }

            return default; // in case of error
        }
    }
}
