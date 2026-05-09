using EventEase.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class AddBookingViewModel
    {
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Please select an event")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please select a venue")]
        public int VenueId { get; set; }

        public DateTime BookingDate { get; set; }

        public List<Event> Events { get; set; } = new();
        public List<Venue> Venues { get; set; } = new();
    }
}
