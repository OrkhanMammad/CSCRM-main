using CSCRM.ViewModels.ItineraryVMS;

namespace CSCRM.ViewModels.TourVMs
{
    public class EditTourVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string>? Itineraries { get; set; } = new List<string>();
    }
}
