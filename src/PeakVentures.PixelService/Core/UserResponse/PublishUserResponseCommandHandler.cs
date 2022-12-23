using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using PeakVentures.PixelService.Services;

namespace PeakVentures.PixelService.Core.UserResponse
{
    internal class PublishUserResponseCommandHandler : INotificationHandler<PublishUserResponseCommand>
    {
        private readonly IServiceBusClientFactory serviceBusClientFactory;

        public PublishUserResponseCommandHandler(IServiceBusClientFactory serviceBusClientFactory)
        {
            this.serviceBusClientFactory = serviceBusClientFactory;
        }

        public async Task Handle(PublishUserResponseCommand notification, CancellationToken cancellationToken)
        {
            var queueMessage = new UserResponseNotification
            {
                IpAddress = notification.IpAddress,
                Referer = notification.Referer,
                UserAgent = notification.UserAgent
            };

            var sender = serviceBusClientFactory.GetOrCreateSender();
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(queueMessage));
            await sender.SendMessageAsync(message);
        }
    }
}
