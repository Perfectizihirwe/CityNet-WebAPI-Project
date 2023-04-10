using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsOfInterest")]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailService;
        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService mailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterest(int cityId)
        {
            try
            {
                // throw new Exception("Hahahahahaha");
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with ID {cityId} not found");
                    return NotFound();
                }
                return Ok(city.PointsOfInterest);
            }
            catch (Exception exe)
            {
                _logger.LogCritical("Exception while getting points of interest", exe);
                return StatusCode(500, "The server has encountered an error that is being fixed");
            }
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var length = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var newPoint = new PointOfInterestDto()
            {
                Id = ++length,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(newPoint);




            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                id = newPoint.Id
            }, newPoint);
        }

        [HttpPut("{PointOfInterestId}")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterest(int cityId, int PointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var point = city.PointsOfInterest.FirstOrDefault(p => p.Id == PointOfInterestId);
            if (point == null)
            {
                return NotFound();
            }
            point.Name = pointOfInterest.Name;
            point.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var point = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (point == null)
            {
                return NotFound();
            }
            var pointToBePatched = new PointOfInterestForUpdateDto()
            {
                Name = point.Name,
                Description = point.Description
            };

            patchDocument.ApplyTo(pointToBePatched, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointToBePatched))
            {
                return BadRequest(ModelState);
            }

            point.Name = pointToBePatched.Name;
            point.Description = pointToBePatched.Description;

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var point = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (point == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(point);
            _mailService.Send($"Point with ID {pointOfInterestId} Deleted", "This point was deleted with a call from the delete endpoint");
            return NoContent();

        }
    }
}