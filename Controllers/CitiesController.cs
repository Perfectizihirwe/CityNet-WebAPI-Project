using System.Text.Json;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiversion}/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // <summary>
        // Endpoint to get all cities
        // </summary>
        // <returns>An array of Cities</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities([FromQuery] string? name, [FromQuery] string? searchQuery, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            // Using data from the token we access User.Claims
            Console.Write(User.Claims.FirstOrDefault(c => c.Type == "username")?.Value + "---------------------------------------------");

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

        // <summary>Get a city by ID</summary>
        // <param name="id">ID of the city you want to get</param>
        // <param name="includePOI">If you want to get point of Interests with the city</param>
        // <returns>IActionResult</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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