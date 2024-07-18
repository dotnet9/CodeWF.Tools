using CodeWF.Tools.Extensions;
using Xunit;

namespace CodeWF.Tools.Test
{
    public class DateTimeExtensionTest
    {
        private readonly DateTime _testDateTime = new DateTime(2024, 7, 6, 23, 12, 33, DateTimeKind.Local);

        private readonly DateTimeOffset _testDateTimeOffset =
            new DateTimeOffset(2024, 7, 6, 23, 12, 33, TimeZoneInfo.Local.BaseUtcOffset);

        private const long ExpectedUnixTimeSeconds = 1720278753;
        private const int StartYear = 2024;
        private const uint ExpectedSpecialUnixTimeSeconds = 162115530;
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