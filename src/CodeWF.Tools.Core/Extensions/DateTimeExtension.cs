using System;
using System.Collections.Generic;
using System.Globalization;

namespace CodeWF.Tools.Extensions
{
    /// <summary>
    /// 日期操作工具类
    /// </summary>
    public static class DateTimeExtension
    {
        private static readonly DateTimeOffset UnixEpochStart = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // 1天的毫秒数
        private const long MillisecondsPerDay = 24 * 60 * 60 * 1000;

        // uint能表示的最大天数
        private const double MaxDaysForUint = uint.MaxValue / (double)MillisecondsPerDay;

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的秒数，默认使用当前系统时区的偏移量，比如北京时间的TimeSpan.FromHours(8)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetUnixTimeSeconds(this DateTime dt) =>
            dt.GetUnixTimeSeconds(TimeZoneInfo.Local.BaseUtcOffset);

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long GetUnixTimeSeconds(this DateTime dt, TimeSpan offset) =>
            new DateTimeOffset(dt, offset).ToUnixTimeSeconds();

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetUnixTimeSeconds(this DateTimeOffset dt) => dt.ToUnixTimeSeconds();

        /// <summary>
        /// 获取该时间相对于指定年份的精确到0.1秒的特殊时间戳，使用4个字节的整型（uint）以节省空间，startYear需在当前时间10年以内, 默认使用当前系统时区的偏移量，比如北京时间的TimeSpan.FromHours(8)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static uint GetSpecialUnixTimeSeconds(this DateTime dt, int startYear)
        {
            return dt.GetSpecialUnixTimeSeconds(
                dt.Kind == DateTimeKind.Utc ? TimeSpan.Zero : TimeZoneInfo.Local.BaseUtcOffset, startYear);
        }

        /// <summary>
        /// 获取该时间相对于指定年份的精确到0.1秒的特殊时间戳，使用4个字节的整型（uint）以节省空间，startYear需在当前时间10年以内
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="offset"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static uint GetSpecialUnixTimeSeconds(this DateTime dt, TimeSpan offset, int startYear) =>
            (uint)((new DateTimeOffset(dt, offset).UtcDateTime.Ticks -
                    new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.Ticks) / 1_000_000L);

        /// <summary>
        /// 获取该时间相对于指定年份的精确到0.1秒的特殊时间戳，使用4个字节的整型（uint）以节省空间，startYear需在当前时间10年以内
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static uint GetSpecialUnixTimeSeconds(this DateTimeOffset dt, int startYear) =>
            (uint)((dt.UtcDateTime.Ticks -
                    new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.Ticks) / 1_000_000L);


        /// <summary>
        /// 获取该时间相对于指定年份的精确到毫秒的特殊时间戳, 默认使用当前系统时区的偏移量，比如北京时间的TimeSpan.FromHours(8)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static ulong GetSpecialUnixTimeMilliseconds(this DateTime dt, int startYear)
        {
            return dt.GetSpecialUnixTimeMilliseconds(
                dt.Kind == DateTimeKind.Utc ? TimeSpan.Zero : TimeZoneInfo.Local.BaseUtcOffset, startYear);
        }

        /// <summary>
        /// 获取该时间相对于指定年份的精确到毫秒的特殊时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="offset"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static ulong GetSpecialUnixTimeMilliseconds(this DateTime dt, TimeSpan offset, int startYear) =>
            (uint)((new DateTimeOffset(dt, offset).UtcDateTime.Ticks -
                    new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.Ticks) / 10_000L);

        /// <summary>
        /// 获取该时间相对于指定年份的精确到毫秒的特殊时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static ulong GetSpecialUnixTimeMilliseconds(this DateTimeOffset dt, int startYear) =>
            (uint)((dt.UtcDateTime.Ticks -
                    new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.Ticks) / 10_000L);

        /// <summary>
        /// 获取两个时间之间的毫秒间隔，会自动处理不同的DateTimeKind
        /// </summary>
        /// <param name="endDt">结束时间</param>
        /// <param name="startDt">开始时间</param>
        /// <returns>毫秒间隔</returns>
        public static uint GetTimeIntervalMilliseconds(this DateTime endDt, DateTime startDt)
        {
            // 统一转换为UTC时间进行比较
            var endUtc = endDt.Kind == DateTimeKind.Local ? endDt.ToUniversalTime() :
                         endDt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(endDt, DateTimeKind.Utc) :
                         endDt;
                         
            var startUtc = startDt.Kind == DateTimeKind.Local ? startDt.ToUniversalTime() :
                           startDt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(startDt, DateTimeKind.Utc) :
                           startDt;

            var interval = endUtc - startUtc;
            CheckForPrecisionLoss(interval);
            return (uint)interval.TotalMilliseconds;
        }

