using System.Security.Cryptography;
using System.Text;

namespace CityInfo.API.Services
{
    public class AuthenticationServices : IAuthentication
    {
        public string HashPassword(string password)
        {
            var sha = SHA256.Create();
            var asByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = sha.ComputeHash(asByteArray);

            return Convert.ToBase64String(hashedPassword);
        }

        public bool ComparePassword(string password, string passwordHash)
        {
            var sha = SHA256.Create();
            var asByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = sha.ComputeHash(asByteArray);
            if (string.Compare(Convert.ToBase64String(hashedPassword), passwordHash) == 0)
            {
                return true;
            }
            return false;
        }
    }
}