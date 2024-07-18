namespace CSCRM.ViewModels.VoucherVMs
{
    public class GetTourForVoucherVM
    {
        public string? TourName { get; set; } = string.Empty;
        public List<string> Itineraries { get; set; } = new List<string>();
    }
}
