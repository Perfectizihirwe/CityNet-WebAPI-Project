using CityInfo.API.Services;

namespace CityInfo.API.Entities
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageSize, int pageNumber);

        Task<City?> GetCityAsync(int cityId, bool includePOI);

        Task<bool> CityExists(int cityId);

        Task<bool> PointExists(int pointOfInterestId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

        Task<IEnumerable<PointOfInterest?>> GetPointsOfInterestForCityAsync(int cityId);

        Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync();

        void RemovePointOfInterestAsync(PointOfInterest pointOfInterest);
    }
}

// This is the contract