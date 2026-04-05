using AutoMapper;

namespace Malama.Utilities
{
    public static class Helper
    {
        public static DateTime? NormalizeDateTime(DateTime? dt)
        {
            if (dt == null) return null;
            return DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified);
        }

        public static DateTime NormalizeDateTime(DateTime dt)
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
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

        //public static void UpdateCollection<TItem, TKey>(
        //    ICollection<TItem> existingList,
        //    ICollection<TItem> updatedList,
        //    Func<TItem, TKey> keySelector,
        //    IMapper mapper)
        //{
        //    if (existingList == null) throw new ArgumentNullException(nameof(existingList));
        //    if (updatedList == null) throw new ArgumentNullException(nameof(updatedList));

        //    // Remove deleted items
        //    var toRemove = existingList
        //        .Where(e => updatedList.All(u => !keySelector(u)!.Equals(keySelector(e))))
        //        .ToList();

        //    foreach (var item in toRemove)
        //        existingList.Remove(item);

        //    // Update existing items & add new ones
        //    foreach (var updatedItem in updatedList)
        //    {
        //        var existingItem = existingList
        //            .FirstOrDefault(e => keySelector(e)!.Equals(keySelector(updatedItem)));

        //        if (existingItem != null)
        //        {
        //            mapper.Map(updatedItem, existingItem);
        //        }
        //        else
        //        {
        //            existingList.Add(updatedItem);
        //        }
        //    }
        //}

        public static void UpdateCollection<TItem, TKey>(
    ICollection<TItem> existingList,
    ICollection<TItem> updatedList,
    Func<TItem, TKey> keySelector,
    IMapper mapper,
    Action<TItem, TItem>? updateChildren = null)
        {
            if (existingList == null) throw new ArgumentNullException(nameof(existingList));
            if (updatedList == null) throw new ArgumentNullException(nameof(updatedList));

            var toRemove = existingList
                .Where(e => updatedList.All(u => !keySelector(u)!.Equals(keySelector(e))))
                .ToList();

            foreach (var item in toRemove)
                existingList.Remove(item);

            foreach (var updatedItem in updatedList)
            {
                var existingItem = existingList
                    .FirstOrDefault(e => keySelector(e)!.Equals(keySelector(updatedItem)));

                if (existingItem != null)
                {
                    mapper.Map(updatedItem, existingItem);

                    updateChildren?.Invoke(existingItem, updatedItem);
                }
                else
                {
                    existingList.Add(updatedItem);
                }
            }
        }
    }
}
