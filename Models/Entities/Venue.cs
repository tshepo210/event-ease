using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EventEase.Models.Entities
{
    public class Venue
    {
        public int VenueId { get; set; }
        [Required(ErrorMessage = "Venue Name is required.")]
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public string? Capacity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
