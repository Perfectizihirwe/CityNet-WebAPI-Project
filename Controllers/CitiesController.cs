using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities()
        {
            var cityEntities = await _cityRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityWithoutPOIDto>>(cityEntities));
            // return Ok(CitiesDataStore.Current.Cities);
        }

        // [HttpGet("{id}")]
        // public ActionResult<CityDto> GetCity(int id)
        // {
        //     var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
        //     if (city == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(city);
        // }
    }
}