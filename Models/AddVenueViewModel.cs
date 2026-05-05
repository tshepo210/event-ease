using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class AddVenueViewModel
    {
        public int VenueId { get; set; }
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public string? Capacity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
