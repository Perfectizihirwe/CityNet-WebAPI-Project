using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {

        [Required(ErrorMessage = "Please, provide a name")]
        [MaxLength(50, ErrorMessage = "Length is too high")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}