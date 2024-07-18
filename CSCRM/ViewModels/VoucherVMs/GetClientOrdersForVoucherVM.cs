namespace CSCRM.ViewModels.VoucherVMs
{
    public class GetClientOrdersForVoucherVM
    {
        public int? Id { get; set; } = 0;
        public string? ClientName { get; set; } = string.Empty;
        public string? ClientSurname { get; set; } = string.Empty;
        public string? ClientPaxSize {  get; set; } = string.Empty;
        public string? ClientCar { get; set; } = string.Empty;

        public string? ArrivalDate { get; set; } = string.Empty;
        public string? DepartureDate { get; set; } = string.Empty;

        public string? CompanyName { get; set; } = string.Empty;

        public string? CompanyContactPerson { get; set; } = string.Empty;
        public string? CompanyContactPhone { get; set; } = string.Empty;
        public List<string>? InclusiveOrderNames { get; set; } = new List<string>();

        
        public List<GetHotelOrderForVoucherVM>? HotelOrders { get; set; } = new List<GetHotelOrderForVoucherVM>();
        public List<GetTourOrderForVoucherVM>? TourOrders { get; set; } = new List<GetTourOrderForVoucherVM>();

           
    }
}
