using CSCRM.ViewModels.TourCarVMs;

namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class TourOrdersSectionVM
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int ClientId { get; set; }
        public string StatusCode { get; set; }

        public List<GetTourOrdersVM>? TourOrders { get; set; } = new List<GetTourOrdersVM>();
        public List<GetTourIdNameVM>? Tours { get; set; } = new List<GetTourIdNameVM>();

        public List<GetCarIdNameVM>? Cars { get; set; } = new List<GetCarIdNameVM>();
    }
}
