using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using CityInfo.API.Services;
using CityInfo.API.Entities;
using AutoMapper;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsOfInterest")]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService mailService, ICityRepository cityRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(int cityId)
        {
            try
            {
                // throw new Exception("Hahahahahaha");

                if (!await _cityRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with ID {cityId} doesn't exist");
                    return NotFound();
                }

                var points = await _cityRepository.GetPointsOfInterestForCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(points));
            }
            catch (Exception exe)
            {
                _logger.LogCritical("Exception while getting points of interest", exe);
                return StatusCode(500, "The server has encountered an error that is being fixed");
            }
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int id)
        {
            if (!await _cityRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with ID {cityId} doesn't exist");
                return NotFound();
            }

            if (!await _cityRepository.PointExists(id))
            {
                _logger.LogInformation($"Point of interest with ID {id} doesn't exist");
                return NotFound();
            }

            var pointOfInterest = _cityRepository.GetPointOfInterestForCityAsync(cityId, id);
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityRepository.AddPointOfInterestAsync(cityId, finalPointOfInterest);

            await _cityRepository.SaveChangesAsync();

            var createdPoint = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                id = createdPoint.Id
            }, createdPoint);
        }

        [HttpPut("{PointOfInterestId}")]
        public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterest(int cityId, int PointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {

            if (!await _cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointToBePatched = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointToBePatched, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointToBePatched))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointToBePatched, pointOfInterestEntity);

            // point.Name = pointToBePatched.Name;
            // point.Description = pointToBePatched.Description;

            await _cityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityRepository.RemovePointOfInterestAsync(pointOfInterestEntity);

            await _cityRepository.SaveChangesAsync();

            _mailService.Send($"Point with ID {pointOfInterestId} Deleted", "This point was deleted with a call from the delete endpoint");
            return NoContent();

        }
    }
}