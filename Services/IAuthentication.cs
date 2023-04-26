namespace CityInfo.API.Services
{
    public interface IAuthentication
    {
        public string HashPassword(string password);
        public bool ComparePassword(string password, string passwordHash);

    }
}