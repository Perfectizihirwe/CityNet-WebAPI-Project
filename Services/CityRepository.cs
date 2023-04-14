using CityInfo.API.DbContexts;
using CityInfo.API.Services;
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

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageSize, int pageNumber)
        {

            var collection = _cityContext.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            var totalNumberOfItems = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(pageSize, pageNumber, totalNumberOfItems);

            var collectionToReturn = await collection.OrderBy(c => c.Name).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOI)
        {
            return await _cityContext.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExists(int cityId)
        {
            return await _cityContext.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> PointExists(int pointOfInterestId)
        {
            return await _cityContext.PointsOfInterest.AnyAsync(p => p.Id == pointOfInterestId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityContext.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest?>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityContext.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }

        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityContext.SaveChangesAsync() >= 0);
        }

        public void RemovePointOfInterestAsync(PointOfInterest pointOfInterest)
        {

            _cityContext.PointsOfInterest.Remove(pointOfInterest);
        }

    }
}
