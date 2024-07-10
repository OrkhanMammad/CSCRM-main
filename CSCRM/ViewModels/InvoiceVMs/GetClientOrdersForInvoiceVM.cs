using CSCRM.ViewModels.ClientOrdersVM;

namespace CSCRM.ViewModels.InvoiceVMs
{
    public class GetClientOrdersForInvoiceVM
    {
        public int? ClientId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? InvCode { get; set; }
        public string? ArrivalTime { get; set; }
        public string? ArrivalFlight { get; set; }
        public string? DepartureTime { get; set; }
        public string? DepartureFlight { get; set; }
        public string? CompanyName { get; set; }
        public decimal? SalesAmount { get; set; }


        public List<GetHotelOrdersForInvoiceVM>? HotelOrders { get; set; }
        public List<GetTourOrdersForInvoiceVM>? TourOrders { get; set; }
        public List<GetInclusiveOrdersForInvoiceVM>? InclusiveOrders { get; set; }

    }
}
