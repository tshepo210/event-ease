using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class AddBookingViewModel
    {
        public int BookingID { get; set; }
        public int EventID { get; set; }
        public int VenueId { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
