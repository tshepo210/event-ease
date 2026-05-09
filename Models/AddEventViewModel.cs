using System.ComponentModel.DataAnnotations;
using EventEase.Models.Entities;

namespace EventEase.Models
{
    public class AddEventViewModel
    {
        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime? EventDate { get; set; }

        public int? VenueId { get; set; }

        public List<Venue> Venues { get; set; } = new();
    }
}