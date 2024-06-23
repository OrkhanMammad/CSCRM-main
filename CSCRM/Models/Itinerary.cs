namespace CSCRM.Models
{
    public class Itinerary
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public int TourId { get; set; }
        public string Description { get; set; }
    }
}
