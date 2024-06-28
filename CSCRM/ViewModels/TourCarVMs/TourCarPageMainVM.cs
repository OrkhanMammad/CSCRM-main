namespace CSCRM.ViewModels.TourCarVMs
{
    public class TourCarPageMainVM
    {
        public List<GetCarIdNameVM> CarIdNameVM { get; set; }= new List<GetCarIdNameVM>();
        public List<GetTourIdNameVM> TourIdNameVM { get; set; } = new List<GetTourIdNameVM>();
        public List<GetTourCarVM> TourCarVM { get; set; } = new List<GetTourCarVM>();

    }
}
