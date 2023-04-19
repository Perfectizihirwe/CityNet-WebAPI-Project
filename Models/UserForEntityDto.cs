namespace CityInfo.API.Models
{
    public class UserForEntityDto
    {
        public string Username { get; set; } = string.Empty;

        public byte[]? PasswordHash { get; set; }

        public byte[]? PasswordSalt { get; set; }
    }
}