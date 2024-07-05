namespace CSCRM.Models
{
    public class TourOrder : BaseEntity
    {   
        public Client Client { get; set; }
        public int ClientID { get; set; }
        public Tour Tour { get; set; }
        public int TourId { get; set; }
        public string CarType { get; set; }
        public bool Guide { get; set; }
        public string Date { get; set; }
    }
}
