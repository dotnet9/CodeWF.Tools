using CodeWF.Tools.Extensions;
using Xunit;

namespace CodeWF.Tools.Test
{
    public class DateTimeExtensionTest
    {
        // GetUnixTime*/FromUnixTime* 默认按当前系统时区换算（见 DateTimeExtension 注释），
        // 而 CI 运行在 UTC 时区，因此夹具不能硬编码某个时区的本地时刻：
        // 先固定一个 UTC 时刻（epoch 值与时区无关），再按本机时区推导输入和期望值。
        private static readonly DateTimeOffset TestInstantUtc =
            new DateTimeOffset(2024, 7, 6, 15, 12, 33, TimeSpan.Zero);

        private static readonly DateTime _testDateTime = TestInstantUtc.LocalDateTime;

        private static readonly DateTimeOffset _testDateTimeOffset = TestInstantUtc.ToLocalTime();

        private const long ExpectedUnixTimeSeconds = 1720278753;
        private const int StartYear = 2024;

        private static readonly uint ExpectedSpecialUnixTimeSeconds =
            (uint)((TestInstantUtc.UtcDateTime.Ticks -
                    new DateTimeOffset(StartYear, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime.Ticks) / 1_000_000L);

        private const long ExpectedUnixTimeMilliseconds = 1720278753000;

        [Fact]
        public void Test_GetUnixTimeSeconds_DateTime()
        {
            var actualResult = _testDateTime.GetUnixTimeSeconds();

            Assert.Equal(ExpectedUnixTimeSeconds, actualResult);
        }

        [Fact]
        public void Test_GetUnixTimeSeconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetUnixTimeSeconds();

            Assert.Equal(ExpectedUnixTimeSeconds, actualResult);
        }


        [Fact]
        public void Test_GetSpecialUnixTimeSeconds_DateTime()
        {
            var actualResult = _testDateTime.GetSpecialUnixTimeSeconds(StartYear);

            Assert.Equal(ExpectedSpecialUnixTimeSeconds, actualResult);
        }

        [Fact]
        public void Test_GetSpecialUnixTimeSeconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetSpecialUnixTimeSeconds(StartYear);

            Assert.Equal(ExpectedSpecialUnixTimeSeconds, actualResult);
        }

        [Fact]
        public void Test_GetUnixTimeMilliseconds_DateTime()
        {
            var actualResult = _testDateTime.GetUnixTimeMilliseconds();

            Assert.Equal(ExpectedUnixTimeMilliseconds, actualResult);
        }

        [Fact]
        public void Test_GetUnixTimeMilliseconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetUnixTimeMilliseconds();

            Assert.Equal(ExpectedUnixTimeMilliseconds, actualResult);
        }


        [Fact]
        public void Test_FromUnixTimeSecondsToDateTime_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeSeconds.FromUnixTimeSecondsToDateTime();

            Assert.Equal(_testDateTime, actualResult);
        }

        [Fact]
        public void Test_FromUnixTimeSecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeSeconds.FromUnixTimeSecondsToDateTimeOffset();

            Assert.Equal(_testDateTimeOffset, actualResult);
        }


        [Fact]
        public void Test_FromSpecialUnixTimeSecondsToDateTime_ValidSeconds()
        {
            var actualResult =
                ExpectedSpecialUnixTimeSeconds.FromSpecialUnixTimeSecondsToDateTime(StartYear);

            Assert.Equal(_testDateTime, actualResult);
        }

        [Fact]
        public void Test_FromSpecialUnixTimeSecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedSpecialUnixTimeSeconds.FromSpecialUnixTimeSecondsToDateTimeOffset(StartYear);

            Assert.Equal(_testDateTimeOffset, actualResult);
        }

        [Fact]
        public void Test_FromUnixTimeMillisecondsToDateTime_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeMilliseconds.FromUnixTimeMillisecondsToDateTime();

            Assert.Equal(_testDateTime, actualResult);
        }

        [Fact]
        public void Test_FromUnixTimeMillisecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeMilliseconds.FromUnixTimeMillisecondsToDateTimeOffset();

            Assert.Equal(_testDateTimeOffset, actualResult);
        }
    }
}