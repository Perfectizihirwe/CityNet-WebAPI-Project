namespace CityInfo.API.Models
{
    // <summary>
    // A Dto for a city without point of interests
    // </summary>
    public class CityWithoutPOIDto
    {
        // <summary>
        // Id of the city
        // </summary>

        public int Id { get; set; }

        // <summary>
        // Name of the city
        // </summary>

        public string Name { get; set; } = string.Empty;

        // <summary>
        // Description of the city
        // </summary>

        public string? Description { get; set; }
    }
}