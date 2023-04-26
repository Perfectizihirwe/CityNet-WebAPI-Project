using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.UserProfileDto, Entities.User>();
            CreateMap<Entities.User, Models.UserProfileDto>();
        }
    }
}