namespace EventEase.Models.Entities
{
    public class Event
    {
        public int EventId { get; set; } 
        public string? EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        // Foreign Key
        public int VenueId { get; set; }
        // Event type
        public int? EventTypeId { get; set; }
        public EventType? EventType { get; set; }
        // Navigation
        public Venue? Venue { get; set; }
        public ICollection<Booking>? Booking { get; set; }
    }
}
