using MediatR;

namespace PeakVentures.StorageService.Core.UserResponse
{
    public class SaveUserResponseCommand : INotification
    {
        public string? Referer { get; set; }
        public string? UserAgent { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
