using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Test
{
    [TestClass]
    public class DateTimeExtensionTest
    {
        private readonly DateTime _testDateTime = new DateTime(2024, 7, 6, 23, 12, 33, DateTimeKind.Local);

        private readonly DateTimeOffset _testDateTimeOffset =
            new DateTimeOffset(2024, 7, 6, 23, 12, 33, TimeSpan.FromHours(8));

        private const long ExpectedUnixTimeSeconds = 1720278753;
        private const int StartYear = 2024;
        private const uint ExpectedSpecialUnixTimeSeconds = 162115530;
        private const long ExpectedUnixTimeMilliseconds = 1720278753000;

        [TestMethod]
        public void Test_GetUnixTimeSeconds_DateTime()
        {
            var actualResult = _testDateTime.GetUnixTimeSeconds(TimeSpan.FromHours(8));

            Assert.AreEqual(ExpectedUnixTimeSeconds, actualResult);
        }

        [TestMethod]
        public void Test_GetUnixTimeSeconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetUnixTimeSeconds();

            Assert.AreEqual(ExpectedUnixTimeSeconds, actualResult);
        }


        [TestMethod]
        public void Test_GetSpecialUnixTimeSeconds_DateTime()
        {
            var actualResult = _testDateTime.GetSpecialUnixTimeSeconds(TimeSpan.FromHours(8), StartYear);

            Assert.AreEqual(ExpectedSpecialUnixTimeSeconds, actualResult);
        }

        [TestMethod]
        public void Test_GetSpecialUnixTimeSeconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetSpecialUnixTimeSeconds(StartYear);

            Assert.AreEqual(ExpectedSpecialUnixTimeSeconds, actualResult);
        }

        [TestMethod]
        public void Test_GetUnixTimeMilliseconds_DateTime()
        {
            var actualResult = _testDateTime.GetUnixTimeMilliseconds(TimeSpan.FromHours(8));

            Assert.AreEqual(ExpectedUnixTimeMilliseconds, actualResult);
        }

        [TestMethod]
        public void Test_GetUnixTimeMilliseconds_DateTimeOffset()
        {
            var actualResult = _testDateTimeOffset.GetUnixTimeMilliseconds();

            Assert.AreEqual(ExpectedUnixTimeMilliseconds, actualResult);
        }


        [TestMethod]
        public void Test_FromUnixTimeSecondsToDateTime_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeSeconds.FromUnixTimeSecondsToDateTime();

            Assert.AreEqual(_testDateTime, actualResult);
        }

        [TestMethod]
        public void Test_FromUnixTimeSecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeSeconds.FromUnixTimeSecondsToDateTimeOffset();

            Assert.AreEqual(_testDateTimeOffset, actualResult);
        }


        [TestMethod]
        public void Test_FromSpecialUnixTimeSecondsToDateTime_ValidSeconds()
        {
            var actualResult =
                ExpectedSpecialUnixTimeSeconds.FromSpecialUnixTimeSecondsToDateTime(TimeSpan.FromHours(8), StartYear);

            Assert.AreEqual(_testDateTime, actualResult);
        }

        [TestMethod]
        public void Test_FromSpecialUnixTimeSecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedSpecialUnixTimeSeconds.FromSpecialUnixTimeSecondsToDateTimeOffset(StartYear);

            Assert.AreEqual(_testDateTimeOffset, actualResult);
        }

        [TestMethod]
        public void Test_FromUnixTimeMillisecondsToDateTime_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeMilliseconds.FromUnixTimeMillisecondsToDateTime(TimeSpan.FromHours(8));

            Assert.AreEqual(_testDateTime, actualResult);
        }

        [TestMethod]
        public void Test_FromUnixTimeMillisecondsToDateTimeOffset_ValidSeconds()
        {
            var actualResult = ExpectedUnixTimeMilliseconds.FromUnixTimeMillisecondsToDateTimeOffset();

            Assert.AreEqual(_testDateTimeOffset, actualResult);
        }
    }
}