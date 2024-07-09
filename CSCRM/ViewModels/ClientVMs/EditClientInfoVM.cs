namespace CSCRM.ViewModels.ClientVMs
{
    public class EditClientInfoVM
    {
        public int Id { get; set; }
        public string InvCode { get; set; }
        public string MailCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PaySituation { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal Received { get; set; }
        public string VisaSituation { get; set; }
        public string? Country { get; set; }
        public string? Company { get; set; }
        public string? ArrivalDate { get; set; }
        public string? DepartureDate { get; set; }
        public string? ArrivalTime { get; set; }
        public string? DepartureTime { get; set; }
        public string? ArrivalFlight { get; set; }
        public string? DepartureFlight { get; set; }
    }
}
