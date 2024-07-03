namespace CSCRM.Models
{
    public class Client : BaseEntity
    {    
        public string InvCode { get; set; }
        public string MailCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PaySituation { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal Received {  get; set; }
        public decimal Pending { get; set; }
        public string VisaSituation { get; set; }
        public string? Country { get; set; }
        public string? Company { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set ; }

    }
}
