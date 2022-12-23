namespace PeakVentures.StorageService.Core.UserResponse
{
    public class UserResponseNotification
    {
        public string? Referer { get; set; }
        public string? UserAgent { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
