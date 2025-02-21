using System;
using System.Globalization;
using CodeWF.Tools.Models;

namespace CodeWF.Tools.Extensions
{
    /// <summary>
    /// 日期操作工具类
    /// </summary>
    public static class DateTimeExtension
    {
        private static readonly DateTimeOffset UnixEpochStart = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
            return dt.GetSpecialUnixTimeSeconds(dt.Kind == DateTimeKind.Utc ? TimeSpan.Zero : TimeZoneInfo.Local.BaseUtcOffset, startYear);
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
            return dt.GetSpecialUnixTimeMilliseconds(dt.Kind == DateTimeKind.Utc ? TimeSpan.Zero : TimeZoneInfo.Local.BaseUtcOffset, startYear);
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
        public static string GetDiffTime(this in DateTime beginTime, in DateTime endTime)
        {
            string strResout = string.Empty;

            //获得2时间的时间间隔秒计算
            TimeSpan span = endTime.Subtract(beginTime);
            if (span.Days >= 365)
            {
                strResout += span.Days / 365 + "年";
            }

            if (span.Days >= 30)
            {
                strResout += span.Days % 365 / 30 + "个月";
            }

            if (span.Days >= 1)
            {
                strResout += (int)(span.TotalDays % 30.42) + "天";
            }

            if (span.Hours >= 1)
            {
                strResout += span.Hours + "小时";
            }

            if (span.Minutes >= 1)
            {
                strResout += span.Minutes + "分钟";
            }

            if (span.Seconds >= 1)
            {
                strResout += span.Seconds + "秒";
            }

            return strResout;
        }
    }
}