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

        public async Task Handle(SaveUserResponseCommand notification, CancellationToken cancellationToken)
        {
            var record = builder.Build(notification.Referer, notification.UserAgent, notification.IpAddress, notification.CreatedAt);
            
            // Introduce an abstraction for better testability 
            using var fs = File.Open(config.FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            byte[] bytes = Encoding.ASCII.GetBytes(record + Environment.NewLine);
            await fs.WriteAsync(bytes, cancellationToken);
            fs.Close();
        }
    }
}
