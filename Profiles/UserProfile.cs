using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.UserForEntityDto, Entities.User>();
            CreateMap<Entities.User, Models.UserForEntityDto>();
        }
    }
}