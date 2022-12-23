using MediatR;
using Microsoft.Extensions.Options;
using PeakVentures.StorageService.Configurations;
using PeakVentures.StorageService.Services;
using System.Text;

namespace PeakVentures.StorageService.Core.UserResponse
{
    public class SaveUserResponseCommandHandler : INotificationHandler<SaveUserResponseCommand>
    {
        private readonly StorageConfiguration config;
        private readonly ILogBuilder builder;

        public SaveUserResponseCommandHandler(IOptionsSnapshot<StorageConfiguration> optionsSnapshot, ILogBuilder builder)
        {
            config = optionsSnapshot.Value;
            this.builder = builder;
        }

        public Task Handle(SaveUserResponseCommand notification, CancellationToken cancellationToken)
        {
            var record = builder.Build(notification.Referer, notification.UserAgent, notification.IpAddress, notification.CreatedAt);
            using (var fs = File.Open(config.FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(record);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return Task.CompletedTask;
        }
    }
}
