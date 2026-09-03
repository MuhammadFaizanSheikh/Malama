using AutoMapper;
using Malama.Models;
using System;

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

        /// <summary>
        /// Event day end times of 00:00 with a later start mean end-of-day (24:00), not start-of-day.
        /// </summary>
        public static int ResolveEventDayEndMinutes(TimeSpan? startTime, TimeSpan? endTime, int defaultEndMinutes = 24 * 60)
        {
            if (!endTime.HasValue)
            {
                return defaultEndMinutes;
            }

            var endMinutes = (int)endTime.Value.TotalMinutes;
            var startMinutes = startTime.HasValue ? (int)startTime.Value.TotalMinutes : 0;

            if (endMinutes == 0 && startMinutes > 0)
            {
                return 24 * 60;
            }

            if (endMinutes <= startMinutes && startMinutes > 0)
            {
                return 24 * 60;
            }

            return endMinutes;
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

        public static void ConvertEventToLocalTime(EventManagement eventManagement, string timezoneId)
        {
            try
            {
                if (eventManagement == null)
                    throw new ArgumentNullException(nameof(eventManagement));

                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
                DateTime eventStartUtc = eventManagement.EventStartDateUtc;
                DateTime eventEndUtc = eventManagement.EventEndDateUtc;

                eventManagement.EventStartDateUtc = TimeZoneInfo.ConvertTimeFromUtc(eventStartUtc, tz);
                eventManagement.EventEndDateUtc = TimeZoneInfo.ConvertTimeFromUtc(eventEndUtc, tz);

                foreach (var day in eventManagement.EventStartEndTimeDayWiseList)
                {
                    if (day.EventStartTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventStartTime.Value;
                        DateTime dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        day.EventStartTime = dayLocal.TimeOfDay;
                    }

                    if (day.EventEndTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventEndTime.Value;
                        DateTime dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        day.EventEndTime = dayLocal.TimeOfDay;
                    }
                }
            }
            catch (TimeZoneNotFoundException ex)
            {
                throw;
            }
            catch (InvalidTimeZoneException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static EventLocalTimeResult ConvertEventToLocalTime(DateTime eventStartUtc, DateTime eventEndUtc, string timezoneId, List<EventStartEndTimeDayWise> dayWiseList)
        {
            if (string.IsNullOrEmpty(timezoneId))
                throw new ArgumentException("Timezone is required");

            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            var result = new EventLocalTimeResult
            {
                EventStartLocal = TimeZoneInfo.ConvertTimeFromUtc(eventStartUtc, tz),
                EventEndLocal = TimeZoneInfo.ConvertTimeFromUtc(eventEndUtc, tz)
            };

            if (dayWiseList != null)
            {
                foreach (var day in dayWiseList)
                {
                    var dayResult = new DayTimeResult
                    {
                        EventDay = day.EventDay
                    };

                    if (day.EventStartTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventStartTime.Value;
                        var local = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        dayResult.StartTimeLocal = local.TimeOfDay;
                    }

                    if (day.EventEndTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventEndTime.Value;
                        var local = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        dayResult.EndTimeLocal = local.TimeOfDay;
                    }

                    result.DayWise.Add(dayResult);
                }
            }

            return result;
        }

        public static void ConvertEventToLocalTime(PostEventManagement eventManagement, string timezoneId)
        {
            try
            {
                if (eventManagement == null)
                    throw new ArgumentNullException(nameof(eventManagement));

                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
                DateTime eventStartUtc = eventManagement.EventStartDateUtc;
                DateTime eventEndUtc = eventManagement.EventEndDateUtc;

                eventManagement.EventStartDateUtc = TimeZoneInfo.ConvertTimeFromUtc(eventStartUtc, tz);
                eventManagement.EventEndDateUtc = TimeZoneInfo.ConvertTimeFromUtc(eventEndUtc, tz);

                foreach (var day in eventManagement.PostEventStartEndTimeDayWise)
                {
                    if (day.EventStartTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventStartTime.Value;
                        DateTime dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        day.EventStartTime = dayLocal.TimeOfDay;
                    }

                    if (day.EventEndTime.HasValue)
                    {
                        DateTime dayUtc = eventStartUtc.AddDays(day.EventDay - 1).Date + day.EventEndTime.Value;
                        DateTime dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayUtc, tz);
                        day.EventEndTime = dayLocal.TimeOfDay;
                    }
                }
            }
            catch (TimeZoneNotFoundException ex)
            {
                throw;
            }
            catch (InvalidTimeZoneException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static ResponseDto ConvertEventTimesToUtc(EventManagement eventManagement, string timeZoneId)
        {
            var response = new ResponseDto();

            try
            {
                if (eventManagement == null)
                {
                    return new ResponseDto { Success = false, Message = "Event data is required." };
                }

                if (string.IsNullOrWhiteSpace(timeZoneId))
                {
                    return new ResponseDto { Success = false, Message = "Event timezone is required." };
                }

                if (!eventManagement.EventStartEndTimeDayWiseList.Any())
                {
                    return new ResponseDto { Success = false, Message = "Event must have at least one day with start/end time." };
                }

                // Validate first/last day times
                if (!eventManagement.EventStartEndTimeDayWiseList.First().EventStartTime.HasValue)
                {
                    return new ResponseDto { Success = false, Message = "First day start time is required." };
                }

                if (!eventManagement.EventStartEndTimeDayWiseList.Last().EventEndTime.HasValue)
                {
                    return new ResponseDto { Success = false, Message = "Last day end time is required." };
                }

                DateTime localEventStartDate = eventManagement.EventStartDateUtc;

                DateTime startUtc = Helper.ConvertToUtcBasedOnTimezone(
                    localEventStartDate,
                    eventManagement.EventStartEndTimeDayWiseList.First().EventStartTime,
                    timeZoneId,
                    out string startError
                );
                if (!string.IsNullOrEmpty(startError))
                {
                    return new ResponseDto { Success = false, Message = startError };
                }

                DateTime endUtc = Helper.ConvertToUtcBasedOnTimezone(
                    eventManagement.EventEndDateUtc,
                    eventManagement.EventStartEndTimeDayWiseList.Last().EventEndTime,
                    timeZoneId,
                    out string endError
                );
                if (!string.IsNullOrEmpty(endError))
                {
                    return new ResponseDto { Success = false, Message = endError };
                }

                eventManagement.EventStartDateUtc = Helper.NormalizeDateTime(startUtc);
                eventManagement.EventEndDateUtc = Helper.NormalizeDateTime(endUtc);

                // --- Convert per-day EventStartTime / EventEndTime ---
                foreach (var day in eventManagement.EventStartEndTimeDayWiseList)
                {
                    // Start time
                    if (day.EventStartTime.HasValue)
                    {
                        DateTime utcStart = Helper.ConvertToUtcBasedOnTimezone(
                            localEventStartDate.AddDays(day.EventDay - 1),
                            day.EventStartTime,
                            timeZoneId,
                            out string dayStartError
                        );

                        if (!string.IsNullOrEmpty(dayStartError))
                        {
                            return new ResponseDto { Success = false, Message = dayStartError };
                        }

                        day.EventStartTime = utcStart.TimeOfDay;
                    }

                    // End time
                    if (day.EventEndTime.HasValue)
                    {
                        DateTime utcEnd = Helper.ConvertToUtcBasedOnTimezone(
                            localEventStartDate.AddDays(day.EventDay - 1),
                            day.EventEndTime,
                            timeZoneId,
                            out string dayEndError
                        );

                        if (!string.IsNullOrEmpty(dayEndError))
                        {
                            return new ResponseDto { Success = false, Message = dayEndError };
                        }

                        day.EventEndTime = utcEnd.TimeOfDay;
                    }
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = "Event times converted to UTC successfully.",
                    Data = eventManagement
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public static ResponseDto ConvertEventTimesToUtc(PostEventManagement eventManagement, string timeZoneId)
        {
            var response = new ResponseDto();

            try
            {
                if (eventManagement == null)
                {
                    return new ResponseDto { Success = false, Message = "Event data is required." };
                }

                if (string.IsNullOrWhiteSpace(timeZoneId))
                {
                    return new ResponseDto { Success = false, Message = "Event timezone is required." };
                }

                if (!eventManagement.PostEventStartEndTimeDayWise.Any())
                {
                    return new ResponseDto { Success = false, Message = "Event must have at least one day with start/end time." };
                }

                // Validate first/last day times
                if (!eventManagement.PostEventStartEndTimeDayWise.First().EventStartTime.HasValue)
                {
                    return new ResponseDto { Success = false, Message = "First day start time is required." };
                }

                if (!eventManagement.PostEventStartEndTimeDayWise.Last().EventEndTime.HasValue)
                {
                    return new ResponseDto { Success = false, Message = "Last day end time is required." };
                }

                DateTime localEventStartDate = eventManagement.EventStartDateUtc;

                DateTime startUtc = Helper.ConvertToUtcBasedOnTimezone(
                    localEventStartDate,
                    eventManagement.PostEventStartEndTimeDayWise.First().EventStartTime,
                    timeZoneId,
                    out string startError
                );
                if (!string.IsNullOrEmpty(startError))
                {
                    return new ResponseDto { Success = false, Message = startError };
                }

                DateTime endUtc = Helper.ConvertToUtcBasedOnTimezone(
                    eventManagement.EventEndDateUtc,
                    eventManagement.PostEventStartEndTimeDayWise.Last().EventEndTime,
                    timeZoneId,
                    out string endError
                );
                if (!string.IsNullOrEmpty(endError))
                {
                    return new ResponseDto { Success = false, Message = endError };
                }

                eventManagement.EventStartDateUtc = Helper.NormalizeDateTime(startUtc);
                eventManagement.EventEndDateUtc = Helper.NormalizeDateTime(endUtc);

                // --- Convert per-day EventStartTime / EventEndTime ---
                foreach (var day in eventManagement.PostEventStartEndTimeDayWise)
                {
                    // Start time
                    if (day.EventStartTime.HasValue)
                    {
                        DateTime utcStart = Helper.ConvertToUtcBasedOnTimezone(
                            localEventStartDate.AddDays(day.EventDay - 1),
                            day.EventStartTime,
                            timeZoneId,
                            out string dayStartError
                        );

                        if (!string.IsNullOrEmpty(dayStartError))
                        {
                            return new ResponseDto { Success = false, Message = dayStartError };
                        }

                        day.EventStartTime = utcStart.TimeOfDay;
                    }

                    // End time
                    if (day.EventEndTime.HasValue)
                    {
                        DateTime utcEnd = Helper.ConvertToUtcBasedOnTimezone(
                            localEventStartDate.AddDays(day.EventDay - 1),
                            day.EventEndTime,
                            timeZoneId,
                            out string dayEndError
                        );

                        if (!string.IsNullOrEmpty(dayEndError))
                        {
                            return new ResponseDto { Success = false, Message = dayEndError };
                        }

                        day.EventEndTime = utcEnd.TimeOfDay;
                    }
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = "Event times converted to UTC successfully.",
                    Data = eventManagement
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

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
