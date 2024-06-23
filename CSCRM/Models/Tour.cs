namespace CSCRM.Models
{
    public class Tour : BaseEntity
    {
        public string Name { get; set; }
        public List<Itinerary> Itineraries { get; set; }

    }
}
