using System.ComponentModel.DataAnnotations;

namespace EventEase.Models.Entities
{
    public class Booking
    {
        public int BookingID { get; set; }
        [Required(ErrorMessage = "Event is required.")]
        public int EventID { get; set; }
        public Event? Event { get; set; }   // navigation property
        [Required(ErrorMessage = "Venue is required.")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }   // navigation property
        [Required(ErrorMessage = "Date is required.")]
        public DateTime BookingDate { get; set; }

    }
}
