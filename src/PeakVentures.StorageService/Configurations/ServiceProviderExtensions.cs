using PeakVentures.StorageService.Core.UserResponse;
using PeakVentures.StorageService.Services;

namespace PeakVentures.StorageService.Configurations
{
    public static class ServiceProviderExtensions
    {
        public static async Task RegisterQueueProcessors(this IServiceProvider serviceProvider)
        {
            var serviceBusConsumer = serviceProvider.GetRequiredService<IServiceBusConsumer>();
            await serviceBusConsumer.RegisterOnMessageHandlerAsync<UserResponseNotification, SaveUserResponseCommand>();
        }
    }
}
