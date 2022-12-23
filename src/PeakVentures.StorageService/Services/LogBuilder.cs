using System.Globalization;

namespace PeakVentures.StorageService.Services
{
    public interface ILogBuilder
    {
        string Build(string? referrer, string? userAgent, string ipAddress, DateTime createdAt);
    }

    public class LogBuilder : ILogBuilder
    {
        public string Build(string? referrer, string? userAgent, string ipAddress, DateTime createdAt)
        {
            var date = createdAt.ToString("o", CultureInfo.InvariantCulture);
            var referrerValue = referrer ?? "null";
            var userAgentValue = userAgent ?? "null";
            return $"{date}|{referrerValue}|{userAgentValue}|{ipAddress}";
        }
    }
}
