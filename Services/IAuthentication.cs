namespace CityInfo.API.Services
{
    public interface IAuthentication
    {
        public void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyHashPassword(string password, byte[] passwordHash, byte[] passwordSalt);

    }
}