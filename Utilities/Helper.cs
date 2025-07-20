namespace Malama.Utilities
{
    public static class Helper
    {
        public static DateTime? NormalizeDateTime(DateTime? dt)
        {
            if (dt == null) return null;
            return DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified);
        }
    }
}
