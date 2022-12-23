using AutoMapper;
using PeakVentures.StorageService.Core.UserResponse;

namespace PeakVentures.StorageService.Configurations
{
    public class StorageServiceMapperProfile : Profile
    {
        public StorageServiceMapperProfile()
        {
            CreateMap<UserResponseNotification, SaveUserResponseCommand>();
        }
    }
}
