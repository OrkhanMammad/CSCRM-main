namespace CSCRM.ViewModels.ConfirmationVMs
{
    public class GetClientOrdersForConfirmationVM
    {
        public string? InvCode { get; set; }
        public string? MailCode { get; set; }
        public string? CompanyName { get; set; }
        public string? PaxsSize { get; set; }
        public string? CarType { get; set; }
        public bool Guide { get; set; } = false;
        public decimal? SalesAmount { get; set; }
        public decimal? Received {  get; set; }
        public decimal? Pending { get; set; }
        public string? PaymentSituation { get; set; }
        public string? VisaSituation { get; set; }
        public string? ArrivalDate { get; set; }
        public string? DepartureDate { get; set; }
        public string? Country { get; set;}
        public string? MarkupTotal { get; set; }
        public string? Note {  get; set; }
        public List<GetHotelOrderForConfirmationVM> HotelOrders { get; set; } = new List<GetHotelOrderForConfirmationVM>();
    }
}
