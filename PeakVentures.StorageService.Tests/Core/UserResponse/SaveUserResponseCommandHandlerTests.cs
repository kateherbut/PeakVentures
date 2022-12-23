using Microsoft.Extensions.Options;
using PeakVentures.StorageService.Configurations;
using PeakVentures.StorageService.Core.UserResponse;
using PeakVentures.StorageService.Services;

namespace PeakVentures.StorageService.Tests.Core.UserResponse
{
    public class SaveUserResponseCommandHandlerTests : IDisposable
    {
        private readonly Mock<IOptionsSnapshot<StorageConfiguration>> storageConfigurationMock = new();
        private readonly Mock<ILogBuilder> logBuilderMock = new();
        private readonly SaveUserResponseCommandHandler sut;
        private const string TestFileName = "test.log";

        public SaveUserResponseCommandHandlerTests()
        {
            storageConfigurationMock.Setup(x => x.Value).Returns(new StorageConfiguration { FilePath = "test.log" });
            sut = new SaveUserResponseCommandHandler(storageConfigurationMock.Object, logBuilderMock.Object);
        }

        [Theory, AutoData]
        public async Task Handle_OneNotification_CreatesFileAndSavesNewLog(SaveUserResponseCommand notification)
        {
            logBuilderMock.Setup(x => x.Build(notification.Referer, notification.UserAgent, notification.IpAddress, notification.CreatedAt)).Returns("log");

            await sut.Handle(notification, default);

            var result = await File.ReadAllLinesAsync(TestFileName);

            result.Should().HaveCount(1);
        }

        [Theory, AutoData]
        public async Task Handle_MultipleNotifications_CreatesFileAndSavesAllLogs(SaveUserResponseCommand[] notifications)
        {
            logBuilderMock.Setup(x => x.Build(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns("log");

            var tasks = notifications.Select(x => sut.Handle(x, default));

            await Task.WhenAll(tasks);

            var result = await File.ReadAllLinesAsync(TestFileName);

            result.Should().HaveCount(notifications.Length);
        }

        public void Dispose()
        {
            File.Delete(TestFileName);
        }
    }
}
