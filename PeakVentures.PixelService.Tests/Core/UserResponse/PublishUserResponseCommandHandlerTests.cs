using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PeakVentures.PixelService.Core.UserResponse;
using PeakVentures.PixelService.Services;
using System.Text;

namespace PeakVentures.PixelService.Tests.Core.UserResponse
{
    public class PublishUserResponseCommandHandlerTests
    {
        private readonly PublishUserResponseCommandHandler sut;
        private readonly Mock<IServiceBusClientFactory> serviceBusClientFactoryMock = new();
        private readonly Mock<ServiceBusSender> serviceBusSenderMock = new();

        public PublishUserResponseCommandHandlerTests()
        {
            serviceBusClientFactoryMock.Setup(x => x.GetOrCreateSender()).Returns(serviceBusSenderMock.Object);
            sut = new PublishUserResponseCommandHandler(serviceBusClientFactoryMock.Object);
        }

        [Theory, AutoData]
        public async Task Handle_Always_SendCorrectMessageToServiceBus(PublishUserResponseCommand notification)
        {
            ServiceBusMessage sentMessage = null;
            serviceBusSenderMock.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default))
                .Callback((ServiceBusMessage message, CancellationToken _) => sentMessage = message);

            await sut.Handle(notification, default);

            var actualMessage = JsonConvert.DeserializeObject<UserResponseNotification>(Encoding.UTF8.GetString(sentMessage.Body));
            var expectedMessage = new UserResponseNotification
            {
                IpAddress = notification.IpAddress,
                Referer = notification.Referer,
                UserAgent = notification.UserAgent
            };

            actualMessage.Should().BeEquivalentTo(expectedMessage, opt => opt.Excluding(x => x.CreatedAt));
        }
    }
}
