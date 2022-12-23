using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using PeakVentures.PixelService.Configurations;

namespace PeakVentures.PixelService.Services
{
    public interface IServiceBusClientFactory
    {
        ServiceBusSender GetOrCreateSender();
    }

    public class ServiceBusClientFactory : IServiceBusClientFactory
    {
        private ServiceBusSender? serviceBusSender;
        private readonly ServiceBusConfiguration config;

        public ServiceBusClientFactory(IOptions<ServiceBusConfiguration> config)
        {
            this.config = config.Value;
        }

        public ServiceBusSender GetOrCreateSender()
        {
            if (serviceBusSender != null)
            {
                return serviceBusSender;
            }

            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            var client = new ServiceBusClient(config.ConnectionString, clientOptions);
            serviceBusSender = client.CreateSender(config.QueueName);
            return serviceBusSender;
        }
    }
}
