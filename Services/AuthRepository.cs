using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{
    public class AuthRepository : IAuthRepository
    {
        public CityContext _citycontext;
        public AuthRepository(CityContext context)
        {
            _citycontext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task LoginAsync(User user)
        {
            throw new NotImplementedException();
        }

        public void RegisterAsync(User user)
        {
            _citycontext.Users.Add(user);
        }

        public async Task<bool> UserExistsAsync(string userName)
        {
            return await _citycontext.Users.AnyAsync(u => u.Username == userName);
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await _citycontext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _citycontext.SaveChangesAsync() >= 0);
        }
    }
}