        /// <summary>
        /// 获取两个时间之间的毫秒间隔，可指定时区偏移量
        /// </summary>
        /// <param name="endDt">结束时间</param>
        /// <param name="offset">时区偏移量</param>
        /// <param name="startDt">开始时间</param>
        /// <returns>毫秒间隔</returns>
        public static uint GetTimeIntervalMilliseconds(this DateTime endDt, TimeSpan offset, DateTime startDt)
        {
            var endOffset = CreateDateTimeOffset(endDt, offset);
            var startOffset = CreateDateTimeOffset(startDt, offset);
            var interval = endOffset - startOffset;
            CheckForPrecisionLoss(interval);
            return (uint)interval.TotalMilliseconds;
        }

        /// <summary>
        /// 获取两个DateTimeOffset类型时间之间的毫秒间隔
        /// </summary>
        /// <param name="endDt">结束时间</param>
        /// <param name="startDt">开始时间</param>
        /// <returns>毫秒间隔</returns>
        public static uint GetTimeIntervalMilliseconds(this DateTimeOffset endDt, DateTimeOffset startDt)
        {
            var interval = endDt - startDt;
            CheckForPrecisionLoss(interval);
            return (uint)interval.TotalMilliseconds;
        }

        private static DateTimeOffset CreateDateTimeOffset(DateTime dt, TimeSpan offset)
        {
            if (dt.Kind == DateTimeKind.Local)
            {
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            }

            return new DateTimeOffset(dt, offset);
        }

        private static void CheckForPrecisionLoss(TimeSpan interval)
        {
            if (interval.TotalDays > MaxDaysForUint)
            {
                throw new OverflowException("时间间隔过大，转换为uint类型时会丢失精度");
            }
        }

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的毫秒数, 默认使用当前系统时区的偏移量，比如北京时间的TimeSpan.FromHours(8)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetUnixTimeMilliseconds(this DateTime dt) =>
            dt.GetUnixTimeMilliseconds(TimeZoneInfo.Local.BaseUtcOffset);

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long GetUnixTimeMilliseconds(this DateTime dt, TimeSpan offset) =>
            new DateTimeOffset(dt, offset).ToUnixTimeMilliseconds();

        /// <summary>
        /// 获取该时间相对于1970-01-01T00:00:00Z的毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetUnixTimeMilliseconds(this in DateTimeOffset dt) => dt.ToUnixTimeMilliseconds();

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的秒数转换为DateTime
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeSecondsToDateTime(this long seconds) =>
            UnixEpochStart.UtcDateTime.AddSeconds(seconds).ToLocalTime();


        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的秒数转换为DateTimeOffset
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTimeOffset FromUnixTimeSecondsToDateTimeOffset(this long seconds) =>
            UnixEpochStart.UtcDateTime.AddSeconds(seconds);

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的精确到0.1秒的特殊时间戳转换为DateTime, 默认使用当前系统时区的偏移量，比如北京时间的TimeSpan.FromHours(8)
        /// </summary>
        /// <param name="specialSeconds"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTime FromSpecialUnixTimeSecondsToDateTime(this uint specialSeconds,
            int startYear) =>
            specialSeconds.FromSpecialUnixTimeSecondsToDateTime(TimeZoneInfo.Local.BaseUtcOffset, startYear);

        public const int StartYear = 2024;

