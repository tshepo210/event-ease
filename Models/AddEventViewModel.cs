namespace EventEase.Models
{
    public class AddEventViewModel
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        // Foreign Key
        public int VenueId { get; set; }
    }
}
