namespace CityInfo.API.Services
{

    public class LocalMailService : IMailService
    {
        private readonly string mailFrom = string.Empty;
        private readonly string mailTo = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
            mailFrom = configuration["mailSettings:mailFrom"];
            mailTo = configuration["mailSettings:mailTo"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {mailFrom} to {mailTo}, " + $"with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}