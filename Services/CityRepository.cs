using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{

    public class CityRepository : ICityRepository
    {

        private readonly CityContext _cityContext;
        public CityRepository(CityContext cityContext)
        {
            _cityContext = cityContext ?? throw new ArgumentNullException(nameof(cityContext));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOI)
        {
            return await _cityContext.Cities.Include(c => c.PointOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityContext.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest?>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityContext.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
        }
    }
}