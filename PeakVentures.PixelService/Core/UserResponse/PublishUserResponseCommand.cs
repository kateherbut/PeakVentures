using MediatR;

namespace PeakVentures.PixelService.Core.UserResponse
{
    public record PublishUserResponseCommand : INotification
    {
        public string? Referer { get; set; }
        public string? UserAgent { get; set; }
        public string IpAddress { get; set; }
    }
}
