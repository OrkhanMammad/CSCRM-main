namespace CSCRM.Models
{
    public class Tour : BaseEntity
    {
        public string Name { get; set; }
        public List<Itinerary> Itineraries { get; set; }
        public List<TourByCarType> TourByCarTypes { get; set; }

    }
}
