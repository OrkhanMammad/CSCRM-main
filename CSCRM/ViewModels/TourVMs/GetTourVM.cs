using CSCRM.Models;
using CSCRM.ViewModels.ItineraryVMS;

namespace CSCRM.ViewModels.TourVMs
{
    public class GetTourVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GetItineraryVM>? Itineraries { get; set; }
    }
}
