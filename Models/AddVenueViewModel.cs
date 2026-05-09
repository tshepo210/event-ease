using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class AddVenueViewModel
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public string? Capacity { get; set; }

        public string? ImageUrl { get; set; }
    }
}