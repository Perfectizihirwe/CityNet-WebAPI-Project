using System.Text.Json;
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
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities([FromQuery] string? name, [FromQuery] string? searchQuery, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            const int maxCitiesPageSize = 20;

            if (maxCitiesPageSize < pageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityRepository.GetCitiesAsync(name, searchQuery, pageSize, pageNumber);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));


            return Ok(_mapper.Map<IEnumerable<CityWithoutPOIDto>>(cityEntities));
            // return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePOI = false)
        {
            var city = await _cityRepository.GetCityAsync(id, includePOI);
            if (city == null)
            {
                return NotFound();
            }
            if (includePOI)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPOIDto>(city));

        }
    }
}