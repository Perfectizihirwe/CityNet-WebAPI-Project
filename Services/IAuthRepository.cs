namespace CityInfo.API.Entities
{
    public interface IAuthRepository
    {
        void RegisterAsync(User user);

        Task LoginAsync(User user);

        Task<User?> GetUserAsync(string username);

        Task<bool> UserExistsAsync(string userName);

        Task<bool> SaveChangesAsync();
    }
}