        /// <summary>
        /// 将毫秒级时间戳转换为DateTime对象，精确到0.1秒
        /// </summary>
        /// <param name="milliseconds">要转换的毫秒数</param>
        /// <param name="startYear">起始年份，默认值为StartYear常量所表示的年份(2024年)</param>
        /// <returns>返回一个DateTime对象，表示从指定年份开始的时间</returns>
        public static DateTime ToDateTime(this uint milliseconds, int startYear = StartYear)
        {
            var time = new DateTime(startYear, 1, 1, 0, 0, 0, 0);
            return time.AddMilliseconds((long)milliseconds * 100);
        }

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的精确到0.1秒的特殊时间戳转换为DateTime
        /// </summary>
        /// <param name="specialSeconds"></param>
        /// <param name="offset"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTime FromSpecialUnixTimeSecondsToDateTime(this uint specialSeconds, TimeSpan offset,
            int startYear) =>
            new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.AddTicks(
                specialSeconds * 1_000_000L) + offset;

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的精确到0.1秒的特殊时间戳转换为DateTimeOffset
        /// </summary>
        /// <param name="specialSeconds"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTimeOffset
            FromSpecialUnixTimeSecondsToDateTimeOffset(this uint specialSeconds, int startYear) =>
            new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(specialSeconds * 1_000_000L);

        /// <summary>
        /// 根据开始时间和毫秒间隔，计算结束时间，默认使用当前系统时区的偏移量
        /// </summary>
        /// <param name="startDt">开始时间</param>
        /// <param name="milliseconds">毫秒间隔</param>
        /// <returns>结束时间</returns>
        public static DateTime GetEndDateTime(this DateTime startDt, uint milliseconds)
        {
            var offset = startDt.Kind == DateTimeKind.Utc ? TimeSpan.Zero : TimeZoneInfo.Local.BaseUtcOffset;
            var startOffset = CreateDateTimeOffset(startDt, offset);
            var endOffset = startOffset.AddMilliseconds(milliseconds);
            return endOffset.DateTime;
        }

        /// <summary>
        /// 根据开始时间、时区偏移量和毫秒间隔，计算结束时间
        /// </summary>
        /// <param name="startDt">开始时间</param>
        /// <param name="offset">时区偏移量</param>
        /// <param name="milliseconds">毫秒间隔</param>
        /// <returns>结束时间</returns>
        public static DateTime GetEndDateTime(this DateTime startDt, TimeSpan offset, uint milliseconds)
        {
            var startOffset = CreateDateTimeOffset(startDt, offset);
            var endOffset = startOffset.AddMilliseconds(milliseconds);
            return endOffset.DateTime;
        }

        /// <summary>
        /// 根据开始的DateTimeOffset类型时间和毫秒间隔，计算结束的DateTimeOffset类型时间
        /// </summary>
        /// <param name="startDt">开始时间</param>
        /// <param name="milliseconds">毫秒间隔</param>
        /// <returns>结束时间</returns>
        public static DateTimeOffset GetEndDateTimeOffset(this DateTimeOffset startDt, uint milliseconds)
        {
            return startDt.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的毫秒数转换为DateTime
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMillisecondsToDateTime(this long milliseconds) =>
            milliseconds.FromUnixTimeMillisecondsToDateTime(TimeZoneInfo.Local.BaseUtcOffset);

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的毫秒数转换为DateTime
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMillisecondsToDateTime(this long milliseconds, TimeSpan offset) =>
            UnixEpochStart.UtcDateTime.AddMilliseconds(milliseconds) + offset;

        /// <summary>
        /// 将相对于1970-01-01T00:00:00Z的毫秒数转换为DateTimeOffset
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static DateTimeOffset FromUnixTimeMillisecondsToDateTimeOffset(this long milliseconds) =>
            UnixEpochStart.UtcDateTime.AddMilliseconds(milliseconds);

        /// <summary>
        /// 获取某一年有多少周
        /// </summary>
        /// <param name="now"></param>
        /// <returns>该年周数</returns>
        public static int GetWeekAmount(this in DateTime now)
        {
            var end = new DateTime(now.Year, 12, 31); //该年最后一天
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday); //该年星期数
        }

        /// <summary>
        /// 返回年度第几个星期   默认星期日是第一天
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>第几周</returns>
        public static int WeekOfYear(this in DateTime date)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// 返回年度第几个星期
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="week">一周的开始日期</param>
        /// <returns>第几周</returns>
        public static int WeekOfYear(this in DateTime date, DayOfWeek week)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, week);
        }

        /// <summary>
        /// 得到一年中的某周的起始日和截止日
        /// 周数 nNumWeek
        /// </summary>
        /// <param name="now"></param>
        /// <param name="nNumWeek">第几周</param>
        public static (DateTime Start, DateTime End) GetWeekTime(this DateTime now, int nNumWeek)
        {
            var dt = new DateTime(now.Year, 1, 1);
            dt += new TimeSpan((nNumWeek - 1) * 7, 0, 0, 0);
            return (dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday),
                dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1));
        }

        /// <summary>本年有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>本天在当年的天数</returns>
        public static int GetDaysOfYear(this in DateTime dt)
        {
            //取得传入参数的年份部分，用来判断是否是闰年
            var n = dt.Year;
            return DateTime.IsLeapYear(n) ? 366 : 365;
        }

        /// <summary>本月有多少天</summary>
        /// <param name="now"></param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(this DateTime now)
        {
            return now.Month switch
            {
                1 => 31,
                2 => DateTime.IsLeapYear(now.Year) ? 29 : 28,
                3 => 31,
                4 => 30,
                5 => 31,
                6 => 30,
                7 => 31,
                8 => 31,
                9 => 30,
                10 => 31,
                11 => 30,
                12 => 31,
                _ => 0
            };
        }

        /// <summary>返回当前日期的星期名称</summary>
        /// <param name="now">日期</param>
        /// <returns>星期名称</returns>
        public static string GetWeekNameOfDay(this in DateTime now)
        {
            return now.DayOfWeek switch
            {
                DayOfWeek.Monday => "星期一",
                DayOfWeek.Tuesday => "星期二",
                DayOfWeek.Wednesday => "星期三",
                DayOfWeek.Thursday => "星期四",
                DayOfWeek.Friday => "星期五",
                DayOfWeek.Saturday => "星期六",
                DayOfWeek.Sunday => "星期日",
                _ => ""
            };
        }

        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="now"></param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(this DateTime now)
        {
            var lastDay = new DateTime(now.Year, now.Month,
                new GregorianCalendar().GetDaysInMonth(now.Year, now.Month));
            return lastDay.Day;
        }

        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="dateTime1">时间1</param>
        /// <param name="dateTime2">时间2</param>
        /// <returns>时间差</returns>
        public static string DateDiff(this in DateTime dateTime1, in DateTime dateTime2)
        {
            string dateDiff;
            var ts = dateTime2 - dateTime1;
            if (ts.TotalDays >= 1)
            {
                dateDiff = ts.TotalDays >= 30 ? (ts.TotalDays / 30) + "个月前" : ts.TotalDays + "天前";
            }
            else
            {
                dateDiff = ts.Hours > 1 ? ts.Hours + "小时前" : ts.Minutes + "分钟前";
            }

            return dateDiff;
        }

        /// <summary>
        /// 计算2个时间差
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>时间差</returns>
        public static string GetDiffTime(this DateTime beginTime, DateTime endTime)
        {
            // 处理时间顺序
            if (endTime < beginTime)
            {
                var temp = beginTime;
                beginTime = endTime;
                endTime = temp;
            }

            // 如果时间差为0
            if (beginTime == endTime)
                return "0秒";

            var result = new List<string>();
            var current = beginTime;

            // 计算年
            int years = endTime.Year - current.Year;
            if (endTime.Month < current.Month || (endTime.Month == current.Month && endTime.Day < current.Day))
            {
                years--;
            }

            if (years > 0)
            {
                result.Add($"{years}年");
                current = current.AddYears(years);
            }

            // 计算月
            int months = endTime.Month - current.Month;
            if (months < 0)
            {
                months += 12;
            }
            if (endTime.Day < current.Day)
            {
                months--;
            }

            if (months > 0)
            {
                result.Add($"{months}个月");
                current = current.AddMonths(months);
            }

            // 计算剩余的时间
            var remaining = endTime - current;

            if (remaining.Days > 0)
            {
                result.Add($"{remaining.Days}天");
            }
            if (remaining.Hours > 0)
            {
                result.Add($"{remaining.Hours}小时");
            }
            if (remaining.Minutes > 0)
            {
                result.Add($"{remaining.Minutes}分钟");
            }
            if (remaining.Seconds > 0 || result.Count == 0)
            {
                result.Add($"{remaining.Seconds}秒");
            }

            return string.Join("", result);
        }

        #region 时间转换核心方法

        /// <summary>
        /// 将本地时间转换为UTC时间
        /// </summary>
        /// <param name="localTime">本地时间</param>
        /// <returns>UTC时间</returns>
        /// <example>
        /// var localTime = new DateTime(2024, 1, 1, 8, 0, 0);
        /// var utcTime = localTime.ToUtcTime(); // 2024-01-01 00:00:00
        /// </example>
        public static DateTime ToUtcTime(this DateTime localTime)
        {
            if (localTime.Kind == DateTimeKind.Utc)
                return localTime;
            
            return localTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(localTime, DateTimeKind.Local).ToUniversalTime()
                : localTime.ToUniversalTime();
        }

        /// <summary>
        /// 将UTC时间转换为本地时间
        /// </summary>
        /// <param name="utcTime">UTC时间</param>
        /// <returns>本地时间</returns>
        /// <example>
        /// var utcTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// var localTime = utcTime.ToLocalTime(); // 在中国为 2024-01-01 08:00:00
        /// </example>
        public static DateTime ToLocalTime(this DateTime utcTime)
        {
            if (utcTime.Kind == DateTimeKind.Local)
                return utcTime;
            
            return utcTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(utcTime, DateTimeKind.Utc).ToLocalTime()
                : utcTime.ToLocalTime();
        }

        #endregion

        #region Unix时间戳转换

        /// <summary>
        /// 获取UTC时间的Unix时间戳（秒）
        /// </summary>
        /// <param name="dateTime">UTC时间</param>
        /// <returns>Unix时间戳（秒）</returns>
        /// <example>
        /// var utcNow = DateTime.UtcNow;
        /// var timestamp = utcNow.ToUnixTimeSeconds();
        /// </example>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            var utcTime = dateTime.ToUtcTime();
            return new DateTimeOffset(utcTime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取UTC时间的Unix时间戳（毫秒）
        /// </summary>
        /// <param name="dateTime">UTC时间</param>
        /// <returns>Unix时间戳（毫秒）</returns>
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            var utcTime = dateTime.ToUtcTime();
            return new DateTimeOffset(utcTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 从Unix时间戳（秒）转换为本地时间
        /// </summary>
        /// <param name="seconds">Unix时间戳（秒）</param>
        /// <returns>本地时间</returns>
        /// <example>
        /// var timestamp = 1704067200; // 2024-01-01 00:00:00 UTC
        /// var localTime = timestamp.FromUnixTimeSeconds(); // 在中国为 2024-01-01 08:00:00
        /// </example>
        public static DateTime FromUnixTimeSeconds(this long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;
        }

        /// <summary>
        /// 从Unix时间戳（毫秒）转换为本地时间
        /// </summary>
        /// <param name="milliseconds">Unix时间戳（毫秒）</param>
        /// <returns>本地时间</returns>
        public static DateTime FromUnixTimeMilliseconds(this long milliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).LocalDateTime;
        }

        #endregion

        #region 时间间隔计算

        /// <summary>
        /// 计算两个时间之间的间隔（毫秒）
        /// </summary>
        /// <param name="endTime">结束时间</param>
        /// <param name="startTime">开始时间</param>
        /// <returns>时间间隔（毫秒）</returns>
        /// <example>
        /// var start = DateTime.Now;
        /// var end = start.AddHours(1);
        /// var interval = end.GetIntervalMilliseconds(start); // 3600000
        /// </example>
        public static long GetIntervalMilliseconds(this DateTime endTime, DateTime startTime)
        {
            return (long)(endTime - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取指定时间间隔后的时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="milliseconds">时间间隔（毫秒）</param>
        /// <returns>结束时间</returns>
        public static DateTime AddMillisecondsTime(this DateTime startTime, long milliseconds)
        {
            return startTime.AddMilliseconds(milliseconds);
        }

        #endregion

        #region 人性化时间显示

        /// <summary>
        /// 获取人性化的时间差描述
        /// </summary>
        /// <param name="current">当前时间</param>
        /// <param name="compareTime">比较时间</param>
        /// <returns>人性化的时间差描述</returns>
        /// <example>
        /// var now = DateTime.Now;
        /// var past = now.AddHours(-2);
        /// var desc = now.GetFriendlyTimeSpan(past); // "2小时前"
        /// </example>
        public static string GetFriendlyTimeSpan(this DateTime current, DateTime compareTime)
        {
            var timeSpan = current - compareTime;
            if (timeSpan.TotalDays > 365)
                return $"{(int)(timeSpan.TotalDays / 365)}年前";
            if (timeSpan.TotalDays > 30)
                return $"{(int)(timeSpan.TotalDays / 30)}个月前";
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}天前";
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours}小时前";
            if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes}分钟前";
            
            return "刚刚";
        }

        #endregion
    }
}