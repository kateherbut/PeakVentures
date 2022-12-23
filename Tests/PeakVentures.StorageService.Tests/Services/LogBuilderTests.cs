using PeakVentures.StorageService.Services;

namespace PeakVentures.StorageService.Tests.Services
{
    public class LogBuilderTests
    {
        private readonly LogBuilder sut = new();

        [Theory]
        [InlineData("referrer", "userAgent", "ipAddress", "2022-12-23T00:00:00.0000000|referrer|userAgent|ipAddress")]
        [InlineData(null, "userAgent", "ipAddress", "2022-12-23T00:00:00.0000000|null|userAgent|ipAddress")]
        [InlineData("referrer", null, "ipAddress", "2022-12-23T00:00:00.0000000|referrer|null|ipAddress")]
        public void Build_Always_ReturnsCorectValue(string? referrer, string? userAgent, string ipAddress, string expectedResult)
        {
            var date = new DateTime(2022, 12, 23);

            var result = sut.Build(referrer, userAgent, ipAddress, date);

            result.Should().Be(expectedResult);
        }
    }
}
