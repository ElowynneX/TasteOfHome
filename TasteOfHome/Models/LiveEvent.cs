namespace TasteOfHome.Models
{
    public class LiveEvent
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string CultureTag { get; set; } = "";
        public string Description { get; set; } = "";
        public string City { get; set; } = "";
        public string VenueName { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime? EventDateTimeUtc { get; set; }
        public string LocalDate { get; set; } = "";
        public string LocalTime { get; set; } = "";
        public string TimeZone { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string TicketUrl { get; set; } = "";
        public string Status { get; set; } = "";
        public string PriceRangeText { get; set; } = "";
        public bool HasTickets { get; set; }
        public string Source { get; set; } = "Ticketmaster";
    }
}