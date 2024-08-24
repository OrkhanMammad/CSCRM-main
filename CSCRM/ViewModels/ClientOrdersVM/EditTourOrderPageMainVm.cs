using CSCRM.ViewModels.TourCarVMs;

namespace CSCRM.ViewModels.ClientOrdersVM
{
    public class EditTourOrderPageMainVm
    {
        public EditTourOrderVM? TourOrder { get; set; } = new();
        public List<GetTourIdNameVM>? Tours { get; set; } = new();

        public List<GetCarIdNameVM>? Cars { get; set; } = new();
    }
}
