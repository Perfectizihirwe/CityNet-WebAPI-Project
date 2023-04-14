using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPOIDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}